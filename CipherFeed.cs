using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using TradingPlatform.BusinessLayer;
using CipherFeed.Indicators;

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
        public double LastSize { get; set; }
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
        
        // Max Trade Volume
        public double MaxOneTradeVolume { get; set; }
        public double MaxOneTradeVolumePercent { get; set; }
        
        // Filtered Volume
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
        public double AskTradeSize { get; set; }
        
        // Time Information
        public DateTime TimeBid { get; set; }
        public DateTime TimeAsk { get; set; }
        
        // Session Open (for percentage calculations)
        public double SessionOpen { get; set; }
    }

    public class CipherFeed : Strategy
    {
        [InputParameter("Account", 1)]
        public Account CurrentAccount { get; set; }

        [InputParameter("Log Interval (seconds)", 2)]
        public int LogIntervalSeconds { get; set; } = 5;

        [InputParameter("Log Orderflow Features", 3)]
        public bool LogOrderflowFeatures { get; set; } = false;

        private readonly string[] symbolNames = { "MNQ", "MES", "M2K", "MYM", "ENQ", "EP", "RTY", "YM" };
        private readonly Dictionary<string, Symbol> symbols = new Dictionary<string, Symbol>();
        private readonly Dictionary<Symbol, double> sessionOpenPrices = new Dictionary<Symbol, double>();
        private readonly Dictionary<Symbol, bool> sessionOpenInitialized = new Dictionary<Symbol, bool>();
        
        // Current prices for batch logging
        private readonly Dictionary<Symbol, double> currentPrices = new Dictionary<Symbol, double>();
        private DateTime lastLogTime;
        
        // Market data snapshots per symbol
        private readonly Dictionary<Symbol, MarketDataSnapshot> latestSnapshots = new Dictionary<Symbol, MarketDataSnapshot>();
        
        // Cumulative tracking for orderflow features
        private readonly Dictionary<Symbol, double> cumulativeDelta = new Dictionary<Symbol, double>();
        private readonly Dictionary<Symbol, double> cumulativeBuyVolume = new Dictionary<Symbol, double>();
        private readonly Dictionary<Symbol, double> cumulativeSellVolume = new Dictionary<Symbol, double>();
        private readonly Dictionary<Symbol, double> cumulativeSizeBid = new Dictionary<Symbol, double>();
        private readonly Dictionary<Symbol, double> cumulativeSizeAsk = new Dictionary<Symbol, double>();
        
        // Indicators per symbol
        private readonly Dictionary<Symbol, HistoricalData> historicalDataCache = new Dictionary<Symbol, HistoricalData>();
        private readonly Dictionary<Symbol, SessionAnchoredVWAP> vwapIndicators = new Dictionary<Symbol, SessionAnchoredVWAP>();
        private readonly Dictionary<Symbol, VPOCIndicator> vpocIndicators = new Dictionary<Symbol, VPOCIndicator>();
        private readonly Dictionary<Symbol, TWAPIndicator> twapIndicators = new Dictionary<Symbol, TWAPIndicator>();

        // Session times: PST to UTC conversion (PST + 8 hours = UTC)
        // RTH: 4:00 AM - 1:45 PM PST = 12:00 - 21:45 UTC
        // ETH: 3:15 PM - 4:00 AM PST = 23:15 UTC - 12:00 UTC (next day)
        private static readonly TimeSpan RTH_START = new TimeSpan(12, 0, 0);   // 12:00 UTC (4am PST)
        private static readonly TimeSpan RTH_END = new TimeSpan(21, 45, 0);    // 21:45 UTC (1:45pm PST)
        private static readonly TimeSpan ETH_START = new TimeSpan(23, 15, 0);  // 23:15 UTC (3:15pm PST)
        private static readonly TimeSpan ETH_END = new TimeSpan(12, 0, 0);     // 12:00 UTC (4am PST next day)

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
                meter.CreateObservableGauge(metricName, () =>
                {
                    var symbol = symbols.ContainsKey(symbolName) ? symbols[symbolName] : null;
                    if (symbol != null && sessionOpenPrices.ContainsKey(symbol))
                    {
                        return sessionOpenPrices[symbol];
                    }
                    return 0.0;
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
                var candidates = Core.Instance.Symbols
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
                symbol.NewLast += OnNewLast;
                symbol.NewQuote += OnNewQuote;
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
        }

        private void InitializeIndicatorsForSymbol(Symbol symbol)
        {
            try
            {
                // Create historical data for indicators (1-minute bars from session start)
                var historyParams = new HistoryRequestParameters
                {
                    Symbol = symbol,
                    FromTime = sessionStartTime,
                    ToTime = Core.Instance.TimeUtils.DateTimeUtcNow,
                    Aggregation = new HistoryAggregationTime(Period.MIN1, symbol.HistoryType)
                };

                var historicalData = symbol.GetHistory(historyParams);
                
                if (historicalData != null)
                {
                    historicalDataCache[symbol] = historicalData;
                    
                    // Initialize VWAP indicator
                    var vwap = new SessionAnchoredVWAP
                    {
                        UseTypicalPrice = true,
                        ShowStdDevBands = true,
                        ShowMPDBands = true
                    };
                    historicalData.AddIndicator(vwap);
                    vwapIndicators[symbol] = vwap;
                    
                    // Initialize VPOC indicator
                    var vpoc = new VPOCIndicator();
                    historicalData.AddIndicator(vpoc);
                    vpocIndicators[symbol] = vpoc;
                    
                    // Initialize TWAP indicator
                    var twap = new TWAPIndicator
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
                var historyParams = new HistoryRequestParameters
                {
                    Symbol = symbol,
                    FromTime = sessionStartTime.AddMinutes(-5),
                    ToTime = sessionStartTime.AddMinutes(5),
                    Aggregation = new HistoryAggregationTime(Period.MIN1, symbol.HistoryType)
                };

                var history = symbol.GetHistory(historyParams);
                
                if (history != null && history.Count > 0)
                {
                    // Find the first bar at or after session start
                    IHistoryItem sessionBar = null;
                    double closestTimeDiff = double.MaxValue;

                    for (int i = 0; i < history.Count; i++)
                    {
                        var bar = history[i, SeekOriginHistory.Begin];
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

        private void OnNewLast(Symbol symbol, Last last)
        {
            if (symbol == null || last == null)
                return;

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

            // Check if it's time to log
            if ((last.Time - lastLogTime).TotalSeconds >= LogIntervalSeconds)
            {
                LogAllSymbols(last.Time);
                lastLogTime = last.Time;
            }
        }

        private void OnNewQuote(Symbol symbol, Quote quote)
        {
            if (symbol == null || quote == null)
                return;
                
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

            var snapshot = latestSnapshots[symbol];
            snapshot.Timestamp = last.Time;
            snapshot.Last = last.Price;
            snapshot.LastSize = last.Size;
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
                snapshot.BuyVolumePercent = (snapshot.BuyVolume / totalVolume) * 100;
                snapshot.SellVolumePercent = (snapshot.SellVolume / totalVolume) * 100;
            }
            
            // Calculate imbalance
            snapshot.Imbalance = snapshot.BuyVolume - snapshot.SellVolume;
            if (totalVolume > 0)
            {
                snapshot.ImbalancePercent = (snapshot.Imbalance / totalVolume) * 100;
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

            var snapshot = latestSnapshots[symbol];
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
            var sortedSymbols = symbols.OrderBy(kvp => kvp.Key).ToList();

            foreach (var kvp in sortedSymbols)
            {
                string symbolRoot = kvp.Key;
                Symbol symbol = kvp.Value;

                if (!sessionOpenPrices.ContainsKey(symbol) || !currentPrices.ContainsKey(symbol))
                    continue;

                double sessionOpen = sessionOpenPrices[symbol];
                double currentPrice = currentPrices[symbol];
                
                if (currentPrice == 0.0)
                    continue;

                double changePercent = (currentPrice / sessionOpen - 1) * 100;
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
                        double vwapDiffPct = vwap * 100;
                        string vwapPos = vwapDiff >= 0 ? "ABOVE" : "BELOW";
                        
                        Log($"│", StrategyLoggingLevel.Trading);
                        Log($"│  📊 VWAP Analysis:", StrategyLoggingLevel.Trading);
                        Log($"│     VWAP Price:  {vwapPrice,10:F2}  ({vwapPos} by {Math.Abs(vwapDiff),7:F2} pts / {Math.Abs(vwapDiffPct),6:F3}%)", StrategyLoggingLevel.Trading);
                        
                        if (!double.IsNaN(vwapUpper) && !double.IsNaN(vwapLower))
                        {
                            double upperPrice = sessionOpen * (1 + vwapUpper);
                            double lowerPrice = sessionOpen * (1 + vwapLower);
                            double upperPct = vwapUpper * 100;
                            double lowerPct = vwapLower * 100;
                            
                            Log($"│     ± 2σ Bands:  Upper {upperPrice,10:F2} ({upperPct,+7:F3}%)  │  Lower {lowerPrice,10:F2} ({lowerPct,+7:F3}%)", StrategyLoggingLevel.Trading);
                        }
                        
                        if (!double.IsNaN(vwapUpperMPD) && !double.IsNaN(vwapLowerMPD))
                        {
                            double upperMPDPrice = sessionOpen * (1 + vwapUpperMPD);
                            double lowerMPDPrice = sessionOpen * (1 + vwapLowerMPD);
                            double upperMPDPct = vwapUpperMPD * 100;
                            double lowerMPDPct = vwapLowerMPD * 100;
                            
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
                        double vpocPct = vpoc * 100;
                        
                        Log($"│", StrategyLoggingLevel.Trading);
                        Log($"│  📈 Volume Profile:", StrategyLoggingLevel.Trading);
                        Log($"│     VPOC:        {vpocPrice,10:F2} ({vpocPct,+7:F3}%)", StrategyLoggingLevel.Trading);
                        
                        if (!double.IsNaN(vah) && !double.IsNaN(val))
                        {
                            double vahPrice = sessionOpen * (1 + vah);
                            double valPrice = sessionOpen * (1 + val);
                            double vahPct = vah * 100;
                            double valPct = val * 100;
                            
                            // Determine position relative to value area
                            string vaPosition;
                            if (currentPrice > vahPrice)
                                vaPosition = "🔴 ABOVE VALUE AREA";
                            else if (currentPrice < valPrice)
                                vaPosition = "🔵 BELOW VALUE AREA";
                            else
                                vaPosition = "🟢 INSIDE VALUE AREA";
                            

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
                        double twapDiffPct = twap * 100;
                        string twapPos = twapDiff >= 0 ? "ABOVE" : "BELOW";
                        
                        Log($"│", StrategyLoggingLevel.Trading);
                        Log($"│  ⏱️  TWAP Analysis:", StrategyLoggingLevel.Trading);
                        Log($"│     TWAP Price:  {twapPrice,10:F2}  ({twapPos} by {Math.Abs(twapDiff),7:F2} pts / {Math.Abs(twapDiffPct),6:F3}%)", StrategyLoggingLevel.Trading);
                        
                        if (!double.IsNaN(twapUpper) && !double.IsNaN(twapLower))
                        {
                            double upperPrice = sessionOpen * (1 + twapUpper);
                            double lowerPrice = sessionOpen * (1 + twapLower);
                            double upperPct = twapUpper * 100;
                            double lowerPct = twapLower * 100;
                            
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
            if (symbols.ContainsKey(symbolRoot))
            {
                return GetMarketDataSnapshot(symbols[symbolRoot]);
            }
            return null;
        }

        /// <summary>
        /// Log orderflow features for a specific symbol
        /// </summary>
        private void LogOrderflowFeaturesForSymbol(Symbol symbol)
        {
            if (!latestSnapshots.ContainsKey(symbol))
                return;

            var snapshot = latestSnapshots[symbol];
            
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
            
            if (snapshot.LastSize > 0)
            {
                string aggressorStr = snapshot.Aggressor == AggressorFlag.Buy ? "BUY" :
                                     snapshot.Aggressor == AggressorFlag.Sell ? "SELL" : "N/A";
                Log($"│     Last Trade:   {snapshot.Last,10:F2} x {snapshot.LastSize,8:F0} ({aggressorStr})", StrategyLoggingLevel.Trading);
            }
        }

        private TradingSession GetSession(DateTime utcTime)
        {
            TimeSpan time = utcTime.TimeOfDay;

            if (time >= RTH_START && time < RTH_END)
                return TradingSession.RTH;

            if (time >= ETH_START || time < ETH_END)
                return TradingSession.ETH;

            return TradingSession.RTH;
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
                
                // Re-initialize session open and indicators for all symbols
                foreach (var symbol in symbols.Values)
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
                        var oldHistory = historicalDataCache[symbol];
                        if (vwapIndicators.ContainsKey(symbol))
                            oldHistory.RemoveIndicator(vwapIndicators[symbol]);
                        if (vpocIndicators.ContainsKey(symbol))
                            oldHistory.RemoveIndicator(vpocIndicators[symbol]);
                        if (twapIndicators.ContainsKey(symbol))
                            oldHistory.RemoveIndicator(twapIndicators[symbol]);
                    }
                    
                    // Re-initialize indicators for new session
                    InitializeIndicatorsForSymbol(symbol);
                }
            }

            lastCheckTime = utcTime;
        }

        protected override void OnStop()
        {
            foreach (var symbol in symbols.Values)
            {
                symbol.NewLast -= OnNewLast;
                symbol.NewQuote -= OnNewQuote;
                
                // Clean up indicators
                if (historicalDataCache.ContainsKey(symbol))
                {
                    var history = historicalDataCache[symbol];
                    if (vwapIndicators.ContainsKey(symbol))
                        history.RemoveIndicator(vwapIndicators[symbol]);
                    if (vpocIndicators.ContainsKey(symbol))
                        history.RemoveIndicator(vpocIndicators[symbol]);
                    if (twapIndicators.ContainsKey(symbol))
                        history.RemoveIndicator(twapIndicators[symbol]);
                }
            }
            
            symbols.Clear();
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
            
            Log("Stopped", StrategyLoggingLevel.Trading);
        }
    }
}
