using CipherFeed.Indicators;
using CipherFeed.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using TradingPlatform.BusinessLayer;

namespace CipherFeed.Core
{
    /// <summary>
    /// Handles formatted console logging for symbol market data and indicators
    /// </summary>
    public class SymbolLogger
    {
        private readonly Action<string, StrategyLoggingLevel> logAction;

        public SymbolLogger(Action<string, StrategyLoggingLevel> logAction)
        {
            this.logAction = logAction;
        }

        /// <summary>
        /// Log all subscribed symbols with current prices, indicators, and optional orderflow.
        /// Call this method at regular intervals (e.g., every 5 seconds) for batch logging.
        /// </summary>
        /// <param name="session">Current trading session (RTH/ETH)</param>
        /// <param name="timestamp">Current timestamp for header</param>
        /// <param name="symbols">Dictionary of symbol root names to Symbol objects</param>
        /// <param name="sessionOpenPrices">Dictionary of session open prices per symbol</param>
        /// <param name="currentPrices">Dictionary of current prices per symbol</param>
        /// <param name="vwapIndicators">Dictionary of VWAP indicators per symbol</param>
        /// <param name="vpocIndicators">Dictionary of VPOC indicators per symbol</param>
        /// <param name="twapIndicators">Dictionary of TWAP indicators per symbol</param>
        /// <param name="latestSnapshots">Dictionary of market data snapshots per symbol (optional)</param>
        /// <param name="logOrderflowFeatures">Whether to log detailed orderflow features</param>
        public void LogAllSymbols(
            TradingSession session,
            DateTime timestamp,
            Dictionary<string, Symbol> symbols,
            Dictionary<Symbol, double> sessionOpenPrices,
            Dictionary<Symbol, double> currentPrices,
            Dictionary<Symbol, SessionAnchoredVWAP> vwapIndicators,
            Dictionary<Symbol, VPOCIndicator> vpocIndicators,
            Dictionary<Symbol, TWAPIndicator> twapIndicators,
            Dictionary<Symbol, MarketDataSnapshot> latestSnapshots = null,
            bool logOrderflowFeatures = false)
        {
            // Log header
            Log($"?????????????????????????????????????????????????????????????????????????????????");
            Log($"? [{session}] Market Snapshot - {timestamp:HH:mm:ss} UTC                                     ?");
            Log($"?????????????????????????????????????????????????????????????????????????????????");

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
                string direction = changePercent >= 0 ? "?" : "?";
                string sign = changePercent >= 0 ? "+" : "";

                // Log symbol header
                Log($"?? {direction} {symbolRoot,-6} ??????????????????????????????????????????????????????????????");
                Log($"?  Price: {currentPrice,10:F2}  ?  Change: {sign}{changePercent,7:F3}% ({sign}{changePoints,8:F2} pts)");

                // Log VWAP indicator
                if (vwapIndicators.ContainsKey(symbol))
                {
                    LogVWAPCompact(vwapIndicators[symbol], sessionOpen, currentPrice);
                }

                // Log VPOC indicator
                if (vpocIndicators.ContainsKey(symbol))
                {
                    LogVPOCCompact(vpocIndicators[symbol], sessionOpen, currentPrice);
                }

                // Log TWAP indicator
                if (twapIndicators.ContainsKey(symbol))
                {
                    LogTWAPCompact(twapIndicators[symbol], sessionOpen, currentPrice);
                }

                // Optionally log orderflow features
                if (logOrderflowFeatures && latestSnapshots != null && latestSnapshots.ContainsKey(symbol))
                {
                    LogOrderflowFeaturesCompact(latestSnapshots[symbol]);
                }

                Log($"?????????????????????????????????????????????????????????????????????????????????");
                Log("");
            }

            Log($"???????????????????????????????????????????????????????????????????????????????");
        }

        /// <summary>
        /// Log VWAP analysis in compact box format
        /// </summary>
        private void LogVWAPCompact(SessionAnchoredVWAP vwap, double sessionOpen, double currentPrice)
        {
            if (vwap == null)
            {
                return;
            }

            double vwapValue = vwap.GetVWAP();
            double vwapUpper = vwap.GetUpperStdDev();
            double vwapLower = vwap.GetLowerStdDev();
            double vwapUpperMPD = vwap.GetUpperMPD();
            double vwapLowerMPD = vwap.GetLowerMPD();

            if (!double.IsNaN(vwapValue))
            {
                double vwapPrice = sessionOpen * (1 + vwapValue);
                double vwapDiff = currentPrice - vwapPrice;
                double vwapDiffPct = vwapValue;
                string vwapPos = vwapDiff >= 0 ? "ABOVE" : "BELOW";

                Log($"?");
                Log($"?  ?? VWAP Analysis:");
                Log($"?     VWAP Price:  {vwapPrice,10:F2}  ({vwapPos} by {Math.Abs(vwapDiff),7:F2} pts / {Math.Abs(vwapDiffPct),6:F3}%)");

                if (!double.IsNaN(vwapUpper) && !double.IsNaN(vwapLower))
                {
                    double upperPrice = sessionOpen * (1 + vwapUpper);
                    double lowerPrice = sessionOpen * (1 + vwapLower);
                    double upperPct = vwapUpper;
                    double lowerPct = vwapLower;

                    Log($"?     ± 2? Bands:  Upper {upperPrice,10:F2} ({upperPct,+7:F3}%)  ?  Lower {lowerPrice,10:F2} ({lowerPct,+7:F3}%)");
                }

                if (!double.IsNaN(vwapUpperMPD) && !double.IsNaN(vwapLowerMPD))
                {
                    double upperMPDPrice = sessionOpen * (1 + vwapUpperMPD);
                    double lowerMPDPrice = sessionOpen * (1 + vwapLowerMPD);
                    double upperMPDPct = vwapUpperMPD;
                    double lowerMPDPct = vwapLowerMPD;

                    Log($"?     MPD Bands:   Upper {upperMPDPrice,10:F2} ({upperMPDPct,+7:F3}%)  ?  Lower {lowerMPDPrice,10:F2} ({lowerMPDPct,+7:F3}%)");
                }
            }
        }

        /// <summary>
        /// Log VPOC analysis in compact box format
        /// </summary>
        private void LogVPOCCompact(VPOCIndicator vpoc, double sessionOpen, double currentPrice)
        {
            if (vpoc == null)
            {
                return;
            }

            double vpocValue = vpoc.GetVPOC();
            double vah = vpoc.GetVAH();
            double val = vpoc.GetVAL();

            if (!double.IsNaN(vpocValue))
            {
                double vpocPrice = sessionOpen * (1 + vpocValue);
                double vpocPct = vpocValue;

                Log($"?");
                Log($"?  ?? Volume Profile:");
                Log($"?     VPOC:        {vpocPrice,10:F2} ({vpocPct,+7:F3}%)");

                if (!double.IsNaN(vah) && !double.IsNaN(val))
                {
                    double vahPrice = sessionOpen * (1 + vah);
                    double valPrice = sessionOpen * (1 + val);
                    double vahPct = vah;
                    double valPct = val;

                    string vaPosition = currentPrice > vahPrice ? "?? ABOVE VALUE AREA" : currentPrice < valPrice ? "?? BELOW VALUE AREA" : "?? INSIDE VALUE AREA";
                    Log($"?     VAH:         {vahPrice,10:F2} ({vahPct,+7:F3}%)");
                    Log($"?     VAL:         {valPrice,10:F2} ({valPct,+7:F3}%)");
                    Log($"?     Position:    {vaPosition}");
                }
            }
        }

        /// <summary>
        /// Log TWAP analysis in compact box format
        /// </summary>
        private void LogTWAPCompact(TWAPIndicator twap, double sessionOpen, double currentPrice)
        {
            if (twap == null)
            {
                return;
            }

            double twapValue = twap.GetTWAP();
            double twapUpper = twap.GetUpperStdDev();
            double twapLower = twap.GetLowerStdDev();

            if (!double.IsNaN(twapValue))
            {
                double twapPrice = sessionOpen * (1 + twapValue);
                double twapDiff = currentPrice - twapPrice;
                double twapDiffPct = twapValue;
                string twapPos = twapDiff >= 0 ? "ABOVE" : "BELOW";

                Log($"?");
                Log($"?  ??  TWAP Analysis:");
                Log($"?     TWAP Price:  {twapPrice,10:F2}  ({twapPos} by {Math.Abs(twapDiff),7:F2} pts / {Math.Abs(twapDiffPct),6:F3}%)");

                if (!double.IsNaN(twapUpper) && !double.IsNaN(twapLower))
                {
                    double upperPrice = sessionOpen * (1 + twapUpper);
                    double lowerPrice = sessionOpen * (1 + twapLower);
                    double upperPct = twapUpper;
                    double lowerPct = twapLower;

                    Log($"?     ± 2? Bands:  Upper {upperPrice,10:F2} ({upperPct,+7:F3}%)  ?  Lower {lowerPrice,10:F2} ({lowerPct,+7:F3}%)");
                }
            }
        }

        /// <summary>
        /// Log orderflow features in compact box format
        /// </summary>
        private void LogOrderflowFeaturesCompact(MarketDataSnapshot snapshot)
        {
            if (snapshot == null)
            {
                return;
            }

            Log($"?  ?? Orderflow Features:");
            Log($"?     Volume:       {snapshot.Volume,10:F0}  ?  Trades: {snapshot.Trades,8}");
            Log($"?     Buy Vol:      {snapshot.BuyVolume,10:F0} ({snapshot.BuyVolumePercent,5:F1}%)");
            Log($"?     Sell Vol:     {snapshot.SellVolume,10:F0} ({snapshot.SellVolumePercent,5:F1}%)");
            Log($"?     Delta:        {snapshot.Delta,10:F0}  ?  Cumulative: {snapshot.CumulativeDelta,10:F0}");
            Log($"?     Imbalance:    {snapshot.Imbalance,10:F0} ({snapshot.ImbalancePercent,6:F2}%)");

            if (snapshot.BidSize > 0 || snapshot.AskSize > 0)
            {
                Log($"?     Bid:          {snapshot.BidPrice,10:F2} x {snapshot.BidSize,8:F0}");
                Log($"?     Ask:          {snapshot.AskPrice,10:F2} x {snapshot.AskSize,8:F0}");
            }

            if (snapshot.Size > 0)
            {
                string aggressorStr = snapshot.Aggressor == AggressorFlag.Buy ? "BUY" :
                                     snapshot.Aggressor == AggressorFlag.Sell ? "SELL" : "N/A";
                Log($"?     Last Trade:   {snapshot.Last,10:F2} x {snapshot.Size,8:F0} ({aggressorStr})");
            }
        }

        /// <summary>
        /// Log header for market snapshot
        /// </summary>
        public void LogHeader(TradingSession session, DateTime timestamp)
        {
            Log("================================================================================");
            Log($"  [{session}] Market Snapshot - {timestamp:HH:mm:ss} UTC");
            Log("================================================================================");
        }

        /// <summary>
        /// Log footer for market snapshot
        /// </summary>
        public void LogFooter()
        {
            Log("================================================================================");
        }

        /// <summary>
        /// Log symbol price header with change information
        /// </summary>
        public void LogSymbolHeader(string symbolRoot, double currentPrice, double sessionOpen)
        {
            double changePercent = ((currentPrice / sessionOpen) - 1) * 100;
            double changePoints = currentPrice - sessionOpen;
            string direction = changePercent >= 0 ? "UP" : "DN";
            string sign = changePercent >= 0 ? "+" : "";

            Log($"--- {symbolRoot} ({direction}) -----------------------------------------------------------");
            Log($"  Price: {currentPrice,10:F2}  |  Change: {sign}{changePercent,7:F3}% ({sign}{changePoints,8:F2} pts)");
        }

        /// <summary>
        /// Log symbol footer
        /// </summary>
        public void LogSymbolFooter()
        {
            Log("--------------------------------------------------------------------------------");
            Log("");
        }

        /// <summary>
        /// Log VWAP analysis
        /// </summary>
        public void LogVWAP(SessionAnchoredVWAP vwap, double sessionOpen, double currentPrice)
        {
            if (vwap == null)
            {
                return;
            }

            double vwapValue = vwap.GetVWAP();
            if (double.IsNaN(vwapValue))
            {
                return;
            }

            double vwapPrice = sessionOpen * (1 + vwapValue);
            double vwapDiff = currentPrice - vwapPrice;
            string vwapPos = vwapDiff >= 0 ? "ABOVE" : "BELOW";

            Log("");
            Log("  VWAP Analysis:");
            Log($"    VWAP Price:  {vwapPrice,10:F2}  ({vwapPos} by {Math.Abs(vwapDiff),7:F2} pts / {Math.Abs(vwapValue),6:F3}%)");

            // Standard deviation bands
            double vwapUpper = vwap.GetUpperStdDev();
            double vwapLower = vwap.GetLowerStdDev();
            if (!double.IsNaN(vwapUpper) && !double.IsNaN(vwapLower))
            {
                double upperPrice = sessionOpen * (1 + vwapUpper);
                double lowerPrice = sessionOpen * (1 + vwapLower);
                Log($"    +/- 2SD:     Upper {upperPrice,10:F2} ({vwapUpper,+7:F3}%)  |  Lower {lowerPrice,10:F2} ({vwapLower,+7:F3}%)");
            }

            // MPD bands
            double vwapUpperMPD = vwap.GetUpperMPD();
            double vwapLowerMPD = vwap.GetLowerMPD();
            if (!double.IsNaN(vwapUpperMPD) && !double.IsNaN(vwapLowerMPD))
            {
                double upperMPDPrice = sessionOpen * (1 + vwapUpperMPD);
                double lowerMPDPrice = sessionOpen * (1 + vwapLowerMPD);
                Log($"    MPD Bands:   Upper {upperMPDPrice,10:F2} ({vwapUpperMPD,+7:F3}%)  |  Lower {lowerMPDPrice,10:F2} ({vwapLowerMPD,+7:F3}%)");
            }
        }

        /// <summary>
        /// Log VPOC analysis
        /// </summary>
        public void LogVPOC(VPOCIndicator vpoc, double sessionOpen, double currentPrice)
        {
            if (vpoc == null)
            {
                return;
            }

            double vpocValue = vpoc.GetVPOC();
            if (double.IsNaN(vpocValue))
            {
                return;
            }

            double vpocPrice = sessionOpen * (1 + vpocValue);

            Log("");
            Log("  Volume Profile:");
            Log($"    VPOC:        {vpocPrice,10:F2} ({vpocValue,+7:F3}%)");

            // Value area
            double vah = vpoc.GetVAH();
            double val = vpoc.GetVAL();
            if (!double.IsNaN(vah) && !double.IsNaN(val))
            {
                double vahPrice = sessionOpen * (1 + vah);
                double valPrice = sessionOpen * (1 + val);

                string vaPosition = currentPrice > vahPrice
                    ? "ABOVE VALUE AREA"
                    : currentPrice < valPrice
                        ? "BELOW VALUE AREA"
                        : "INSIDE VALUE AREA";

                Log($"    VAH:         {vahPrice,10:F2} ({vah,+7:F3}%)");
                Log($"    VAL:         {valPrice,10:F2} ({val,+7:F3}%)");
                Log($"    Position:    {vaPosition}");
            }
        }

        /// <summary>
        /// Log TWAP analysis
        /// </summary>
        public void LogTWAP(TWAPIndicator twap, double sessionOpen, double currentPrice)
        {
            if (twap == null)
            {
                return;
            }

            double twapValue = twap.GetTWAP();
            if (double.IsNaN(twapValue))
            {
                return;
            }

            double twapPrice = sessionOpen * (1 + twapValue);
            double twapDiff = currentPrice - twapPrice;
            string twapPos = twapDiff >= 0 ? "ABOVE" : "BELOW";

            Log("");
            Log("  TWAP Analysis:");
            Log($"    TWAP Price:  {twapPrice,10:F2}  ({twapPos} by {Math.Abs(twapDiff),7:F2} pts / {Math.Abs(twapValue),6:F3}%)");

            // Standard deviation bands
            double twapUpper = twap.GetUpperStdDev();
            double twapLower = twap.GetLowerStdDev();
            if (!double.IsNaN(twapUpper) && !double.IsNaN(twapLower))
            {
                double upperPrice = sessionOpen * (1 + twapUpper);
                double lowerPrice = sessionOpen * (1 + twapLower);
                Log($"    +/- 2SD:     Upper {upperPrice,10:F2} ({twapUpper,+7:F3}%)  |  Lower {lowerPrice,10:F2} ({twapLower,+7:F3}%)");
            }
        }

        /// <summary>
        /// Log orderflow features from snapshot
        /// </summary>
        public void LogOrderflowFeatures(MarketDataSnapshot snapshot)
        {
            if (snapshot == null)
            {
                return;
            }

            Log("  Orderflow Features:");
            Log($"    Volume:       {snapshot.Volume,10:F0}  |  Trades: {snapshot.Trades,8}");
            Log($"    Buy Vol:      {snapshot.BuyVolume,10:F0} ({snapshot.BuyVolumePercent,5:F1}%)");
            Log($"    Sell Vol:     {snapshot.SellVolume,10:F0} ({snapshot.SellVolumePercent,5:F1}%)");
            Log($"    Delta:        {snapshot.Delta,10:F0}  |  Cumulative: {snapshot.CumulativeDelta,10:F0}");
            Log($"    Imbalance:    {snapshot.Imbalance,10:F0} ({snapshot.ImbalancePercent,6:F2}%)");

            if (snapshot.BidSize > 0 || snapshot.AskSize > 0)
            {
                Log($"    Bid:          {snapshot.BidPrice,10:F2} x {snapshot.BidSize,8:F0}");
                Log($"    Ask:          {snapshot.AskPrice,10:F2} x {snapshot.AskSize,8:F0}");
            }

            if (snapshot.Size > 0)
            {
                string aggressorStr = snapshot.Aggressor == AggressorFlag.Buy ? "BUY"
                    : snapshot.Aggressor == AggressorFlag.Sell ? "SELL"
                    : "N/A";
                Log($"    Last Trade:   {snapshot.Last,10:F2} x {snapshot.Size,8:F0} ({aggressorStr})");
            }
        }

        private void Log(string message)
        {
            logAction?.Invoke(message, StrategyLoggingLevel.Trading);
        }
    }
}
