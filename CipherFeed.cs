using System;
using System.Collections.Generic;
using System.Linq;
using TradingPlatform.BusinessLayer;

namespace CipherFeed
{
    /// <summary>
    /// Tracks active positions and their entry details
    /// </summary>
    internal class PositionTracker
    {
        public Position Position { get; set; }
        public double EntryPrice { get; set; }
        public DateTime EntryTime { get; set; }
        public string EntryStrategy { get; set; }
        public int ConfidenceScore { get; set; }
    }

    public class CipherFeed : Strategy
    {
        #region Input Parameters

        [InputParameter("Account", 1)]
        public Account CurrentAccount { get; set; }

        [InputParameter("Order Quantity", 2)]
        public double OrderQuantity { get; set; } = 1;

        [InputParameter("Enable Auto Trading", 10)]
        public bool EnableAutoTrading { get; set; } = false;

        [InputParameter("Min Confidence Score", 11)]
        public int MinConfidenceScore { get; set; } = 65;

        [InputParameter("Max Positions Per Symbol", 12)]
        public int MaxPositionsPerSymbol { get; set; } = 1;

        #endregion

        #region Private Fields

        // Dictionary to store subscribed symbols by name
        private readonly Dictionary<string, Symbol> symbols = [];

        // Micro futures symbols to subscribe to
        private readonly string[] symbolNames = { "MNQ", "MES", "M2K", "MYM", "ENQ", "EP", "RTY", "YM" };

        // Position tracking
        private readonly Dictionary<string, PositionTracker> activePositions = [];

        #endregion

        #region Constructor

        public CipherFeed() : base()
        {
            Name = "CipherFeed";
            Description = "Multi-symbol micro futures strategy";
        }

        #endregion

        #region Strategy Lifecycle

        protected override void OnRun()
        {
            // Subscribe to symbols
            int successCount = 0;
            int failCount = 0;

            foreach (string symbolName in symbolNames)
            {
                try
                {
                    Symbol symbol = Core.Instance.GetSymbol(new GetSymbolRequestParameters { SymbolId = symbolName }, CurrentAccount.ConnectionId);

                    if (symbol == null)
                    {
                        Log($"Symbol {symbolName} not found on connection {CurrentAccount.ConnectionId}", StrategyLoggingLevel.Error);
                        failCount++;
                        continue;
                    }

                    if (!symbol.IsTradingAllowed(CurrentAccount))
                    {
                        Log($"Trading not allowed for {symbolName} on account {CurrentAccount.Name}", StrategyLoggingLevel.Error);
                        failCount++;
                        continue;
                    }

                    symbols[symbolName] = symbol;

                    symbol.NewQuote += Symbol_OnNewQuote;
                    symbol.NewLast += Symbol_OnNewLast;
                    symbol.NewLevel2 += Symbol_OnNewLevel2;

                    successCount++;
                    Log($"? Subscribed to {symbolName} (ID: {symbol.Id})", StrategyLoggingLevel.Trading);
                }
                catch (Exception ex)
                {
                    Log($"Exception subscribing to {symbolName}: {ex.Message}", StrategyLoggingLevel.Error);
                    failCount++;
                }
            }

            Log($"Symbol subscription complete: {successCount} successful, {failCount} failed out of {symbolNames.Length} total",
                StrategyLoggingLevel.Trading);

            if (symbols.Count == 0)
            {
                Log("No symbols were successfully subscribed. Strategy cannot run.", StrategyLoggingLevel.Error);
                return;
            }

            // Subscribe to Core trading events
            Core.PositionAdded += Core_PositionAdded;
            Core.PositionRemoved += Core_PositionRemoved;
            Core.OrdersHistoryAdded += Core_OrdersHistoryAdded;

            Log($"Strategy started successfully | Account: {CurrentAccount.Name} | Symbols: {symbols.Count}/{symbolNames.Length}",
                StrategyLoggingLevel.Trading);
        }

        protected override void OnStop()
        {
            // Unsubscribe from symbols
            foreach (KeyValuePair<string, Symbol> symbolPair in symbols)
            {
                Symbol symbol = symbolPair.Value;
                if (symbol != null)
                {
                    symbol.NewQuote -= Symbol_OnNewQuote;
                    symbol.NewLast -= Symbol_OnNewLast;
                    symbol.NewLevel2 -= Symbol_OnNewLevel2;
                }
            }

            symbols.Clear();

            // Unsubscribe from Core trading events
            Core.PositionAdded -= Core_PositionAdded;
            Core.PositionRemoved -= Core_PositionRemoved;
            Core.OrdersHistoryAdded -= Core_OrdersHistoryAdded;

            Log("Strategy stopped and all subscriptions cleaned up", StrategyLoggingLevel.Trading);

            base.OnStop();
        }

        protected override void OnRemove()
        {
            if (symbols.Count > 0)
            {
                // Unsubscribe from symbols
                foreach (KeyValuePair<string, Symbol> symbolPair in symbols)
                {
                    Symbol symbol = symbolPair.Value;
                    if (symbol != null)
                    {
                        symbol.NewQuote -= Symbol_OnNewQuote;
                        symbol.NewLast -= Symbol_OnNewLast;
                        symbol.NewLevel2 -= Symbol_OnNewLevel2;
                    }
                }

                symbols.Clear();
            }

            CurrentAccount = null;

            base.OnRemove();
        }

        protected override void OnInitializeMetrics(System.Diagnostics.Metrics.Meter meter)
        {
            base.OnInitializeMetrics(meter);

            // Connection & Subscription
            _ = meter.CreateObservableGauge("symbols-subscribed",
                () => symbols.Count,
                description: "Number of symbols currently subscribed");

            // Position Metrics
            _ = meter.CreateObservableGauge("active-positions",
                () => activePositions.Count,
                description: "Number of currently open positions");
        }

        #endregion

        #region Market Data Event Handlers

        private void Symbol_OnNewQuote(Symbol symbol, Quote quote)
        {
            if (symbol == null || quote == null)
            {
                return;
            }

            try
            {
                // TODO: Process quote data
            }
            catch (Exception ex)
            {
                Log($"[{symbol.Name}] Error in OnNewQuote: {ex.Message}", StrategyLoggingLevel.Error);
            }
        }

        private void Symbol_OnNewLast(Symbol symbol, Last last)
        {
            if (symbol == null || last == null)
            {
                return;
            }

            try
            {
                // TODO: Process trade data
            }
            catch (Exception ex)
            {
                Log($"[{symbol.Name}] Error in OnNewLast: {ex.Message}", StrategyLoggingLevel.Error);
            }
        }

        private void Symbol_OnNewLevel2(Symbol symbol, Level2Quote level2, DOMQuote dom)
        {
            if (symbol == null || dom == null)
            {
                return;
            }

            try
            {
                // TODO: Process DOM data
            }
            catch (Exception ex)
            {
                Log($"[{symbol.Name}] Error in OnNewLevel2: {ex.Message}", StrategyLoggingLevel.Error);
            }
        }

        #endregion

        #region Core Trading Events

        private void Core_PositionAdded(Position obj)
        {
            if (obj == null || obj.Account != CurrentAccount)
            {
                return;
            }

            try
            {
                string symbolName = symbols.FirstOrDefault(x => x.Value == obj.Symbol).Key;

                if (!string.IsNullOrEmpty(symbolName))
                {
                    activePositions[symbolName] = new PositionTracker
                    {
                        Position = obj,
                        EntryPrice = obj.OpenPrice,
                        EntryTime = Core.Instance.TimeUtils.DateTimeUtcNow,
                        EntryStrategy = "Manual",
                        ConfidenceScore = 0
                    };
                }

                Log($"[{obj.Symbol.Name}] Position opened - Side: {obj.Side}, Qty: {obj.Quantity}, Price: {obj.OpenPrice:F2}",
                    StrategyLoggingLevel.Trading);
            }
            catch (Exception ex)
            {
                Log($"Error in Core_PositionAdded: {ex.Message}", StrategyLoggingLevel.Error);
            }
        }

        private void Core_PositionRemoved(Position obj)
        {
            if (obj == null || obj.Account != CurrentAccount)
            {
                return;
            }

            try
            {
                string symbolName = symbols.FirstOrDefault(x => x.Value == obj.Symbol).Key;

                if (!string.IsNullOrEmpty(symbolName) && activePositions.ContainsKey(symbolName))
                {
                    _ = activePositions.Remove(symbolName);
                    Log($"[{obj.Symbol.Name}] Position removed from tracking", StrategyLoggingLevel.Trading);
                }
            }
            catch (Exception ex)
            {
                Log($"Error in Core_PositionRemoved: {ex.Message}", StrategyLoggingLevel.Error);
            }
        }

        private void Core_OrdersHistoryAdded(OrderHistory obj)
        {
            if (obj == null || obj.Account != CurrentAccount)
            {
                return;
            }

            try
            {
                if (obj.Status == OrderStatus.Refused)
                {
                    Log($"[{obj.Symbol.Name}] Order REFUSED - Side: {obj.Side}, Type: {obj.OrderType}",
                        StrategyLoggingLevel.Error);
                }
            }
            catch (Exception ex)
            {
                Log($"Error in Core_OrdersHistoryAdded: {ex.Message}", StrategyLoggingLevel.Error);
            }
        }

        #endregion
    }
}
