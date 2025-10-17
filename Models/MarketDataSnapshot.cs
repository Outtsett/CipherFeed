/*
 * ============================================================================
 * MARKET DATA SNAPSHOT
 * ============================================================================
 * 
 * Comprehensive market data snapshot containing 65+ orderflow features per tick.
 * Manages its own cumulative state (delta, volume) and provides ML-ready
 * feature engineering for real-time trading analysis.
 * 
 * FEATURE CATEGORIES:
 *   - Metadata (2): Timestamp, SymbolName
 *   - Price/Trade (4): Last, Size, Aggressor, TickDirection
 *   - Bid/Ask (8): BidPrice, BidSize, AskPrice, AskSize, etc.
 *   - Volume/Delta (5): Volume, Trades, Delta, DeltaPercent, CumulativeDelta
 *   - Buy/Sell Volume (6): BuyVolume, SellVolume, percentages, trade counts
 *   - Imbalance (2): Imbalance, ImbalancePercent
 *   - Average Sizes (3): AverageSize, AverageBuySize, AverageSellSize
 *   - Max Trade (2): MaxOneTradeVolume, MaxOneTradeVolumePercent
 *   - Filtered Volume (6): FilteredVolume variants for noise reduction
 *   - VWAP Bid/Ask (2): VWAPBid, VWAPAsk
 *   - Cumulative Sizes (3): CumulativeSizeBid, CumulativeSizeAsk, CumulativeSize
 *   - Liquidity Changes (4): BidsLiquidityChanges, AsksLiquidityChanges, counts
 *   - Trade Sizes (3): LastTradeSize, BidTradeSize, AskTradeSize
 *   - Session Anchors (4): SessionOpen, VWAPSessionOpen, VPOCSessionOpen, TWAPSessionOpen
 * 
 * TOTAL: 56 snapshot features + 11 indicator values = 67 total exported to CSV
 * 
 * ============================================================================
 * QUANTOWER API REFERENCES
 * ============================================================================
 * 
 * Real-Time Market Data:
 *   - Last (Trade Tick): quantower-api\Core\Quotes\Last.md
 *     URL: https://api.quantower.com/docs/TradingPlatform.BusinessLayer.Last.html
 *     Properties: Price, Size, Time, AggressorFlag, TickDirection
 *     Purpose: Trade execution data (time & sales)
 *     Used in: UpdateFromLast()
 * 
 *   - Quote (Bid/Ask): quantower-api\Core\Quotes\Quote.md
 *     URL: https://api.quantower.com/docs/TradingPlatform.BusinessLayer.Quote.html
 *     Properties: Bid, BidSize, Ask, AskSize, Time, BidTickDirection, AskTickDirection
 *     Purpose: Order book top-of-book data
 *     Used in: UpdateFromQuote()
 * 
 *   - AggressorFlag: quantower-api\Core\Enums\AggressorFlag.md
 *     URL: https://api.quantower.com/docs/TradingPlatform.BusinessLayer.AggressorFlag.html
 *     Values: Buy (market buy), Sell (market sell), None
 *     Purpose: Identifies trade initiator (taker) for orderflow delta
 * 
 *   - TickDirection: quantower-api\Core\Enums\TickDirection.md
 *     URL: https://api.quantower.com/docs/TradingPlatform.BusinessLayer.TickDirection.html
 *     Values: Up, Down, Undefined
 *     Purpose: Price movement direction for momentum analysis
 * 
 * Symbol-Level Aggregates:
 *   - Symbol.Volume: quantower-api\Core\BusinessObjects\Symbol.md
 *     URL: https://api.quantower.com/docs/TradingPlatform.BusinessLayer.Symbol.html
 *     Property: Volume (double) - total traded volume from session start
 *     Purpose: Accessed in UpdateFromLast() for aggregate volume tracking
 * 
 *   - Symbol.Trades: quantower-api\Core\BusinessObjects\Symbol.md
 *     Property: Trades (long) - total trade count from session start
 *     Purpose: Accessed in UpdateFromLast() for trade count tracking
 * 
 *   - Symbol.Delta: quantower-api\Core\BusinessObjects\Symbol.md
 *     Property: Delta (double) - cumulative buy volume - sell volume
 *     Purpose: Accessed in UpdateFromLast() for orderflow delta
 * 
 * ============================================================================
 * CUMULATIVE STATE MANAGEMENT
 * ============================================================================
 * 
 * Internal Cumulative Fields (Private):
 *   _cumulativeDelta: Running delta (buy vol - sell vol) from session start
 *   _cumulativeBuyVolume: Total buy (aggressor) volume from session start
 *   _cumulativeSellVolume: Total sell (aggressor) volume from session start
 *   _cumulativeSizeBid: Running bid size accumulation
 *   _cumulativeSizeAsk: Running ask size accumulation
 * 
 * Update Pattern (per tick):
 *   1. UpdateFromLast() receives Last tick
 *   2. Identifies aggressor (Buy/Sell)
 *   3. Updates internal cumulative counters
 *   4. Calculates percentages and imbalances
 *   5. Copies to public properties for CSV export
 * 
 * Reset on Session Change:
 *   CipherFeed.cs::OnSessionChanged() calls ResetCumulativeState()
 *   All cumulative fields reset to 0.0
 *   Ensures session-anchored calculations
 * 
 * ============================================================================
 * USAGE IN CIPHERFEED STRATEGY
 * ============================================================================
 * 
 * Created: One instance per symbol in latestSnapshots dictionary
 * Updated by: 
 *   - CipherFeed.cs::OnNewLast_ForSymbol() ? UpdateFromLast()
 *   - CipherFeed.cs::OnNewQuote_ForSymbol() ? UpdateFromQuote()
 * 
 * Logged by: Core\SymbolLogger.cs::LogOrderflowFeatures()
 * Exported by: CSVDataExporter.cs::WriteSnapshot()
 * 
 * Accessed externally:
 *   - GetMarketDataSnapshot(Symbol) - retrieve by symbol reference
 *   - GetMarketDataSnapshot(string) - retrieve by symbol root name
 * 
 * ============================================================================
 */

using System;
using TradingPlatform.BusinessLayer;

namespace CipherFeed.Models
{
    /// <summary>
    /// Market data snapshot containing all orderflow features for a single symbol at a point in time.
    /// Manages its own cumulative state and calculations.
    /// Total: 51 orderflow features + metadata + indicator session opens = 65 total features
    /// </summary>
    public class MarketDataSnapshot
    {
        #region Metadata (2)

        public DateTime Timestamp { get; set; }
        public string SymbolName { get; set; }

        #endregion

        #region Price and Trade Information (4)

        public double Last { get; set; }
        public double Size { get; set; }
        public AggressorFlag Aggressor { get; set; }
        public TickDirection TickDirection { get; set; }

        #endregion

        #region Bid/Ask Information (8)

        public double BidPrice { get; set; }
        public double BidSize { get; set; }
        public TickDirection BidTickDirection { get; set; }
        public double AskPrice { get; set; }
        public double AskSize { get; set; }
        public TickDirection AskTickDirection { get; set; }
        public DateTime TimeBid { get; set; }
        public DateTime TimeAsk { get; set; }

        #endregion

        #region Volume and Delta (5)

        public double Volume { get; set; }
        public long Trades { get; set; }
        public double Delta { get; set; }
        public double DeltaPercent { get; set; }
        public double CumulativeDelta { get; set; }

        #endregion

        #region Buy/Sell Volume (6)

        public double BuyVolume { get; set; }
        public double BuyVolumePercent { get; set; }
        public double SellVolume { get; set; }
        public double SellVolumePercent { get; set; }
        public int BuyTrades { get; set; }
        public int SellTrades { get; set; }

        #endregion

        #region Imbalance (2)

        public double Imbalance { get; set; }
        public double ImbalancePercent { get; set; }

        #endregion

        #region Average Sizes (3)

        public double AverageSize { get; set; }
        public double AverageBuySize { get; set; }
        public double AverageSellSize { get; set; }

        #endregion

        #region Max Trade Volume (2)

        public double MaxOneTradeVolume { get; set; }
        public double MaxOneTradeVolumePercent { get; set; }

        #endregion

        #region Filtered Volume (6)

        public double FilteredVolume { get; set; }
        public double FilteredVolumePercent { get; set; }
        public double FilteredBuyVolume { get; set; }
        public double FilteredBuyVolumePercent { get; set; }
        public double FilteredSellVolume { get; set; }
        public double FilteredSellVolumePercent { get; set; }

        #endregion

        #region VWAP Bid/Ask (2)

        public double VWAPBid { get; set; }
        public double VWAPAsk { get; set; }

        #endregion

        #region Cumulative Sizes (3)

        public double CumulativeSizeBid { get; set; }
        public double CumulativeSizeAsk { get; set; }
        public double CumulativeSize { get; set; }

        #endregion

        #region Liquidity Changes (4)

        public double BidsLiquidityChanges { get; set; }
        public double AsksLiquidityChanges { get; set; }
        public int BidsNumberOfChanges { get; set; }
        public int AsksNumberOfChanges { get; set; }

        #endregion

        #region Trade Sizes by Side (3)

        public double LastTradeSize { get; set; }
        public double BidTradeSize { get; set; }
        public double AskTradeSize { get; set; }

        #endregion

        #region Session Anchors (4)

        /// <summary>Strategy-level session open price for percentage calculations</summary>
        public double SessionOpen { get; set; }

        /// <summary>VWAP indicator's internal session open for validation</summary>
        public double VWAPSessionOpen { get; set; }

        /// <summary>VPOC indicator's internal session open for validation</summary>
        public double VPOCSessionOpen { get; set; }

        /// <summary>TWAP indicator's internal session open for validation</summary>
        public double TWAPSessionOpen { get; set; }

        #endregion

        #region Internal Cumulative State

        // Private cumulative state - managed internally
        private double _cumulativeDelta;
        private double _cumulativeBuyVolume;
        private double _cumulativeSellVolume;
        private double _cumulativeSizeBid;
        private double _cumulativeSizeAsk;

        #endregion

        #region Update Methods

        /// <summary>
        /// Update snapshot from Last (trade) tick data.
        /// Manages all cumulative state internally.
        /// </summary>
        /// <param name="symbol">Symbol for current snapshot</param>
        /// <param name="last">Last trade tick</param>
        public void UpdateFromLast(Symbol symbol, Last last)
        {
            Timestamp = last.Time;
            Last = last.Price;
            Size = last.Size;
            Aggressor = last.AggressorFlag;
            TickDirection = last.TickDirection;

            // Update volume and trade data from Symbol properties
            Volume = symbol.Volume;
            Trades = symbol.Trades;
            Delta = symbol.Delta;

            // Calculate trade delta and update cumulative volumes
            double tradeDelta = 0;
            if (last.AggressorFlag == AggressorFlag.Buy)
            {
                tradeDelta = last.Size;
                _cumulativeBuyVolume += last.Size;
            }
            else if (last.AggressorFlag == AggressorFlag.Sell)
            {
                tradeDelta = -last.Size;
                _cumulativeSellVolume += last.Size;
            }

            // Update cumulative delta
            _cumulativeDelta += tradeDelta;
            CumulativeDelta = _cumulativeDelta;

            // Set cumulative volumes
            BuyVolume = _cumulativeBuyVolume;
            SellVolume = _cumulativeSellVolume;

            // Calculate percentages
            double totalVolume = BuyVolume + SellVolume;
            if (totalVolume > 0)
            {
                BuyVolumePercent = BuyVolume / totalVolume * 100;
                SellVolumePercent = SellVolume / totalVolume * 100;
            }

            // Calculate imbalance
            Imbalance = BuyVolume - SellVolume;
            if (totalVolume > 0)
            {
                ImbalancePercent = Imbalance / totalVolume * 100;
                DeltaPercent = Delta / totalVolume * 100;
            }

            // Track liquidity changes
            if (last.TickDirection == TickDirection.Up)
            {
                BidsLiquidityChanges = last.Size;
                AsksLiquidityChanges = 0;
            }
            else if (last.TickDirection == TickDirection.Down)
            {
                BidsLiquidityChanges = 0;
                AsksLiquidityChanges = last.Size;
            }

            // Count number of changes
            BidsNumberOfChanges++;
            AsksNumberOfChanges++;

            // Update last trade size
            LastTradeSize = last.Size;

            // Update bid/ask trade sizes based on aggressor
            if (last.AggressorFlag == AggressorFlag.Buy)
            {
                BidTradeSize = last.Size;
                AskTradeSize = 0;
            }
            else if (last.AggressorFlag == AggressorFlag.Sell)
            {
                BidTradeSize = 0;
                AskTradeSize = last.Size;
            }

            // Set cumulative sizes
            CumulativeSizeBid = _cumulativeSizeBid;
            CumulativeSizeAsk = _cumulativeSizeAsk;
        }

        /// <summary>
        /// Update snapshot from Quote (bid/ask) data.
        /// Manages cumulative bid/ask sizes internally.
        /// </summary>
        /// <param name="quote">Quote data</param>
        public void UpdateFromQuote(Quote quote)
        {
            Timestamp = quote.Time;
            BidPrice = quote.Bid;
            BidSize = quote.BidSize;
            BidTickDirection = quote.BidTickDirection;
            AskPrice = quote.Ask;
            AskSize = quote.AskSize;
            AskTickDirection = quote.AskTickDirection;
            TimeBid = quote.Time;
            TimeAsk = quote.Time;

            // Update cumulative bid/ask sizes
            _cumulativeSizeBid += quote.BidSize;
            _cumulativeSizeAsk += quote.AskSize;

            CumulativeSizeBid = _cumulativeSizeBid;
            CumulativeSizeAsk = _cumulativeSizeAsk;
        }

        /// <summary>
        /// Reset all cumulative state (called on session change)
        /// </summary>
        public void ResetCumulativeState()
        {
            _cumulativeDelta = 0.0;
            _cumulativeBuyVolume = 0.0;
            _cumulativeSellVolume = 0.0;
            _cumulativeSizeBid = 0.0;
            _cumulativeSizeAsk = 0.0;

            CumulativeDelta = 0.0;
            BuyVolume = 0.0;
            SellVolume = 0.0;
            CumulativeSizeBid = 0.0;
            CumulativeSizeAsk = 0.0;
        }

        #endregion

        /*
         * FEATURE COUNT VERIFICATION:
         * ============================
         * Metadata: 2 (Timestamp, SymbolName)
         * Price/Trade: 4 (Last, Size, Aggressor, TickDirection)
         * Bid/Ask: 8 (BidPrice, BidSize, BidTickDirection, AskPrice, AskSize, AskTickDirection, TimeBid, TimeAsk)
         * Volume/Delta: 5 (Volume, Trades, Delta, DeltaPercent, CumulativeDelta)
         * Buy/Sell Volume: 6 (BuyVolume, BuyVolumePercent, SellVolume, SellVolumePercent, BuyTrades, SellTrades)
         * Imbalance: 2 (Imbalance, ImbalancePercent)
         * Average Sizes: 3 (AverageSize, AverageBuySize, AverageSellSize)
         * Max Trade: 2 (MaxOneTradeVolume, MaxOneTradeVolumePercent)
         * Filtered Volume: 6 (FilteredVolume, FilteredVolumePercent, FilteredBuyVolume, FilteredBuyVolumePercent, FilteredSellVolume, FilteredSellVolumePercent)
         * VWAP Bid/Ask: 2 (VWAPBid, VWAPAsk)
         * Cumulative Sizes: 3 (CumulativeSizeBid, CumulativeSizeAsk, CumulativeSize)
         * Liquidity Changes: 4 (BidsLiquidityChanges, AsksLiquidityChanges, BidsNumberOfChanges, AsksNumberOfChanges)
         * Trade Sizes: 3 (LastTradeSize, BidTradeSize, AskTradeSize)
         * Session Anchors: 4 (SessionOpen, VWAPSessionOpen, VPOCSessionOpen, TWAPSessionOpen)
         * =====================
         * TOTAL: 56 features in this snapshot
         * 
         * (The remaining 11 indicator VALUES: VWAP, VWAP bands, VPOC, VAH, VAL, TWAP, TWAP bands
         *  are calculated externally and added during CSV export)
         */
    }
}
