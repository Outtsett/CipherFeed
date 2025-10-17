/*
 * FILE: CipherFeed.cs
 * PURPOSE: Main strategy orchestrator for 8 CME micro futures with parallel tick processing
 * KEY DEPENDENCIES: Core.SessionManager, Models.MarketDataSnapshot, Indicators.*
 * LAST MODIFIED: Refactored to remove duplicated logic - moved to proper class files
 */

using CipherFeed.Core;
using CipherFeed.Indicators;
using CipherFeed.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
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
        private CSVDataExporter csvExporter;

        public CipherFeed()
        {
            Name = "CipherFeed";
        }

        protected override void OnCreated()
        {
            base.OnCreated();
            
            // Initialize session manager (created once, not on restarts)
            sessionManager = new SessionManager();
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

            DateTime now = TradingPlatform.BusinessLayer.Core.Instance.TimeUtils.DateTimeUtcNow;
            
            // Initialize session manager
            sessionManager.Initialize(now);
            sessionManager.SessionChanged += OnSessionChanged;
            
            lastLogTime = now;

            Log($"Current session: {sessionManager.CurrentSession} | Start: {sessionManager.SessionStartTime:yyyy-MM-dd HH:mm} UTC", StrategyLoggingLevel.Trading);

            foreach (string symbolRoot in symbolNames)
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
                    continue;
                }

                Symbol symbol = candidates.First();
                symbols[symbolRoot] = symbol;
                symbolToRoot[symbol] = symbolRoot;

                // Create dedicated event handler for this symbol using lambda closure
                symbol.NewLast += (s, last) => OnNewLast_ForSymbol(symbolRoot, s, last);
                symbol.NewQuote += (s, quote) => OnNewQuote_ForSymbol(symbolRoot, s, quote);

                sessionOpenInitialized[symbol] = false;
                currentPrices[symbol] = 0.0;

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
                    csvExporter = new CSVDataExporter(CSVLogDirectory, sessionManager.CurrentSession, sessionManager.SessionStartTime);

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
            try
            {
                HistoryRequestParameters historyParams = new()
                {
                    Symbol = symbol,
                    FromTime = sessionManager.SessionStartTime.AddMinutes(-5),
                    ToTime = sessionManager.SessionStartTime.AddMinutes(5),
                    Aggregation = new HistoryAggregationTime(Period.MIN1, symbol.HistoryType)
                };

                HistoricalData history = symbol.GetHistory(historyParams);

                if (history != null && history.Count > 0)
                {
                    IHistoryItem sessionBar = null;
                    double closestTimeDiff = double.MaxValue;

                    for (int i = 0; i < history.Count; i++)
                    {
                        IHistoryItem bar = history[i, SeekOriginHistory.Begin];
                        if (bar is HistoryItemBar barItem && barItem.TimeLeft >= sessionManager.SessionStartTime)
                        {
                            double timeDiff = (barItem.TimeLeft - sessionManager.SessionStartTime).TotalSeconds;

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
                        Log($"[{sessionManager.CurrentSession}] {symbol.Name} session open from history: {sessionBarItem.Open:F2} (bar time: {sessionBarItem.TimeLeft:yyyy-MM-dd HH:mm})", StrategyLoggingLevel.Trading);
                        return;
                    }
                }

                Log($"[{sessionManager.CurrentSession}] {symbol.Name} - No historical data found, will use first tick", StrategyLoggingLevel.Trading);
            }
            catch (Exception ex)
            {
                Log($"[{sessionManager.CurrentSession}] {symbol.Name} - Error getting historical data: {ex.Message}, will use first tick", StrategyLoggingLevel.Error);
            }
        }

        private void OnNewLast_ForSymbol(string symbolRoot, Symbol symbol, Last last)
        {
            if (symbol == null || last == null)
            {
                return;
            }

            // Check session boundary using SessionManager
            sessionManager.CheckBoundary(last.Time);

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

        private void OnNewQuote_ForSymbol(string symbolRoot, Symbol symbol, Quote quote)
        {
            if (symbol == null || quote == null)
            {
                return;
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

        private void LogAllSymbols(DateTime timestamp)
        {
            Log($"╔═══════════════════════════════════════════════════════════════════════════════╗", StrategyLoggingLevel.Trading);
            Log($"║ [{sessionManager.CurrentSession}] Market Snapshot - {timestamp:HH:mm:ss} UTC                                     ║", StrategyLoggingLevel.Trading);
            Log($"╚═══════════════════════════════════════════════════════════════════════════════╝", StrategyLoggingLevel.Trading);

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
                        double vwapPrice = sessionOpen * (1 + vwap);
                        double vwapDiff = currentPrice - vwapPrice;
                        double vwapDiffPct = vwap;
                        string vwapPos = vwapDiff >= 0 ? "ABOVE" : "BELOW";

                        Log($"│", StrategyLoggingLevel.Trading);
                        Log($"│  📊 VWAP Analysis:", StrategyLoggingLevel.Trading);
                        Log($"│     VWAP Price:  {vwapPrice,10:F2}  ({vwapPos} by {Math.Abs(vwapDiff),7:F2} pts / {Math.Abs(vwapDiffPct),6:F3}%)", StrategyLoggingLevel.Trading);

                        if (!double.IsNaN(vwapUpper) && !double.IsNaN(vwapLower))
                        {
                            double upperPrice = sessionOpen * (1 + vwapUpper);
                            double lowerPrice = sessionOpen * (1 + vwapLower);
                            double upperPct = vwapUpper;
                            double lowerPct = vwapLower;

                            Log($"│     ± 2σ Bands:  Upper {upperPrice,10:F2} ({upperPct,+7:F3}%)  │  Lower {lowerPrice,10:F2} ({lowerPct,+7:F3}%)", StrategyLoggingLevel.Trading);
                        }

                        if (!double.IsNaN(vwapUpperMPD) && !double.IsNaN(vwapLowerMPD))
                        {
                            double upperMPDPrice = sessionOpen * (1 + vwapUpperMPD);
                            double lowerMPDPrice = sessionOpen * (1 + vwapLowerMPD);
                            double upperMPDPct = vwapUpperMPD;
                            double lowerMPDPct = vwapLowerMPD;

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
                        double vpocPrice = sessionOpen * (1 + vpoc);
                        double vpocPct = vpoc;

                        Log($"│", StrategyLoggingLevel.Trading);
                        Log($"│  📈 Volume Profile:", StrategyLoggingLevel.Trading);
                        Log($"│     VPOC:        {vpocPrice,10:F2} ({vpocPct,+7:F3}%)", StrategyLoggingLevel.Trading);

                        if (!double.IsNaN(vah) && !double.IsNaN(val))
                        {
                            double vahPrice = sessionOpen * (1 + vah);
                            double valPrice = sessionOpen * (1 + val);
                            double vahPct = vah;
                            double valPct = val;

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
                        double twapPrice = sessionOpen * (1 + twap);
                        double twapDiff = currentPrice - twapPrice;
                        double twapDiffPct = twap;
                        string twapPos = twapDiff >= 0 ? "ABOVE" : "BELOW";

                        Log($"│", StrategyLoggingLevel.Trading);
                        Log($"│  ⏱️  TWAP Analysis:", StrategyLoggingLevel.Trading);
                        Log($"│     TWAP Price:  {twapPrice,10:F2}  ({twapPos} by {Math.Abs(twapDiff),7:F2} pts / {Math.Abs(twapDiffPct),6:F3}%)", StrategyLoggingLevel.Trading);

                        if (!double.IsNaN(twapUpper) && !double.IsNaN(twapLower))
                        {
                            double upperPrice = sessionOpen * (1 + twapUpper);
                            double lowerPrice = sessionOpen * (1 + twapLower);
                            double upperPct = twapUpper;
                            double lowerPct = twapLower;

                            Log($"│     ± 2σ Bands:  Upper {upperPrice,10:F2} ({upperPct,+7:F3}%)  │  Lower {lowerPrice,10:F2} ({lowerPct,+7:F3}%)", StrategyLoggingLevel.Trading);
                        }
                    }
                }

                if (LogOrderflowFeatures)
                {
                    LogOrderflowFeaturesForSymbol(symbol);
                }

                Log($"└───────────────────────────────────────────────────────────────────────────────┘", StrategyLoggingLevel.Trading);
                Log("", StrategyLoggingLevel.Trading);
            }

            Log($"═══════════════════════════════════════════════════════════════════════════════", StrategyLoggingLevel.Trading);
        }

        public MarketDataSnapshot GetMarketDataSnapshot(Symbol symbol)
        {
            return latestSnapshots.ContainsKey(symbol) ? latestSnapshots[symbol] : null;
        }

        public MarketDataSnapshot GetMarketDataSnapshot(string symbolRoot)
        {
            return symbols.ContainsKey(symbolRoot) ? GetMarketDataSnapshot(symbols[symbolRoot]) : null;
        }

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

            sessionManager = null;
            csvExporter = null;

            Log("Stopped", StrategyLoggingLevel.Trading);
        }
    }
}
