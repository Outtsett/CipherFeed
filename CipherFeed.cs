using CipherFeed.Indicators;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using TradingPlatform.BusinessLayer;

namespace CipherFeed
{
    public enum TradingSession
    {
        RTH,
        ETH
    }

    /// <summary>
    /// Market data snapshot containing all orderflow features for a single symbol at a point in time
    /// </summary>
    public class MarketDataSnapshot
    {
        public DateTime Timestamp { get; set; }
        public string SymbolName { get; set; }

        // Price and Trade Information
        public double Last { get; set; }
        public double Size { get; set; }
        public AggressorFlag Aggressor { get; set; }
        public TickDirection TickDirection { get; set; }

        // Bid/Ask Information
        public double BidPrice { get; set; }
        public double BidSize { get; set; }
        public TickDirection BidTickDirection { get; set; }
        public double AskPrice { get; set; }
        public double AskSize { get; set; }
        public TickDirection AskTickDirection { get; set; }

        // Volume and Delta
        public double Volume { get; set; }
        public long Trades { get; set; }
        public double Delta { get; set; }
        public double DeltaPercent { get; set; }
        public double CumulativeDelta { get; set; }

        // Buy/Sell Volume
        public double BuyVolume { get; set; }
        public double BuyVolumePercent { get; set; }
        public double SellVolume { get; set; }
        public double SellVolumePercent { get; set; }

        // Buy/Sell Trades
        public int BuyTrades { get; set; }
        public int SellTrades { get; set; }

        // Imbalance
        public double Imbalance { get; set; }
        public double ImbalancePercent { get; set; }

        // Average Sizes
        public double AverageSize { get; set; }
        public double AverageBuySize { get; set; }
        public double AverageSellSize { get; set; }

        // Max Trade Volume1
        public double MaxOneTradeVolume { get; set; }
        public double MaxOneTradeVolumePercent { get; set; }

        // Filtered Volume1
        public double FilteredVolume { get; set; }
        public double FilteredVolumePercent { get; set; }
        public double FilteredBuyVolume { get; set; }
        public double FilteredBuyVolumePercent { get; set; }
        public double FilteredSellVolume { get; set; }
        public double FilteredSellVolumePercent { get; set; }

        // VWAP Bid/Ask
        public double VWAPBid { get; set; }
        public double VWAPAsk { get; set; }

        // Cumulative Sizes
        public double CumulativeSizeBid { get; set; }
        public double CumulativeSizeAsk { get; set; }
        public double CumulativeSize { get; set; }

        // Liquidity Changes
        public double BidsLiquidityChanges { get; set; }
        public double AsksLiquidityChanges { get; set; }
        public int BidsNumberOfChanges { get; set; }
        public int AsksNumberOfChanges { get; set; }

        // Trade Sizes by Side
        public double LastTradeSize { get; set; }
        public double BidTradeSize { get; set; }
        public double AskTradeSize { get; set; }

        // Time Information
        public DateTime TimeBid { get; set; }
        public DateTime TimeAsk { get; set; }

        // Session Open (for percentage calculations)
        public double SessionOpen { get; set; }

        // Indicator Session Opens (for validation/analysis)
        public double VWAPSessionOpen { get; set; }
        public double VPOCSessionOpen { get; set; }
        public double TWAPSessionOpen { get; set; }
    }

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
        private readonly Dictionary<Symbol, string> symbolToRoot = [];  // Reverse lookup for fast access
        private readonly Dictionary<Symbol, double> sessionOpenPrices = [];
        private readonly Dictionary<Symbol, bool> sessionOpenInitialized = [];

        // Current prices for batch logging
        private readonly Dictionary<Symbol, double> currentPrices = [];
        private DateTime lastLogTime;

        // Market data snapshots per symbol
        private readonly Dictionary<Symbol, MarketDataSnapshot> latestSnapshots = [];

        // Cumulative tracking for orderflow features
        private readonly Dictionary<Symbol, double> cumulativeDelta = [];
        private readonly Dictionary<Symbol, double> cumulativeBuyVolume = [];
        private readonly Dictionary<Symbol, double> cumulativeSellVolume = [];
        private readonly Dictionary<Symbol, double> cumulativeSizeBid = [];
        private readonly Dictionary<Symbol, double> cumulativeSizeAsk = [];

        // Indicators per symbol
        private readonly Dictionary<Symbol, HistoricalData> historicalDataCache = [];
        private readonly Dictionary<Symbol, SessionAnchoredVWAP> vwapIndicators = [];
        private readonly Dictionary<Symbol, VPOCIndicator> vpocIndicators = [];
        private readonly Dictionary<Symbol, TWAPIndicator> twapIndicators = [];

        // CSV Data Exporter
        private CSVDataExporter csvExporter;

        // Session times: PST to UTC conversion (PST + 8 hours = UTC)
        // RTH: 4:00 AM - 1:45 PM PST = 12:00 - 21:45 UTC
        // ETH: 3:15 PM - 4:00 AM PST = 23:15 UTC - 12:00 UTC (next day)
        private static readonly TimeSpan RTH_START = new(12, 0, 0);   // 12:00 UTC (4am PST)
        private static readonly TimeSpan RTH_END = new(21, 45, 0);    // 21:45 UTC (1:45pm PST)
        private static readonly TimeSpan ETH_START = new(23, 15, 0);  // 23:15 UTC (3:15pm PST)
        private static readonly TimeSpan ETH_END = new(12, 0, 0);     // 12:00 UTC (4am PST next day)

        private TradingSession currentSession;
        private DateTime sessionStartTime;
        private DateTime lastCheckTime;

        public CipherFeed()
        {
            Name = "CipherFeed";
        }

        protected override void OnInitializeMetrics(Meter meter)
        {
            // Create separate gauge for each symbol's session open price
            foreach (string symbolName in symbolNames)
            {
                string metricName = $"{symbolName}_session_open";
                _ = meter.CreateObservableGauge(metricName, () =>
                {
                    Symbol symbol = symbols.ContainsKey(symbolName) ? symbols[symbolName] : null;
                    return symbol != null && sessionOpenPrices.ContainsKey(symbol) ? sessionOpenPrices[symbol] : 0.0;
                }, unit: "price", description: $"Session open price for {symbolName}");
            }

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

            DateTime now = Core.Instance.TimeUtils.DateTimeUtcNow;
            currentSession = GetSession(now);
            sessionStartTime = GetSessionStart(now, currentSession);
            lastCheckTime = now;
            lastLogTime = now;

            Log($"Current session: {currentSession} | Start: {sessionStartTime:yyyy-MM-dd HH:mm} UTC", StrategyLoggingLevel.Trading);

            foreach (string symbolRoot in symbolNames)
            {
                List<Symbol> candidates = Core.Instance.Symbols
                    .Where(s => s.ConnectionId == CurrentAccount.ConnectionId &&
                               s.Name.StartsWith(symbolRoot) &&
                               s.SymbolType == SymbolType.Futures &&
                               s.ExpirationDate > now)
                    .OrderBy(s => s.ExpirationDate)
                    .ToList();

                if (!candidates.Any())
                {
                    Log($"{symbolRoot} - no active contracts found", StrategyLoggingLevel.Error);
                    continue;
                }

                Symbol symbol = candidates.First();
                symbols[symbolRoot] = symbol;
                symbolToRoot[symbol] = symbolRoot;  // Store reverse mapping

                // Create dedicated event handler for this symbol using lambda closure
                symbol.NewLast += (s, last) => OnNewLast_ForSymbol(symbolRoot, s, last);
                symbol.NewQuote += (s, quote) => OnNewQuote_ForSymbol(symbolRoot, s, quote);

                sessionOpenInitialized[symbol] = false;
                currentPrices[symbol] = 0.0;

                // Initialize cumulative tracking
                cumulativeDelta[symbol] = 0.0;
                cumulativeBuyVolume[symbol] = 0.0;
                cumulativeSellVolume[symbol] = 0.0;
                cumulativeSizeBid[symbol] = 0.0;
                cumulativeSizeAsk[symbol] = 0.0;

                // Try to get session open from historical data
                InitializeSessionOpen(symbol);

                // Initialize indicators for this symbol
                InitializeIndicatorsForSymbol(symbol);

                Log($"✓ Subscribed to {symbol.Name} (Expires: {symbol.ExpirationDate:yyyy-MM-dd})", StrategyLoggingLevel.Trading);
            }

            if (symbols.Count == 0)
            {
                Log("Failed to subscribe to any symbols", StrategyLoggingLevel.Error);
                Stop();
                return;
            }

            Log($"Running with {symbols.Count}/{symbolNames.Length} symbols", StrategyLoggingLevel.Trading);
            Log($"Batch logging every {LogIntervalSeconds} seconds", StrategyLoggingLevel.Trading);

            // Initialize CSV exporter if enabled
            if (EnableCSVLogging)
            {
                try
                {
                    csvExporter = new CSVDataExporter(CSVLogDirectory, currentSession, sessionStartTime);

                    // Initialize CSV files for all symbols
                    foreach (string symbolRoot in symbolNames)
                    {
                        csvExporter.InitializeSymbolFile(symbolRoot);
                    }

                    Log($"CSV logging enabled for {symbolNames.Length} symbols in: {CSVLogDirectory}", StrategyLoggingLevel.Trading);
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
                // Create historical data for indicators (1-minute bars from session start)
                HistoryRequestParameters historyParams = new()
                {
                    Symbol = symbol,
                    FromTime = sessionStartTime,
                    ToTime = Core.Instance.TimeUtils.DateTimeUtcNow,
                    Aggregation = new HistoryAggregationTime(Period.MIN1, symbol.HistoryType)
                };

                HistoricalData historicalData = symbol.GetHistory(historyParams);

                if (historicalData != null)
                {
                    historicalDataCache[symbol] = historicalData;

                    // Initialize VWAP indicator
                    SessionAnchoredVWAP vwap = new()
                    {
                        UseTypicalPrice = true,
                        ShowStdDevBands = true,
                        ShowMPDBands = true
                    };
                    historicalData.AddIndicator(vwap);
                    vwapIndicators[symbol] = vwap;

                    // Initialize VPOC indicator
                    VPOCIndicator vpoc = new();
                    historicalData.AddIndicator(vpoc);
                    vpocIndicators[symbol] = vpoc;

                    // Initialize TWAP indicator
                    TWAPIndicator twap = new()
                    {
                        UseTypicalPrice = true,
                        ShowStdDevBands = true
                    };
                    historicalData.AddIndicator(twap);
                    twapIndicators[symbol] = twap;

                    Log($"[{currentSession}] {symbol.Name} indicators initialized with {historicalData.Count} bars", StrategyLoggingLevel.Trading);
                }
                else
                {
                    Log($"[{currentSession}] {symbol.Name} - Failed to get historical data for indicators", StrategyLoggingLevel.Error);
                }
            }
            catch (Exception ex)
            {
                Log($"[{currentSession}] {symbol.Name} - Error initializing indicators: {ex.Message}", StrategyLoggingLevel.Error);
            }
        }

        private void InitializeSessionOpen(Symbol symbol)
        {
            try
            {
                // Request historical data around session start time
                HistoryRequestParameters historyParams = new()
                {
                    Symbol = symbol,
                    FromTime = sessionStartTime.AddMinutes(-5),
                    ToTime = sessionStartTime.AddMinutes(5),
                    Aggregation = new HistoryAggregationTime(Period.MIN1, symbol.HistoryType)
                };

                HistoricalData history = symbol.GetHistory(historyParams);

                if (history != null && history.Count > 0)
                {
                    // Find the first bar at or after session start
                    IHistoryItem sessionBar = null;
                    double closestTimeDiff = double.MaxValue;

                    for (int i = 0; i < history.Count; i++)
                    {
                        IHistoryItem bar = history[i, SeekOriginHistory.Begin];
                        if (bar is HistoryItemBar barItem && barItem.TimeLeft >= sessionStartTime)
                        {
                            double timeDiff = (barItem.TimeLeft - sessionStartTime).TotalSeconds;

                            if (timeDiff < closestTimeDiff)
                            {
                                sessionBar = bar;
                                closestTimeDiff = timeDiff;
                            }
                        }
                    }

                    if (sessionBar is HistoryItemBar sessionBarItem)
                    {
                        sessionOpenPrices[symbol] = sessionBarItem.Open;
                        sessionOpenInitialized[symbol] = true;
                        Log($"[{currentSession}] {symbol.Name} session open from history: {sessionBarItem.Open:F2} (bar time: {sessionBarItem.TimeLeft:yyyy-MM-dd HH:mm})", StrategyLoggingLevel.Trading);
                        return;
                    }
                }

                Log($"[{currentSession}] {symbol.Name} - No historical data found, will use first tick", StrategyLoggingLevel.Trading);
            }
            catch (Exception ex)
            {
                Log($"[{currentSession}] {symbol.Name} - Error getting historical data: {ex.Message}, will use first tick", StrategyLoggingLevel.Error);
            }
        }

        /// <summary>
        /// Dedicated event handler for NewLast - each symbol gets its own closure
        /// </summary>
        private void OnNewLast_ForSymbol(string symbolRoot, Symbol symbol, Last last)
        {
            if (symbol == null || last == null)
            {
                return;
            }

            CheckSessionBoundary(last.Time);

            // If session open hasn't been initialized yet, use this tick
            if (!sessionOpenInitialized.ContainsKey(symbol) || !sessionOpenInitialized[symbol])
            {
                if (!sessionOpenPrices.ContainsKey(symbol))
                {
                    sessionOpenPrices[symbol] = last.Price;
                    sessionOpenInitialized[symbol] = true;
                    Log($"[{currentSession}] {symbol.Name} session open from first tick: {last.Price:F2} (tick time: {last.Time:yyyy-MM-dd HH:mm:ss})", StrategyLoggingLevel.Trading);
                }
                return;
            }

            // Update current price
            currentPrices[symbol] = last.Price;

            // Update market data snapshot with trade information
            UpdateSnapshotFromLast(symbol, last);

            // Write to CSV immediately on every tick if enabled (non-blocking with symbolRoot available)
            if (EnableCSVLogging && csvExporter != null && latestSnapshots.ContainsKey(symbol))
            {
                try
                {
                    csvExporter.WriteSnapshot(
                        symbolRoot,  // Already available from closure - no lookup needed!
                        symbol,
                        latestSnapshots[symbol],
                        sessionOpenPrices.ContainsKey(symbol) ? sessionOpenPrices[symbol] : 0.0,
                        vwapIndicators.ContainsKey(symbol) ? vwapIndicators[symbol] : null,
                        vpocIndicators.ContainsKey(symbol) ? vpocIndicators[symbol] : null,
                        twapIndicators.ContainsKey(symbol) ? twapIndicators[symbol] : null);
                }
                catch (Exception ex)
                {
                    Log($"Error writing CSV for {symbolRoot}: {ex.Message}", StrategyLoggingLevel.Error);
                }
            }

            // Check if it's time to log to console
            if ((last.Time - lastLogTime).TotalSeconds >= LogIntervalSeconds)
            {
                LogAllSymbols(last.Time);
                lastLogTime = last.Time;
            }
        }

        /// <summary>
        /// Dedicated event handler for NewQuote - each symbol gets its own closure
        /// </summary>
        private void OnNewQuote_ForSymbol(string symbolRoot, Symbol symbol, Quote quote)
        {
            if (symbol == null || quote == null)
            {
                return;
            }

            // Update market data snapshot with bid/ask information
            UpdateSnapshotFromQuote(symbol, quote);
        }

        private void UpdateSnapshotFromLast(Symbol symbol, Last last)
        {
            if (!latestSnapshots.ContainsKey(symbol))
            {
                latestSnapshots[symbol] = new MarketDataSnapshot
                {
                    SymbolName = symbol.Name,
                    SessionOpen = sessionOpenPrices.ContainsKey(symbol) ? sessionOpenPrices[symbol] : 0.0
                };
            }

            MarketDataSnapshot snapshot = latestSnapshots[symbol];
            snapshot.Timestamp = last.Time;
            snapshot.Last = last.Price;
            snapshot.Size = last.Size;
            snapshot.Aggressor = last.AggressorFlag;
            snapshot.TickDirection = last.TickDirection;

            // Update volume and trade data from Symbol properties
            snapshot.Volume = symbol.Volume;
            snapshot.Trades = symbol.Trades;
            snapshot.Delta = symbol.Delta;

            // Calculate buy/sell volumes based on aggressor
            double tradeDelta = 0;
            if (last.AggressorFlag == AggressorFlag.Buy)
            {
                tradeDelta = last.Size;
                cumulativeBuyVolume[symbol] += last.Size;
            }
            else if (last.AggressorFlag == AggressorFlag.Sell)
            {
                tradeDelta = -last.Size;
                cumulativeSellVolume[symbol] += last.Size;
            }

            // Update cumulative delta
            cumulativeDelta[symbol] += tradeDelta;
            snapshot.CumulativeDelta = cumulativeDelta[symbol];

            snapshot.BuyVolume = cumulativeBuyVolume[symbol];
            snapshot.SellVolume = cumulativeSellVolume[symbol];

            // Calculate percentages
            double totalVolume = snapshot.BuyVolume + snapshot.SellVolume;
            if (totalVolume > 0)
            {
                snapshot.BuyVolumePercent = snapshot.BuyVolume / totalVolume * 100;
                snapshot.SellVolumePercent = snapshot.SellVolume / totalVolume * 100;
            }

            // Calculate imbalance
            snapshot.Imbalance = snapshot.BuyVolume - snapshot.SellVolume;
            if (totalVolume > 0)
            {
                snapshot.ImbalancePercent = snapshot.Imbalance / totalVolume * 100;
                snapshot.DeltaPercent = snapshot.Delta / totalVolume * 100;
            }

            // Track liquidity changes
            if (last.TickDirection == TickDirection.Up)
            {
                snapshot.BidsLiquidityChanges = last.Size;
                snapshot.AsksLiquidityChanges = 0;
            }
            else if (last.TickDirection == TickDirection.Down)
            {
                snapshot.BidsLiquidityChanges = 0;
                snapshot.AsksLiquidityChanges = last.Size;
            }

            // Count number of changes
            snapshot.BidsNumberOfChanges++;
            snapshot.AsksNumberOfChanges++;

            // Update last trade size
            snapshot.LastTradeSize = last.Size;

            // Update bid/ask trade sizes based on aggressor
            if (last.AggressorFlag == AggressorFlag.Buy)
            {
                snapshot.BidTradeSize = last.Size;
                snapshot.AskTradeSize = 0;
            }
            else if (last.AggressorFlag == AggressorFlag.Sell)
            {
                snapshot.BidTradeSize = 0;
                snapshot.AskTradeSize = last.Size;
            }

            // Update indicator session opens
            if (vwapIndicators.ContainsKey(symbol))
            {
                snapshot.VWAPSessionOpen = vwapIndicators[symbol].GetSessionOpen();
            }

            if (vpocIndicators.ContainsKey(symbol))
            {
                snapshot.VPOCSessionOpen = vpocIndicators[symbol].GetSessionOpen();
            }

            if (twapIndicators.ContainsKey(symbol))
            {
                snapshot.TWAPSessionOpen = twapIndicators[symbol].GetSessionOpen();
            }
        }

        private void UpdateSnapshotFromQuote(Symbol symbol, Quote quote)
        {
            if (!latestSnapshots.ContainsKey(symbol))
            {
                latestSnapshots[symbol] = new MarketDataSnapshot
                {
                    SymbolName = symbol.Name,
                    SessionOpen = sessionOpenPrices.ContainsKey(symbol) ? sessionOpenPrices[symbol] : 0.0
                };
            }

            MarketDataSnapshot snapshot = latestSnapshots[symbol];
            snapshot.Timestamp = quote.Time;
            snapshot.BidPrice = quote.Bid;
            snapshot.BidSize = quote.BidSize;
            snapshot.BidTickDirection = quote.BidTickDirection;
            snapshot.AskPrice = quote.Ask;
            snapshot.AskSize = quote.AskSize;
            snapshot.AskTickDirection = quote.AskTickDirection;
            snapshot.TimeBid = quote.Time;
            snapshot.TimeAsk = quote.Time;

            // Update cumulative bid/ask sizes
            cumulativeSizeBid[symbol] += quote.BidSize;
            cumulativeSizeAsk[symbol] += quote.AskSize;
            snapshot.CumulativeSizeBid = cumulativeSizeBid[symbol];
            snapshot.CumulativeSizeAsk = cumulativeSizeAsk[symbol];
        }

        private void LogAllSymbols(DateTime timestamp)
        {
            // Log header
            Log($"╔═══════════════════════════════════════════════════════════════════════════════╗", StrategyLoggingLevel.Trading);
            Log($"║ [{currentSession}] Market Snapshot - {timestamp:HH:mm:ss} UTC                                     ║", StrategyLoggingLevel.Trading);
            Log($"╚═══════════════════════════════════════════════════════════════════════════════╝", StrategyLoggingLevel.Trading);

            // Sort symbols by name for consistent display
            List<KeyValuePair<string, Symbol>> sortedSymbols = symbols.OrderBy(kvp => kvp.Key).ToList();

            foreach (KeyValuePair<string, Symbol> kvp in sortedSymbols)
            {
                string symbolRoot = kvp.Key;
                Symbol symbol = kvp.Value;

                if (!sessionOpenPrices.ContainsKey(symbol) || !currentPrices.ContainsKey(symbol))
                {
                    continue;
                }

                double sessionOpen = sessionOpenPrices[symbol];
                double currentPrice = currentPrices[symbol];

                if (currentPrice == 0.0)
                {
                    continue;
                }

                double changePercent = ((currentPrice / sessionOpen) - 1) * 100;
                double changePoints = currentPrice - sessionOpen;
                string direction = changePercent >= 0 ? "▲" : "▼";
                string sign = changePercent >= 0 ? "+" : "";

                // Build main price line with better formatting
                Log($"┌─ {direction} {symbolRoot,-6} ─────────────────────────────────────────────────────────────┐", StrategyLoggingLevel.Trading);
                Log($"│  Price: {currentPrice,10:F2}  │  Change: {sign}{changePercent,7:F3}% ({sign}{changePoints,8:F2} pts)", StrategyLoggingLevel.Trading);

                // Get VWAP indicator values
                if (vwapIndicators.ContainsKey(symbol))
                {
                    double vwap = vwapIndicators[symbol].GetVWAP();
                    double vwapUpper = vwapIndicators[symbol].GetUpperStdDev();
                    double vwapLower = vwapIndicators[symbol].GetLowerStdDev();
                    double vwapUpperMPD = vwapIndicators[symbol].GetUpperMPD();
                    double vwapLowerMPD = vwapIndicators[symbol].GetLowerMPD();

                    if (!double.IsNaN(vwap))
                    {
                        // Convert VWAP from percentage back to price
                        double vwapPrice = sessionOpen * (1 + vwap);
                        double vwapDiff = currentPrice - vwapPrice;
                        double vwapDiffPct = vwap;  // Already in decimal form
                        string vwapPos = vwapDiff >= 0 ? "ABOVE" : "BELOW";

                        Log($"│", StrategyLoggingLevel.Trading);
                        Log($"│  📊 VWAP Analysis:", StrategyLoggingLevel.Trading);
                        Log($"│     VWAP Price:  {vwapPrice,10:F2}  ({vwapPos} by {Math.Abs(vwapDiff),7:F2} pts / {Math.Abs(vwapDiffPct),6:F3}%)", StrategyLoggingLevel.Trading);

                        if (!double.IsNaN(vwapUpper) && !double.IsNaN(vwapLower))
                        {
                            double upperPrice = sessionOpen * (1 + vwapUpper);
                            double lowerPrice = sessionOpen * (1 + vwapLower);
                            double upperPct = vwapUpper;  // Already in decimal form
                            double lowerPct = vwapLower;  // Already in decimal form

                            Log($"│     ± 2σ Bands:  Upper {upperPrice,10:F2} ({upperPct,+7:F3}%)  │  Lower {lowerPrice,10:F2} ({lowerPct,+7:F3}%)", StrategyLoggingLevel.Trading);
                        }

                        if (!double.IsNaN(vwapUpperMPD) && !double.IsNaN(vwapLowerMPD))
                        {
                            double upperMPDPrice = sessionOpen * (1 + vwapUpperMPD);
                            double lowerMPDPrice = sessionOpen * (1 + vwapLowerMPD);
                            double upperMPDPct = vwapUpperMPD;  // Already in decimal form
                            double lowerMPDPct = vwapLowerMPD;  // Already in decimal form

                            Log($"│     MPD Bands:   Upper {upperMPDPrice,10:F2} ({upperMPDPct,+7:F3}%)  │  Lower {lowerMPDPrice,10:F2} ({lowerMPDPct,+7:F3}%)", StrategyLoggingLevel.Trading);
                        }
                    }
                }

                // Get VPOC indicator values
                if (vpocIndicators.ContainsKey(symbol))
                {
                    double vpoc = vpocIndicators[symbol].GetVPOC();
                    double vah = vpocIndicators[symbol].GetVAH();
                    double val = vpocIndicators[symbol].GetVAL();

                    if (!double.IsNaN(vpoc))
                    {
                        // Convert VPOC values from percentage back to price
                        double vpocPrice = sessionOpen * (1 + vpoc);
                        double vpocPct = vpoc;  // Already in decimal form

                        Log($"│", StrategyLoggingLevel.Trading);
                        Log($"│  📈 Volume Profile:", StrategyLoggingLevel.Trading);
                        Log($"│     VPOC:        {vpocPrice,10:F2} ({vpocPct,+7:F3}%)", StrategyLoggingLevel.Trading);

                        if (!double.IsNaN(vah) && !double.IsNaN(val))
                        {
                            double vahPrice = sessionOpen * (1 + vah);
                            double valPrice = sessionOpen * (1 + val);
                            double vahPct = vah;  // Already in decimal form
                            double valPct = val;  // Already in decimal form

                            // Determine position relative to value area
                            string vaPosition = currentPrice > vahPrice ? "🔴 ABOVE VALUE AREA" : currentPrice < valPrice ? "🔵 BELOW VALUE AREA" : "🟢 INSIDE VALUE AREA";
                            Log($"│     VAH:         {vahPrice,10:F2} ({vahPct,+7:F3}%)", StrategyLoggingLevel.Trading);
                            Log($"│     VAL:         {valPrice,10:F2} ({valPct,+7:F3}%)", StrategyLoggingLevel.Trading);
                            Log($"│     Position:    {vaPosition}", StrategyLoggingLevel.Trading);
                        }
                    }
                }

                // Get TWAP indicator values
                if (twapIndicators.ContainsKey(symbol))
                {
                    double twap = twapIndicators[symbol].GetTWAP();
                    double twapUpper = twapIndicators[symbol].GetUpperStdDev();
                    double twapLower = twapIndicators[symbol].GetLowerStdDev();

                    if (!double.IsNaN(twap))
                    {
                        // Convert TWAP from percentage back to price
                        double twapPrice = sessionOpen * (1 + twap);
                        double twapDiff = currentPrice - twapPrice;
                        double twapDiffPct = twap;  // Already in decimal form
                        string twapPos = twapDiff >= 0 ? "ABOVE" : "BELOW";

                        Log($"│", StrategyLoggingLevel.Trading);
                        Log($"│  ⏱️  TWAP Analysis:", StrategyLoggingLevel.Trading);
                        Log($"│     TWAP Price:  {twapPrice,10:F2}  ({twapPos} by {Math.Abs(twapDiff),7:F2} pts / {Math.Abs(twapDiffPct),6:F3}%)", StrategyLoggingLevel.Trading);

                        if (!double.IsNaN(twapUpper) && !double.IsNaN(twapLower))
                        {
                            double upperPrice = sessionOpen * (1 + twapUpper);
                            double lowerPrice = sessionOpen * (1 + twapLower);
                            double upperPct = twapUpper;  // Already in decimal form
                            double lowerPct = twapLower;  // Already in decimal form

                            Log($"│     ± 2σ Bands:  Upper {upperPrice,10:F2} ({upperPct,+7:F3}%)  │  Lower {lowerPrice,10:F2} ({lowerPct,+7:F3}%)", StrategyLoggingLevel.Trading);
                        }
                    }
                }

                // Optionally log orderflow features
                if (LogOrderflowFeatures)
                {
                    LogOrderflowFeaturesForSymbol(symbol);
                }

                Log($"└───────────────────────────────────────────────────────────────────────────────┘", StrategyLoggingLevel.Trading);
                Log("", StrategyLoggingLevel.Trading);
            }

            Log($"═══════════════════════════════════════════════════════════════════════════════", StrategyLoggingLevel.Trading);
        }

        /// <summary>
        /// Get the latest market data snapshot for a symbol
        /// </summary>
        public MarketDataSnapshot GetMarketDataSnapshot(Symbol symbol)
        {
            return latestSnapshots.ContainsKey(symbol) ? latestSnapshots[symbol] : null;
        }

        /// <summary>
        /// Get the latest market data snapshot for a symbol by root name
        /// </summary>
        public MarketDataSnapshot GetMarketDataSnapshot(string symbolRoot)
        {
            return symbols.ContainsKey(symbolRoot) ? GetMarketDataSnapshot(symbols[symbolRoot]) : null;
        }

        /// <summary>
        /// Log orderflow features for a specific symbol
        /// </summary>
        private void LogOrderflowFeaturesForSymbol(Symbol symbol)
        {
            if (!latestSnapshots.ContainsKey(symbol))
            {
                return;
            }

            MarketDataSnapshot snapshot = latestSnapshots[symbol];

            Log($"│  📋 Orderflow Features:", StrategyLoggingLevel.Trading);
            Log($"│     Volume:       {snapshot.Volume,10:F0}  │  Trades: {snapshot.Trades,8}", StrategyLoggingLevel.Trading);
            Log($"│     Buy Vol:      {snapshot.BuyVolume,10:F0} ({snapshot.BuyVolumePercent,5:F1}%)", StrategyLoggingLevel.Trading);
            Log($"│     Sell Vol:     {snapshot.SellVolume,10:F0} ({snapshot.SellVolumePercent,5:F1}%)", StrategyLoggingLevel.Trading);
            Log($"│     Delta:        {snapshot.Delta,10:F0}  │  Cumulative: {snapshot.CumulativeDelta,10:F0}", StrategyLoggingLevel.Trading);
            Log($"│     Imbalance:    {snapshot.Imbalance,10:F0} ({snapshot.ImbalancePercent,6:F2}%)", StrategyLoggingLevel.Trading);

            if (snapshot.BidSize > 0 || snapshot.AskSize > 0)
            {
                Log($"│     Bid:          {snapshot.BidPrice,10:F2} x {snapshot.BidSize,8:F0}", StrategyLoggingLevel.Trading);
                Log($"│     Ask:          {snapshot.AskPrice,10:F2} x {snapshot.AskSize,8:F0}", StrategyLoggingLevel.Trading);
            }

            if (snapshot.Size > 0)
            {
                string aggressorStr = snapshot.Aggressor == AggressorFlag.Buy ? "BUY" :
                                     snapshot.Aggressor == AggressorFlag.Sell ? "SELL" : "N/A";
                Log($"│     Last Trade:   {snapshot.Last,10:F2} x {snapshot.Size,8:F0} ({aggressorStr})", StrategyLoggingLevel.Trading);
            }
        }

        private TradingSession GetSession(DateTime utcTime)
        {
            TimeSpan time = utcTime.TimeOfDay;

            return time >= RTH_START && time < RTH_END
                ? TradingSession.RTH
                : time >= ETH_START || time < ETH_END ? TradingSession.ETH : TradingSession.RTH;
        }

        private DateTime GetSessionStart(DateTime utcTime, TradingSession session)
        {
            if (session == TradingSession.RTH)
            {
                DateTime rthStart = utcTime.Date.Add(RTH_START);
                return utcTime.TimeOfDay < RTH_START ? rthStart.AddDays(-1) : rthStart;
            }
            else
            {
                return utcTime.TimeOfDay < ETH_END
                    ? utcTime.Date.AddDays(-1).Add(ETH_START)
                    : utcTime.Date.Add(ETH_START);
            }
        }

        private void CheckSessionBoundary(DateTime utcTime)
        {
            TradingSession newSession = GetSession(utcTime);

            if (newSession != currentSession)
            {
                DateTime newSessionStart = GetSessionStart(utcTime, newSession);

                Log($"\n🔔 SESSION CHANGE: {currentSession} → {newSession} | Start: {newSessionStart:yyyy-MM-dd HH:mm} UTC\n", StrategyLoggingLevel.Trading);

                sessionOpenPrices.Clear();
                sessionOpenInitialized.Clear();
                currentPrices.Clear();
                latestSnapshots.Clear();

                // Reset cumulative tracking
                cumulativeDelta.Clear();
                cumulativeBuyVolume.Clear();
                cumulativeSellVolume.Clear();
                cumulativeSizeBid.Clear();
                cumulativeSizeAsk.Clear();

                currentSession = newSession;
                sessionStartTime = newSessionStart;
                lastLogTime = utcTime;

                // Re-initialize CSV exporter for new session
                if (EnableCSVLogging)
                {
                    try
                    {
                        csvExporter = new CSVDataExporter(CSVLogDirectory, newSession, newSessionStart);

                        // Initialize CSV files for all symbols
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

                    // Re-initialize cumulative tracking
                    cumulativeDelta[symbol] = 0.0;
                    cumulativeBuyVolume[symbol] = 0.0;
                    cumulativeSellVolume[symbol] = 0.0;
                    cumulativeSizeBid[symbol] = 0.0;
                    cumulativeSizeAsk[symbol] = 0.0;

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

            lastCheckTime = utcTime;
        }

        protected override void OnStop()
        {
            // Note: Lambda event handlers are automatically cleaned up when symbol is garbage collected
            // But we should still null out references for good practice
            foreach (Symbol symbol in symbols.Values)
            {
                // No need to manually unsubscribe lambdas - they'll be cleaned up
                // symbol.NewLast -= (handler);  // Can't reference the lambda directly

                // Clean up indicators
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
            cumulativeDelta.Clear();
            cumulativeBuyVolume.Clear();
            cumulativeSellVolume.Clear();
            cumulativeSizeBid.Clear();
            cumulativeSizeAsk.Clear();

            csvExporter = null;

            Log("Stopped", StrategyLoggingLevel.Trading);
        }
    }
}
