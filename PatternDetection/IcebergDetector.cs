using System;
using System.Collections.Generic;
using System.Linq;
using TradingPlatform.BusinessLayer;

namespace CipherFeed.PatternDetection
{
    /// <summary>
    /// Iceberg order pattern (hidden institutional orders)
    /// </summary>
    public class IcebergPattern
    {
        public double Price { get; set; }
        public string KeyLevel { get; set; }
        public Side Side { get; set; }
        public double VisibleSize { get; set; }
        public double EstimatedTotalSize { get; set; }
        public int RefillCount { get; set; }
        public double DistanceFromLevel { get; set; }
        public int Confidence { get; set; }
        public double TypicalSize { get; set; }
        public double VolumeTraded { get; set; }
        public int ChangeCount { get; set; }
        public double AvgRefillSpeed { get; set; }
        public double TimeActive { get; set; }
    }

    /// <summary>
    /// Detects iceberg orders (hidden institutional size) at key price levels
    /// Uses advanced DOM tracking: refills, liquidity changes, volume accumulation, time analysis
    /// </summary>
    public class IcebergDetector
    {
        #region Configuration

        private const double ICEBERG_SIZE_MULTIPLIER = 3.0;        // Hidden size must be 3x visible
        private const int ICEBERG_REFILL_COUNT = 3;                 // Minimum refills to confirm
        private const double ICEBERG_PRICE_TOLERANCE = 0.02;        // 2% price tolerance

        #endregion

        #region State Tracking

        // Iceberg tracking state (per symbol, per price level)
        private static readonly Dictionary<string, Dictionary<double, IcebergTracker>> icebergTracking = [];

        #endregion

        #region Detection Methods

        /// <summary>
        /// Detect iceberg orders at key price levels
        /// </summary>
        public static List<IcebergPattern> Detect(Symbol symbol, DOMQuote dom, KeyPriceLevels keyLevels)
        {
            List<IcebergPattern> icebergs = [];

            if (dom == null || keyLevels == null || symbol == null)
            {
                return icebergs;
            }

            string symbolKey = symbol.Name;

            // Initialize tracking for this symbol if needed
            if (!icebergTracking.ContainsKey(symbolKey))
            {
                icebergTracking[symbolKey] = [];
            }

            // Update tracking with current DOM state
            UpdateIcebergTracking(symbolKey, dom, DateTime.UtcNow);

            // Check for icebergs near key levels
            IcebergPattern vwapIceberg = DetectIcebergAtLevel(symbol, keyLevels.VWAP, "VWAP", symbolKey);
            if (vwapIceberg != null)
            {
                icebergs.Add(vwapIceberg);
            }

            IcebergPattern vpocIceberg = DetectIcebergAtLevel(symbol, keyLevels.VPOC, "VPOC", symbolKey);
            if (vpocIceberg != null)
            {
                icebergs.Add(vpocIceberg);
            }

            IcebergPattern vahIceberg = DetectIcebergAtLevel(symbol, keyLevels.VAH, "VAH", symbolKey);
            if (vahIceberg != null)
            {
                icebergs.Add(vahIceberg);
            }

            IcebergPattern valIceberg = DetectIcebergAtLevel(symbol, keyLevels.VAL, "VAL", symbolKey);
            if (valIceberg != null)
            {
                icebergs.Add(valIceberg);
            }

            return icebergs;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Update iceberg tracking with current DOM state
        /// </summary>
        private static void UpdateIcebergTracking(string symbolKey, DOMQuote dom, DateTime timestamp)
        {
            Dictionary<double, IcebergTracker> tracking = icebergTracking[symbolKey];

            // Process bid side
            if (dom.Bids != null)
            {
                foreach (Level2Quote bid in dom.Bids)
                {
                    UpdatePriceLevel(tracking, bid.Price, bid.Size, Side.Buy, timestamp);
                }
            }

            // Process ask side
            if (dom.Asks != null)
            {
                foreach (Level2Quote ask in dom.Asks)
                {
                    UpdatePriceLevel(tracking, ask.Price, ask.Size, Side.Sell, timestamp);
                }
            }

            // Clean up stale trackers (not seen in last 60 seconds)
            List<double> staleKeys = tracking
                .Where(kvp => (timestamp - kvp.Value.LastUpdateTime).TotalSeconds > 60)
                .Select(kvp => kvp.Key)
                .ToList();

            foreach (double key in staleKeys)
            {
                _ = tracking.Remove(key);
            }
        }

        /// <summary>
        /// Update tracking for a specific price level
        /// </summary>
        private static void UpdatePriceLevel(Dictionary<double, IcebergTracker> tracking, double price, double size, Side side, DateTime timestamp)
        {
            if (!tracking.ContainsKey(price))
            {
                tracking[price] = new IcebergTracker
                {
                    Price = price,
                    Side = side,
                    FirstSeenTime = timestamp,
                    LastUpdateTime = timestamp,
                    CurrentSize = size,
                    PreviousSize = size
                };
            }
            else
            {
                IcebergTracker tracker = tracking[price];
                tracker.PreviousSize = tracker.CurrentSize;
                tracker.CurrentSize = size;
                tracker.LastUpdateTime = timestamp;

                // Detect refill: size was consumed (decreased) then restored (increased back to similar level)
                if (tracker.PreviousSize > 0 && tracker.CurrentSize > tracker.PreviousSize * 0.8)
                {
                    // Size increased back to near original - potential refill
                    if (tracker.CurrentSize >= tracker.TypicalSize * 0.9)
                    {
                        tracker.RefillCount++;
                        tracker.LastRefillTime = timestamp;
                        tracker.TotalRefillVolume += tracker.CurrentSize - tracker.PreviousSize;
                    }
                }

                // Track liquidity changes
                double liquidityChange = tracker.CurrentSize - tracker.PreviousSize;
                tracker.LiquidityChanges.Add(liquidityChange);
                tracker.ChangeCount++;

                // Update typical size (moving average)
                tracker.TypicalSize = tracker.TypicalSize == 0 ? tracker.CurrentSize : (tracker.TypicalSize * 0.8) + (tracker.CurrentSize * 0.2);

                // Accumulate volume traded at this level
                if (liquidityChange < 0) // Size decreased = volume consumed
                {
                    tracker.VolumeTraded += Math.Abs(liquidityChange);
                }

                // Keep only recent liquidity changes (last 50)
                if (tracker.LiquidityChanges.Count > 50)
                {
                    tracker.LiquidityChanges.RemoveAt(0);
                }
            }
        }

        /// <summary>
        /// Detect iceberg order at specific price level using tracking data
        /// </summary>
        private static IcebergPattern DetectIcebergAtLevel(Symbol symbol, double priceLevel, string levelName, string symbolKey)
        {
            if (priceLevel == 0 || !icebergTracking.ContainsKey(symbolKey))
            {
                return null;
            }

            Dictionary<double, IcebergTracker> tracking = icebergTracking[symbolKey];

            // Find trackers near the price level (within tolerance)
            List<IcebergTracker> nearbyTrackers = tracking.Values
                .Where(t => Math.Abs(t.Price - priceLevel) / priceLevel < ICEBERG_PRICE_TOLERANCE)
                .OrderByDescending(t => t.RefillCount)
                .ThenByDescending(t => t.VolumeTraded)
                .ToList();

            if (!nearbyTrackers.Any())
            {
                return null;
            }

            // Analyze each tracker for iceberg signature
            foreach (IcebergTracker tracker in nearbyTrackers)
            {
                // 1. Minimum refill count
                if (tracker.RefillCount < ICEBERG_REFILL_COUNT)
                {
                    continue;
                }

                // 2. Size consistency
                double sizeConsistency = tracker.TypicalSize > 0
                    ? Math.Abs(tracker.CurrentSize - tracker.TypicalSize) / tracker.TypicalSize
                    : 1.0;

                if (sizeConsistency > 0.3)
                {
                    continue;
                }

                // 3. Volume accumulation
                double volumeToSizeRatio = tracker.TypicalSize > 0
                    ? tracker.VolumeTraded / tracker.TypicalSize
                    : 0;

                if (volumeToSizeRatio < ICEBERG_SIZE_MULTIPLIER)
                {
                    continue;
                }

                // 4. High change count
                if (tracker.ChangeCount < 10)
                {
                    continue;
                }

                // 5. Calculate refill speed
                double refillSpeed = 0;
                if (tracker.RefillCount > 1 && tracker.LastRefillTime > tracker.FirstSeenTime)
                {
                    TimeSpan totalTime = tracker.LastRefillTime - tracker.FirstSeenTime;
                    refillSpeed = totalTime.TotalSeconds / tracker.RefillCount;
                }

                // ICEBERG CONFIRMED
                return new IcebergPattern
                {
                    Price = tracker.Price,
                    KeyLevel = levelName,
                    Side = tracker.Side,
                    VisibleSize = tracker.CurrentSize,
                    EstimatedTotalSize = tracker.VolumeTraded + tracker.CurrentSize,
                    RefillCount = tracker.RefillCount,
                    DistanceFromLevel = Math.Abs(tracker.Price - priceLevel) / priceLevel * 100,
                    Confidence = CalculateAdvancedIcebergConfidence(tracker, refillSpeed, priceLevel),
                    TypicalSize = tracker.TypicalSize,
                    VolumeTraded = tracker.VolumeTraded,
                    ChangeCount = tracker.ChangeCount,
                    AvgRefillSpeed = refillSpeed,
                    TimeActive = (DateTime.UtcNow - tracker.FirstSeenTime).TotalSeconds
                };
            }

            return null;
        }

        /// <summary>
        /// Calculate advanced confidence score for iceberg detection
        /// </summary>
        private static int CalculateAdvancedIcebergConfidence(IcebergTracker tracker, double refillSpeed, double keyLevel)
        {
            int score = 50;

            // Factor 1: Refill count
            if (tracker.RefillCount >= 10)
            {
                score += 25;
            }
            else if (tracker.RefillCount >= 7)
            {
                score += 20;
            }
            else if (tracker.RefillCount >= 5)
            {
                score += 15;
            }
            else if (tracker.RefillCount >= 3)
            {
                score += 10;
            }

            // Factor 2: Volume ratio
            double volumeRatio = tracker.TypicalSize > 0 ? tracker.VolumeTraded / tracker.TypicalSize : 0;
            if (volumeRatio >= 10.0)
            {
                score += 20;
            }
            else if (volumeRatio >= 7.0)
            {
                score += 15;
            }
            else if (volumeRatio >= 5.0)
            {
                score += 10;
            }

            // Factor 3: Refill speed
            if (refillSpeed > 0)
            {
                if (refillSpeed < 5.0)
                {
                    score += 15;
                }
                else if (refillSpeed < 10.0)
                {
                    score += 10;
                }
                else if (refillSpeed < 20.0)
                {
                    score += 5;
                }
            }

            // Factor 4: Change count
            if (tracker.ChangeCount >= 50)
            {
                score += 10;
            }
            else if (tracker.ChangeCount >= 30)
            {
                score += 7;
            }
            else if (tracker.ChangeCount >= 20)
            {
                score += 5;
            }

            // Factor 5: Proximity to key level
            double distanceFromKey = Math.Abs(tracker.Price - keyLevel) / keyLevel;
            if (distanceFromKey < 0.001)
            {
                score += 10;
            }
            else if (distanceFromKey < 0.005)
            {
                score += 5;
            }

            // Factor 6: Time active
            double hoursActive = (DateTime.UtcNow - tracker.FirstSeenTime).TotalHours;
            if (hoursActive >= 2.0)
            {
                score += 10;
            }
            else if (hoursActive >= 1.0)
            {
                score += 7;
            }
            else if (hoursActive >= 0.5)
            {
                score += 5;
            }

            return Math.Min(score, 100);
        }

        #endregion

        #region Internal Tracker Class

        /// <summary>
        /// Internal tracker for iceberg order detection
        /// </summary>
        private class IcebergTracker
        {
            public double Price { get; set; }
            public Side Side { get; set; }
            public double CurrentSize { get; set; }
            public double PreviousSize { get; set; }
            public double TypicalSize { get; set; }
            public int RefillCount { get; set; }
            public double TotalRefillVolume { get; set; }
            public DateTime LastRefillTime { get; set; }
            public List<double> LiquidityChanges { get; set; } = [];
            public int ChangeCount { get; set; }
            public double VolumeTraded { get; set; }
            public DateTime FirstSeenTime { get; set; }
            public DateTime LastUpdateTime { get; set; }
        }

        #endregion
    }
}
