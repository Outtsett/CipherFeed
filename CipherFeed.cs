using CipherFeed.Core;
using CipherFeed.Indicators;
using CipherFeed.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Threading.Tasks;
using TradingPlatform.BusinessLayer;

namespace CipherFeed
{
    public class CipherFeed : Strategy
    {
        [InputParameter("Account", 1)]
        public Account CurrentAccount { get; set; }

        [InputParameter("Log Interval (seconds)", 2)]
        public int LogIntervalSeconds { get; set; } = 5;

        [InputParameter("Log Orderflow Features", 3)]
        public bool LogOrderflowFeatures { get; set; } = false;

        [InputParameter("Enable CSV Logging", 4)]
        public bool EnableCSVLogging { get; set; } = true;

        [InputParameter("CSV Log Directory", 5)]
        public string CSVLogDirectory { get; set; } = @"C:\CipherFeed\Logs";

        private readonly string[] symbolNames = { "MNQ", "MES", "M2K", "MYM", "ENQ", "EP", "RTY", "YM" };
        private readonly Dictionary<string, Symbol> symbols = [];
        private readonly Dictionary<Symbol, string> symbolToRoot = [];
        private readonly Dictionary<Symbol, double> sessionOpenPrices = [];
        private readonly Dictionary<Symbol, bool> sessionOpenInitialized = [];
        private readonly Dictionary<Symbol, double> currentPrices = [];

        private DateTime lastLogTime;

        // Market data snapshots per symbol
        private readonly Dictionary<Symbol, MarketDataSnapshot> latestSnapshots = [];

        // Indicators per symbol
        private readonly Dictionary<Symbol, HistoricalData> historicalDataCache = [];
        private readonly Dictionary<Symbol, SessionAnchoredVWAP> vwapIndicators = [];
        private readonly Dictionary<Symbol, VPOCIndicator> vpocIndicators = [];
        private readonly Dictionary<Symbol, TWAPIndicator> twapIndicators = [];

        // Core managers
        private SessionManager sessionManager;
        private SymbolLogger symbolLogger;
        private CSVDataExporter csvExporter;

        // Metrics tracking (per symbol)
        private readonly Dictionary<Symbol, long> ticksReceived = [];
        private readonly Dictionary<Symbol, long> quotesReceived = [];
        private readonly Dictionary<Symbol, long> csvWritesCompleted = [];
        private readonly Dictionary<Symbol, double> lastTickVolume = [];
        private readonly Dictionary<Symbol, long> volumeSpikesDetected = [];
        private readonly Dictionary<Symbol, DateTime> lastTickTime = [];

        // Connection reference for latency metrics
        private Connection connection;

        public CipherFeed()
        {
            Name = "CipherFeed";
        }

        protected override void OnCreated()
        {
            base.OnCreated();

            // Initialize core managers (created once, not on restarts)
            sessionManager = new SessionManager();
            symbolLogger = new SymbolLogger(Log);
        }

        protected override void OnInitializeMetrics(Meter meter)
        {
            // Session open price gauges per symbol
            foreach (string symbolName in symbolNames)
            {
                string metricName = $"{symbolName}_session_open";
                _ = meter.CreateObservableGauge(metricName, () =>
                {
                    Symbol symbol = symbols.ContainsKey(symbolName) ? symbols[symbolName] : null;
                    return symbol != null && sessionOpenPrices.ContainsKey(symbol) ? sessionOpenPrices[symbol] : 0.0;
                }, unit: "price", description: $"Session open price for {symbolName}");
            }

            // Data I/O metrics per symbol
            foreach (string symbolName in symbolNames)
            {
                // Ticks received counter
                _ = meter.CreateObservableCounter($"{symbolName}_ticks_received", () =>
                {
                    Symbol symbol = symbols.ContainsKey(symbolName) ? symbols[symbolName] : null;
                    return symbol != null && ticksReceived.ContainsKey(symbol) ? ticksReceived[symbol] : 0;
                }, unit: "ticks", description: $"Total trade ticks received for {symbolName}");

                // Quotes received counter
                _ = meter.CreateObservableCounter($"{symbolName}_quotes_received", () =>
                {
                    Symbol symbol = symbols.ContainsKey(symbolName) ? symbols[symbolName] : null;
                    return symbol != null && quotesReceived.ContainsKey(symbol) ? quotesReceived[symbol] : 0;
                }, unit: "quotes", description: $"Total quote updates received for {symbolName}");

                // CSV writes counter
                _ = meter.CreateObservableCounter($"{symbolName}_csv_writes", () =>
                {
                    Symbol symbol = symbols.ContainsKey(symbolName) ? symbols[symbolName] : null;
                    return symbol != null && csvWritesCompleted.ContainsKey(symbol) ? csvWritesCompleted[symbol] : 0;
                }, unit: "writes", description: $"Total CSV writes completed for {symbolName}");

                // Volume spikes counter
                _ = meter.CreateObservableCounter($"{symbolName}_volume_spikes", () =>
                {
                    Symbol symbol = symbols.ContainsKey(symbolName) ? symbols[symbolName] : null;
                    return symbol != null && volumeSpikesDetected.ContainsKey(symbol) ? volumeSpikesDetected[symbol] : 0;
                }, unit: "spikes", description: $"Total volume spikes detected for {symbolName}");

                // Current tick volume gauge
                _ = meter.CreateObservableGauge($"{symbolName}_current_volume", () =>
                {
                    Symbol symbol = symbols.ContainsKey(symbolName) ? symbols[symbolName] : null;
                    return symbol != null && lastTickVolume.ContainsKey(symbol) ? lastTickVolume[symbol] : 0.0;
                }, unit: "contracts", description: $"Current tick volume for {symbolName}");

                // Round-trip latency gauge (milliseconds since last tick)
                _ = meter.CreateObservableGauge($"{symbolName}_tick_latency_ms", () =>
                {
                    Symbol symbol = symbols.ContainsKey(symbolName) ? symbols[symbolName] : null;
                    return symbol != null && lastTickTime.ContainsKey(symbol) ? (DateTime.UtcNow - lastTickTime[symbol]).TotalMilliseconds : 0.0;
                }, unit: "ms", description: $"Milliseconds since last tick for {symbolName}");
            }

            // Aggregate metrics across all symbols
            _ = meter.CreateObservableCounter("total_ticks_received", ticksReceived.Values.Sum, unit: "ticks", description: "Total trade ticks received across all symbols");

            _ = meter.CreateObservableCounter("total_quotes_received", quotesReceived.Values.Sum, unit: "quotes", description: "Total quote updates received across all symbols");

            _ = meter.CreateObservableCounter("total_csv_writes", csvWritesCompleted.Values.Sum, unit: "writes", description: "Total CSV writes completed across all symbols");

            _ = meter.CreateObservableCounter("total_volume_spikes", volumeSpikesDetected.Values.Sum, unit: "spikes", description: "Total volume spikes detected across all symbols");

            _ = meter.CreateObservableGauge("active_symbols_count", () =>
            {
                return symbols.Count;
            }, unit: "symbols", description: "Number of active symbols being tracked");

            // Connection-level latency metrics
            _ = meter.CreateObservableGauge("connection_ping_ms", () =>
            {
                return connection != null && connection.PingTime.HasValue ? connection.PingTime.Value.TotalMilliseconds : 0.0;
            }, unit: "ms", description: "Connection ping time in milliseconds");

            _ = meter.CreateObservableGauge("connection_round_trip_ms", () =>
            {
                return connection != null && connection.RoundTripTime.HasValue ? connection.RoundTripTime.Value.TotalMilliseconds : 0.0;
            }, unit: "ms", description: "Connection round-trip time in milliseconds");

            _ = meter.CreateObservableGauge("connection_queue_depth", () =>
            {
                return connection != null ? connection.MessagesQueueDepth : 0;
            }, unit: "messages", description: "Number of messages waiting to be processed");

            _ = meter.CreateObservableGauge("connection_uptime_seconds", () =>
            {
                return connection != null ? connection.Uptime.TotalSeconds : 0.0;
            }, unit: "seconds", description: "Connection uptime in seconds");

            base.OnInitializeMetrics(meter);
        }

        protected override void OnRun()
        {
            if (CurrentAccount == null)
            {
                Log("No account selected", StrategyLoggingLevel.Error);
                Stop();
                return;
            }

            DateTime now = TradingPlatform.BusinessLayer.Core.Instance.TimeUtils.DateTimeUtcNow;

            // Get connection from account for latency metrics
            connection = CurrentAccount.Connection;

            if (connection == null)
            {
                Log($"Could not find connection for account {CurrentAccount.Name}", StrategyLoggingLevel.Error);
                Stop();
                return;
            }

            sessionManager.Initialize(now);
            sessionManager.SessionChanged += OnSessionChanged;
            lastLogTime = now;

            Log($"Current session: {sessionManager.CurrentSession} | Start: {sessionManager.SessionStartTime:yyyy-MM-dd HH:mm} UTC", StrategyLoggingLevel.Trading);
            Log($"Connected to: {connection.Name} ({connection.VendorName}) via {CurrentAccount.Name}", StrategyLoggingLevel.Trading);

            // Log connection latency info if available
            if (connection.PingTime.HasValue)
            {
                Log($"Connection Ping: {connection.PingTime.Value.TotalMilliseconds:F2}ms", StrategyLoggingLevel.Trading);
            }
            if (connection.RoundTripTime.HasValue)
            {
                Log($"Connection RTT: {connection.RoundTripTime.Value.TotalMilliseconds:F2}ms", StrategyLoggingLevel.Trading);
            }

            Log($"Starting parallel initialization of {symbolNames.Length} symbols...", StrategyLoggingLevel.Trading);

            DateTime parallelStart = DateTime.UtcNow;

            _ = Parallel.ForEach(symbolNames, symbolRoot =>
            {
                DateTime symbolStart = DateTime.UtcNow;

                try
                {
                    List<Symbol> candidates = TradingPlatform.BusinessLayer.Core.Instance.Symbols
                        .Where(s => s.ConnectionId == CurrentAccount.ConnectionId &&
                                   s.Name.StartsWith(symbolRoot) &&
                                   s.SymbolType == SymbolType.Futures &&
                                   s.ExpirationDate > now)
                        .OrderBy(s => s.ExpirationDate)
                        .ToList();

                    if (!candidates.Any())
                    {
                        Log($"{symbolRoot} - no active contracts found", StrategyLoggingLevel.Error);
                        return;
                    }

                    Symbol symbol = candidates.First();

                    lock (symbols)
                    {
                        symbols[symbolRoot] = symbol;
                        symbolToRoot[symbol] = symbolRoot;
                        sessionOpenInitialized[symbol] = false;
                        currentPrices[symbol] = 0.0;

                        // Initialize metrics trackers
                        ticksReceived[symbol] = 0;
                        quotesReceived[symbol] = 0;
                        csvWritesCompleted[symbol] = 0;
                        lastTickVolume[symbol] = 0.0;
                        volumeSpikesDetected[symbol] = 0;
                        lastTickTime[symbol] = DateTime.UtcNow;
                    }

                    symbol.NewLast += (s, last) => OnNewLast_ForSymbol(symbolRoot, s, last);
                    symbol.NewQuote += (s, quote) => OnNewQuote_ForSymbol(symbolRoot, s, quote);

                    InitializeSessionOpen(symbol);
                    InitializeIndicatorsForSymbol(symbol);

                    TimeSpan symbolDuration = DateTime.UtcNow - symbolStart;
                    Log($"✓ {symbol.Name} initialized in {symbolDuration.TotalMilliseconds:F0}ms (Expires: {symbol.ExpirationDate:yyyy-MM-dd})", StrategyLoggingLevel.Trading);
                }
                catch (Exception ex)
                {
                    Log($"✗ Error initializing {symbolRoot}: {ex.Message}", StrategyLoggingLevel.Error);
                }
            });

            TimeSpan parallelDuration = DateTime.UtcNow - parallelStart;

            if (symbols.Count == 0)
            {
                Log("Failed to subscribe to any symbols", StrategyLoggingLevel.Error);
                Stop();
                return;
            }

            Log($"Parallel initialization complete in {parallelDuration.TotalMilliseconds:F0}ms (avg: {parallelDuration.TotalMilliseconds / symbols.Count:F0}ms/symbol)", StrategyLoggingLevel.Trading);
            Log($"Running with {symbols.Count}/{symbolNames.Length} symbols | Batch logging every {LogIntervalSeconds} seconds", StrategyLoggingLevel.Trading);

            if (EnableCSVLogging)
            {
                try
                {
                    csvExporter = new CSVDataExporter(CSVLogDirectory, sessionManager.CurrentSession, sessionManager.SessionStartTime);

                    foreach (string symbolRoot in symbolNames)
                    {
                        if (symbols.ContainsKey(symbolRoot))
                        {
                            csvExporter.InitializeSymbolFile(symbolRoot);
                        }
                    }

                    Log($"CSV logging enabled for {symbols.Count} symbols in: {CSVLogDirectory}", StrategyLoggingLevel.Trading);
                }
                catch (Exception ex)
                {
                    Log($"Failed to initialize CSV exporter: {ex.Message}", StrategyLoggingLevel.Error);
                    csvExporter = null;
                }
            }
        }

        private void InitializeIndicatorsForSymbol(Symbol symbol)
        {
            try
            {
                HistoryRequestParameters historyParams = new()
                {
                    Symbol = symbol,
                    FromTime = sessionManager.SessionStartTime,
                    ToTime = TradingPlatform.BusinessLayer.Core.Instance.TimeUtils.DateTimeUtcNow,
                    Aggregation = new HistoryAggregationTime(Period.MIN1, symbol.HistoryType)
                };

                HistoricalData historicalData = symbol.GetHistory(historyParams);

                if (historicalData != null)
                {
                    historicalDataCache[symbol] = historicalData;

                    SessionAnchoredVWAP vwap = new()
                    {
                        UseTypicalPrice = true,
                        ShowStdDevBands = true,
                        ShowMPDBands = true
                    };
                    historicalData.AddIndicator(vwap);
                    vwapIndicators[symbol] = vwap;

                    VPOCIndicator vpoc = new();
                    historicalData.AddIndicator(vpoc);
                    vpocIndicators[symbol] = vpoc;

                    TWAPIndicator twap = new()
                    {
                        UseTypicalPrice = true,
                        ShowStdDevBands = true
                    };
                    historicalData.AddIndicator(twap);
                    twapIndicators[symbol] = twap;

                    Log($"[{sessionManager.CurrentSession}] {symbol.Name} indicators initialized with {historicalData.Count} bars", StrategyLoggingLevel.Trading);
                }
                else
                {
                    Log($"[{sessionManager.CurrentSession}] {symbol.Name} - Failed to get historical data for indicators", StrategyLoggingLevel.Error);
                }
            }
            catch (Exception ex)
            {
                Log($"[{sessionManager.CurrentSession}] {symbol.Name} - Error initializing indicators: {ex.Message}", StrategyLoggingLevel.Error);
            }
        }

        private void InitializeSessionOpen(Symbol symbol)
        {
            // Use SessionManager to get session open price from historical data
            double? sessionOpen = sessionManager.GetSessionOpenPrice(symbol, out string message);

            if (sessionOpen.HasValue)
            {
                sessionOpenPrices[symbol] = sessionOpen.Value;
                sessionOpenInitialized[symbol] = true;
                Log($"[{sessionManager.CurrentSession}] {symbol.Name} {message}", StrategyLoggingLevel.Trading);
            }
            else
            {
                // No historical data found - will use first tick as fallback
                Log($"[{sessionManager.CurrentSession}] {symbol.Name} - {message}", StrategyLoggingLevel.Trading);
            }
        }

        private void OnNewLast_ForSymbol(string symbolRoot, Symbol symbol, Last last)
        {
            if (symbol == null || last == null)
            {
                return;
            }

            // Update metrics
            if (ticksReceived.ContainsKey(symbol))
            {
                ticksReceived[symbol]++;
                lastTickTime[symbol] = DateTime.UtcNow;
                lastTickVolume[symbol] = last.Size;

                // Detect volume spikes (volume > 2x previous tick volume)
                if (lastTickVolume.ContainsKey(symbol) && lastTickVolume[symbol] > 0)
                {
                    if (last.Size > lastTickVolume[symbol] * 2.0)
                    {
                        volumeSpikesDetected[symbol]++;
                    }
                }
            }

            // Check session boundary using SessionManager
            _ = sessionManager.CheckBoundary(last.Time);

            // If session open hasn't been initialized yet, use this tick
            if (!sessionOpenInitialized.ContainsKey(symbol) || !sessionOpenInitialized[symbol])
            {
                if (!sessionOpenPrices.ContainsKey(symbol))
                {
                    sessionOpenPrices[symbol] = last.Price;
                    sessionOpenInitialized[symbol] = true;
                    Log($"[{sessionManager.CurrentSession}] {symbol.Name} session open from first tick: {last.Price:F2} (tick time: {last.Time:yyyy-MM-dd HH:mm:ss})", StrategyLoggingLevel.Trading);
                }
                return;
            }

            currentPrices[symbol] = last.Price;

            // Get or create snapshot
            if (!latestSnapshots.ContainsKey(symbol))
            {
                latestSnapshots[symbol] = new MarketDataSnapshot
                {
                    SymbolName = symbol.Name,
                    SessionOpen = sessionOpenPrices.ContainsKey(symbol) ? sessionOpenPrices[symbol] : 0.0
                };
            }

            // Update snapshot using its own UpdateFromLast method
            latestSnapshots[symbol].UpdateFromLast(symbol, last);

            // Update indicator session opens
            if (vwapIndicators.ContainsKey(symbol))
            {
                latestSnapshots[symbol].VWAPSessionOpen = vwapIndicators[symbol].GetSessionOpen();
            }
            if (vpocIndicators.ContainsKey(symbol))
            {
                latestSnapshots[symbol].VPOCSessionOpen = vpocIndicators[symbol].GetSessionOpen();
            }
            if (twapIndicators.ContainsKey(symbol))
            {
                latestSnapshots[symbol].TWAPSessionOpen = twapIndicators[symbol].GetSessionOpen();
            }

            // Write to CSV immediately on every tick if enabled
            if (EnableCSVLogging && csvExporter != null && latestSnapshots.ContainsKey(symbol))
            {
                try
                {
                    csvExporter.WriteSnapshot(
                        symbolRoot,
                        symbol,
                        latestSnapshots[symbol],
                        sessionOpenPrices.ContainsKey(symbol) ? sessionOpenPrices[symbol] : 0.0,
                        vwapIndicators.ContainsKey(symbol) ? vwapIndicators[symbol] : null,
                        vpocIndicators.ContainsKey(symbol) ? vpocIndicators[symbol] : null,
                        twapIndicators.ContainsKey(symbol) ? twapIndicators[symbol] : null);

                    // Increment CSV write counter
                    if (csvWritesCompleted.ContainsKey(symbol))
                    {
                        csvWritesCompleted[symbol]++;
                    }
                }
                catch (Exception ex)
                {
                    Log($"Error writing CSV for {symbolRoot}: {ex.Message}", StrategyLoggingLevel.Error);
                }
            }

            // Check if it's time to log to console
            if ((last.Time - lastLogTime).TotalSeconds >= LogIntervalSeconds)
            {
                // Use SymbolLogger for batch logging
                symbolLogger.LogAllSymbols(
                    sessionManager.CurrentSession,
                    last.Time,
                    symbols,
                    sessionOpenPrices,
                    currentPrices,
                    vwapIndicators,
                    vpocIndicators,
                    twapIndicators,
                    latestSnapshots,
                    LogOrderflowFeatures);

                lastLogTime = last.Time;
            }
        }

        private void OnNewQuote_ForSymbol(string symbolRoot, Symbol symbol, Quote quote)
        {
            if (symbol == null || quote == null)
            {
                return;
            }

            // Update metrics
            if (quotesReceived.ContainsKey(symbol))
            {
                quotesReceived[symbol]++;
            }

            // Get or create snapshot
            if (!latestSnapshots.ContainsKey(symbol))
            {
                latestSnapshots[symbol] = new MarketDataSnapshot
                {
                    SymbolName = symbol.Name,
                    SessionOpen = sessionOpenPrices.ContainsKey(symbol) ? sessionOpenPrices[symbol] : 0.0
                };
            }

            // Update snapshot using its own UpdateFromQuote method
            latestSnapshots[symbol].UpdateFromQuote(quote);
        }

        private void OnSessionChanged(TradingSession oldSession, TradingSession newSession, DateTime newSessionStart)
        {
            Log($"\n🔔 SESSION CHANGE: {oldSession} → {newSession} | Start: {newSessionStart:yyyy-MM-dd HH:mm} UTC\n", StrategyLoggingLevel.Trading);

            sessionOpenPrices.Clear();
            sessionOpenInitialized.Clear();
            currentPrices.Clear();

            // Reset all snapshots' cumulative state
            foreach (MarketDataSnapshot snapshot in latestSnapshots.Values)
            {
                snapshot.ResetCumulativeState();
            }
            latestSnapshots.Clear();

            // Reset all metrics for new session
            foreach (Symbol symbol in symbols.Values)
            {
                if (ticksReceived.ContainsKey(symbol))
                {
                    ticksReceived[symbol] = 0;
                }
                if (quotesReceived.ContainsKey(symbol))
                {
                    quotesReceived[symbol] = 0;
                }
                if (csvWritesCompleted.ContainsKey(symbol))
                {
                    csvWritesCompleted[symbol] = 0;
                }
                if (lastTickVolume.ContainsKey(symbol))
                {
                    lastTickVolume[symbol] = 0.0;
                }
                if (volumeSpikesDetected.ContainsKey(symbol))
                {
                    volumeSpikesDetected[symbol] = 0;
                }
                if (lastTickTime.ContainsKey(symbol))
                {
                    lastTickTime[symbol] = newSessionStart;
                }
            }

            lastLogTime = newSessionStart;

            // Re-initialize CSV exporter for new session
            if (EnableCSVLogging)
            {
                try
                {
                    csvExporter = new CSVDataExporter(CSVLogDirectory, newSession, newSessionStart);

                    foreach (string symbolRoot in symbolNames)
                    {
                        csvExporter.InitializeSymbolFile(symbolRoot);
                    }

                    Log($"CSV logging reinitialized for new session - {symbolNames.Length} files created", StrategyLoggingLevel.Trading);
                }
                catch (Exception ex)
                {
                    Log($"Failed to reinitialize CSV exporter: {ex.Message}", StrategyLoggingLevel.Error);
                    csvExporter = null;
                }
            }

            // Re-initialize session open and indicators for all symbols
            foreach (Symbol symbol in symbols.Values)
            {
                sessionOpenInitialized[symbol] = false;
                currentPrices[symbol] = 0.0;

                InitializeSessionOpen(symbol);

                // Clean up old indicators
                if (historicalDataCache.ContainsKey(symbol))
                {
                    HistoricalData oldHistory = historicalDataCache[symbol];
                    if (vwapIndicators.ContainsKey(symbol))
                    {
                        oldHistory.RemoveIndicator(vwapIndicators[symbol]);
                    }
                    if (vpocIndicators.ContainsKey(symbol))
                    {
                        oldHistory.RemoveIndicator(vpocIndicators[symbol]);
                    }
                    if (twapIndicators.ContainsKey(symbol))
                    {
                        oldHistory.RemoveIndicator(twapIndicators[symbol]);
                    }
                }

                // Re-initialize indicators for new session
                InitializeIndicatorsForSymbol(symbol);
            }
        }

        public MarketDataSnapshot GetMarketDataSnapshot(Symbol symbol)
        {
            return latestSnapshots.ContainsKey(symbol) ? latestSnapshots[symbol] : null;
        }

        public MarketDataSnapshot GetMarketDataSnapshot(string symbolRoot)
        {
            return symbols.ContainsKey(symbolRoot) ? GetMarketDataSnapshot(symbols[symbolRoot]) : null;
        }

        protected override void OnStop()
        {
            // Unsubscribe from session events
            if (sessionManager != null)
            {
                sessionManager.SessionChanged -= OnSessionChanged;
            }

            // Clean up indicators
            foreach (Symbol symbol in symbols.Values)
            {
                if (historicalDataCache.ContainsKey(symbol))
                {
                    HistoricalData history = historicalDataCache[symbol];
                    if (vwapIndicators.ContainsKey(symbol))
                    {
                        history.RemoveIndicator(vwapIndicators[symbol]);
                    }
                    if (vpocIndicators.ContainsKey(symbol))
                    {
                        history.RemoveIndicator(vpocIndicators[symbol]);
                    }
                    if (twapIndicators.ContainsKey(symbol))
                    {
                        history.RemoveIndicator(twapIndicators[symbol]);
                    }
                }
            }

            symbols.Clear();
            symbolToRoot.Clear();
            sessionOpenPrices.Clear();
            sessionOpenInitialized.Clear();
            historicalDataCache.Clear();
            vwapIndicators.Clear();
            vpocIndicators.Clear();
            twapIndicators.Clear();
            currentPrices.Clear();
            latestSnapshots.Clear();

            // Clear metrics dictionaries
            ticksReceived.Clear();
            quotesReceived.Clear();
            csvWritesCompleted.Clear();
            lastTickVolume.Clear();
            volumeSpikesDetected.Clear();
            lastTickTime.Clear();

            sessionManager = null;
            symbolLogger = null;
            csvExporter = null;
            connection = null;

            Log("Stopped", StrategyLoggingLevel.Trading);
        }
    }
}
