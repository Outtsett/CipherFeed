using System;
using System.Collections.Generic;
using System.Linq;
using TradingPlatform.BusinessLayer;

namespace CipherFeed
{
    public class CipherFeed : Strategy
    {
        [InputParameter("Account", 1)]
        public Account CurrentAccount { get; set; }

        private readonly string[] symbolNames = { "MNQ", "MES", "M2K", "MYM", "ENQ", "EP", "RTY", "YM" };
        private readonly Dictionary<string, Symbol> symbols = new Dictionary<string, Symbol>();

        public CipherFeed()
        {
            Name = "CipherFeed";
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

            foreach (string symbolRoot in symbolNames)
            {
                // Find all contracts for this symbol root
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

                // Get front month (earliest expiring contract)
                Symbol symbol = candidates.First();

                symbols[symbolRoot] = symbol;
                symbol.NewLast += OnNewLast;
                
                Log($"✓ Subscribed to {symbol.Name} (Expires: {symbol.ExpirationDate:yyyy-MM-dd})", StrategyLoggingLevel.Trading);
            }

            if (symbols.Count == 0)
            {
                Log("Failed to subscribe to any symbols", StrategyLoggingLevel.Error);
                Stop();
                return;
            }

            Log($"Running with {symbols.Count}/{symbolNames.Length} symbols", StrategyLoggingLevel.Trading);
        }

        private void OnNewLast(Symbol symbol, Last last)
        {
            if (symbol == null || last == null)
                return;

            Log($"{symbol.Name}: {last.Price}", StrategyLoggingLevel.Info);
        }

        protected override void OnStop()
        {
            foreach (var symbol in symbols.Values)
            {
                symbol.NewLast -= OnNewLast;
            }
            symbols.Clear();
            Log("Stopped", StrategyLoggingLevel.Trading);
        }
    }
}
