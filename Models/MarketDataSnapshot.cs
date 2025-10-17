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
        private int _cumulativeBuyTrades;
        private int _cumulativeSellTrades;
        private double _maxOneTradeVolume;
        private double _cumulativeVWAPBid;
        private double _cumulativeVWAPAsk;
        private int _vwapBidCount;
        private int _vwapAskCount;

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
                _cumulativeBuyTrades++;
            }
            else if (last.AggressorFlag == AggressorFlag.Sell)
            {
                tradeDelta = -last.Size;
                _cumulativeSellVolume += last.Size;
                _cumulativeSellTrades++;
            }

            // Update cumulative delta
            _cumulativeDelta += tradeDelta;
            CumulativeDelta = _cumulativeDelta;

            // Set cumulative volumes
            BuyVolume = _cumulativeBuyVolume;
            SellVolume = _cumulativeSellVolume;
            BuyTrades = _cumulativeBuyTrades;
            SellTrades = _cumulativeSellTrades;

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

            // Calculate average sizes
            if (Trades > 0)
            {
                AverageSize = Volume / Trades;
            }
            if (BuyTrades > 0)
            {
                AverageBuySize = BuyVolume / BuyTrades;
            }
            if (SellTrades > 0)
            {
                AverageSellSize = SellVolume / SellTrades;
            }

            // Track max one trade volume
            if (last.Size > _maxOneTradeVolume)
            {
                _maxOneTradeVolume = last.Size;
            }
            MaxOneTradeVolume = _maxOneTradeVolume;
            if (totalVolume > 0)
            {
                MaxOneTradeVolumePercent = MaxOneTradeVolume / totalVolume * 100;
            }

            // Calculate filtered volume (filter out trades < 10% of max trade)
            double filterThreshold = MaxOneTradeVolume * 0.1;
            if (last.Size >= filterThreshold)
            {
                FilteredVolume += last.Size;
                if (last.AggressorFlag == AggressorFlag.Buy)
                {
                    FilteredBuyVolume += last.Size;
                }
                else if (last.AggressorFlag == AggressorFlag.Sell)
                {
                    FilteredSellVolume += last.Size;
                }
            }
            if (totalVolume > 0)
            {
                FilteredVolumePercent = FilteredVolume / totalVolume * 100;
                FilteredBuyVolumePercent = FilteredBuyVolume / totalVolume * 100;
                FilteredSellVolumePercent = FilteredSellVolume / totalVolume * 100;
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

            // Set cumulative sizes and total
            CumulativeSizeBid = _cumulativeSizeBid;
            CumulativeSizeAsk = _cumulativeSizeAsk;
            CumulativeSize = CumulativeSizeBid + CumulativeSizeAsk;
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
            CumulativeSize = CumulativeSizeBid + CumulativeSizeAsk;

            // Calculate VWAP for bid/ask
            if (quote.BidSize > 0)
            {
                _cumulativeVWAPBid += quote.Bid * quote.BidSize;
                _vwapBidCount++;
                if (_vwapBidCount > 0)
                {
                    VWAPBid = _cumulativeVWAPBid / _cumulativeSizeBid;
                }
            }
            if (quote.AskSize > 0)
            {
                _cumulativeVWAPAsk += quote.Ask * quote.AskSize;
                _vwapAskCount++;
                if (_vwapAskCount > 0)
                {
                    VWAPAsk = _cumulativeVWAPAsk / _cumulativeSizeAsk;
                }
            }
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
            _cumulativeBuyTrades = 0;
            _cumulativeSellTrades = 0;
            _maxOneTradeVolume = 0.0;
            _cumulativeVWAPBid = 0.0;
            _cumulativeVWAPAsk = 0.0;
            _vwapBidCount = 0;
            _vwapAskCount = 0;

            CumulativeDelta = 0.0;
            BuyVolume = 0.0;
            SellVolume = 0.0;
            BuyTrades = 0;
            SellTrades = 0;
            CumulativeSizeBid = 0.0;
            CumulativeSizeAsk = 0.0;
            CumulativeSize = 0.0;
            MaxOneTradeVolume = 0.0;
            FilteredVolume = 0.0;
            FilteredBuyVolume = 0.0;
            FilteredSellVolume = 0.0;
            VWAPBid = 0.0;
            VWAPAsk = 0.0;
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
