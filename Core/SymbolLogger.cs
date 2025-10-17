/*
 * ============================================================================
 * SYMBOL LOGGER
 * ============================================================================
 * 
 * Handles formatted console logging for real-time market data, indicators,
 * and orderflow features. Provides structured output for monitoring and
 * debugging the CipherFeed strategy.
 * 
 * ============================================================================
 * QUANTOWER API REFERENCES
 * ============================================================================
 * 
 * Strategy Logging:
 *   - Strategy.Log(): quantower-api\Strategies\Strategy.md
 *     URL: https://api.quantower.com/docs/TradingPlatform.BusinessLayer.Strategy.html
 *     Method: Log(string message, StrategyLoggingLevel level)
 *     Purpose: Write messages to Quantower's Strategy Log panel
 * 
 *   - StrategyLoggingLevel: quantower-api\Strategies\StrategyLoggingLevel.md
 *     URL: https://api.quantower.com/docs/TradingPlatform.BusinessLayer.StrategyLoggingLevel.html
 *     Values: Trading, Error, Warning, Info, Debug
 *     Purpose: Categorizes log messages by severity/type
 * 
 * Data Types Logged:
 *   - AggressorFlag: quantower-api\Core\Enums\AggressorFlag.md
 *     URL: https://api.quantower.com/docs/TradingPlatform.BusinessLayer.AggressorFlag.html
 *     Values: Buy, Sell, None
 *     Usage: Displayed in orderflow features (last trade aggressor)
 * 
 *   - TickDirection: quantower-api\Core\Enums\TickDirection.md
 *     URL: https://api.quantower.com/docs/TradingPlatform.BusinessLayer.TickDirection.html
 *     Values: Up, Down, Undefined
 *     Usage: Not directly logged but tracked in snapshot
 * 
 * Indicators:
 *   - Indicator Methods: quantower-api\Indicators\Indicator.md
 *     URL: https://api.quantower.com/docs/TradingPlatform.BusinessLayer.Indicator.html
 *     Purpose: Custom indicator getter methods (GetVWAP, GetVPOC, etc.)
 *     Used in: LogVWAP(), LogVPOC(), LogTWAP()
 * 
 * ============================================================================
 * OUTPUT FORMAT STRUCTURE
 * ============================================================================
 * 
 * Market Snapshot Header:
 *   ================================================================================
 *     [RTH] Market Snapshot - 14:32:15 UTC
 *   ================================================================================
 * 
 * Per-Symbol Section:
 *   --- MNQ (UP) ---------------------------------------------------------------
 *     Price: 21245.50  |  Change: +0.234% (+49.75 pts)
 * 
 *     VWAP Analysis:
 *       VWAP Price:  21240.25  (ABOVE by 5.25 pts / 0.025%)
 *       +/- 2SD:     Upper 21280.00 (+0.187%)  |  Lower 21200.50 (-0.187%)
 *       MPD Bands:   Upper 21265.00 (+0.118%)  |  Lower 21215.50 (-0.118%)
 * 
 *     Volume Profile:
 *       VPOC:        21238.75 (+0.012%)
 *       VAH:         21255.00 (+0.089%)
 *       VAL:         21225.00 (-0.085%)
 *       Position:    INSIDE VALUE AREA
 * 
 *     TWAP Analysis:
 *       TWAP Price:  21242.00  (ABOVE by 3.50 pts / 0.016%)
 *       +/- 2SD:     Upper 21275.00 (+0.154%)  |  Lower 21209.00 (-0.154%)
 * 
 *   [Optional] Orderflow Features:
 *       Volume:       12450  |  Trades: 234
 *       Buy Vol:       7250 (58.2%)
 *       Sell Vol:      5200 (41.8%)
 *       Delta:        +2050  |  Cumulative: +15230
 *       Imbalance:    +2050 (+16.46%)
 *   --------------------------------------------------------------------------------
 * 
 * ============================================================================
 * USAGE IN CIPHERFEED STRATEGY
 * ============================================================================
 * 
 * Created in: CipherFeed.cs::OnCreated()
 * Called from: CipherFeed.cs::LogSymbolUpdate() (if EnableRealtimeLogging = true)
 * 
 * Log Methods:
 *   - LogSymbolHeader(): Symbol name, price, change from session open
 *   - LogVWAP(): VWAP price, std dev bands, MPD bands
 *   - LogVPOC(): VPOC, VAH, VAL, position relative to value area
 *   - LogTWAP(): TWAP price, std dev bands
 *   - LogOrderflowFeatures(): Volume, delta, imbalance, trades (optional)
 *   - LogSymbolFooter(): Section separator
 * 
 * ============================================================================
 */

using CipherFeed.Indicators;
using System;
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
        public void LogOrderflowFeatures(Models.MarketDataSnapshot snapshot)
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
