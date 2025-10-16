using System;
using System.Collections.Generic;
using System.Linq;
using TradingPlatform.BusinessLayer;

namespace CipherFeed.PatternDetection
{
    /// <summary>
    /// Failed auction pattern (low-volume rejection)
    /// </summary>
    public class FailedAuction
    {
        public double Price { get; set; }
        public string Direction { get; set; }
        public double Volume { get; set; }
        public double VolumeRatio { get; set; }
        public double WickToBodyRatio { get; set; }
        public string NearKeyLevel { get; set; }
        public DateTime Timestamp { get; set; }
        public int Confidence { get; set; }
    }

    /// <summary>
    /// Detects failed auctions (low-volume rejections at key levels)
    /// </summary>
    public class FailedAuctionDetector
    {
        #region Configuration

        private const double LOW_VOLUME_THRESHOLD = 0.5;            // 50% below average
        private const int REJECTION_WICK_RATIO = 3;                 // Wick must be 3x body

        #endregion

        #region Detection Methods

        /// <summary>
        /// Detect failed auctions
        /// </summary>
        public static List<FailedAuction> Detect(Symbol symbol, List<VolumeBar> recentBars, KeyPriceLevels keyLevels)
        {
            List<FailedAuction> failures = [];

            if (recentBars == null || recentBars.Count < 5)
            {
                return failures;
            }

            double avgVolume = recentBars.Take(50).Average(b => b.Volume);
            List<VolumeBar> recentWindow = recentBars.Take(5).ToList();

            foreach (VolumeBar bar in recentWindow)
            {
                // Failed auction signature: Low volume + large rejection wick
                if (bar.Volume < avgVolume * LOW_VOLUME_THRESHOLD)
                {
                    double body = Math.Abs(bar.Close - bar.Open);
                    double upperWick = bar.High - Math.Max(bar.Open, bar.Close);
                    double lowerWick = Math.Min(bar.Open, bar.Close) - bar.Low;

                    // Upper rejection
                    if (upperWick > body * REJECTION_WICK_RATIO)
                    {
                        failures.Add(new FailedAuction
                        {
                            Price = bar.High,
                            Direction = "Bearish Rejection",
                            Volume = bar.Volume,
                            VolumeRatio = bar.Volume / avgVolume,
                            WickToBodyRatio = upperWick / Math.Max(body, 0.01),
                            NearKeyLevel = IsNearKeyLevel(bar.High, keyLevels),
                            Timestamp = bar.Timestamp,
                            Confidence = CalculateFailedAuctionConfidence(bar.Volume / avgVolume, upperWick / body)
                        });
                    }

                    // Lower rejection
                    if (lowerWick > body * REJECTION_WICK_RATIO)
                    {
                        failures.Add(new FailedAuction
                        {
                            Price = bar.Low,
                            Direction = "Bullish Rejection",
                            Volume = bar.Volume,
                            VolumeRatio = bar.Volume / avgVolume,
                            WickToBodyRatio = lowerWick / Math.Max(body, 0.01),
                            NearKeyLevel = IsNearKeyLevel(bar.Low, keyLevels),
                            Timestamp = bar.Timestamp,
                            Confidence = CalculateFailedAuctionConfidence(bar.Volume / avgVolume, lowerWick / body)
                        });
                    }
                }
            }

            return failures;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Check if price is near a key market profile level
        /// </summary>
        private static string IsNearKeyLevel(double price, KeyPriceLevels keyLevels)
        {
            const double tolerance = 0.005; // 0.5%

            if (keyLevels == null)
            {
                return "None";
            }

            return Math.Abs(price - keyLevels.VWAP) / keyLevels.VWAP < tolerance
                ? "VWAP"
                : Math.Abs(price - keyLevels.VPOC) / keyLevels.VPOC < tolerance
                ? "VPOC"
                : Math.Abs(price - keyLevels.VAH) / keyLevels.VAH < tolerance
                ? "VAH"
                : Math.Abs(price - keyLevels.VAL) / keyLevels.VAL < tolerance ? "VAL" : "None";
        }

        /// <summary>
        /// Calculate confidence for failed auction
        /// </summary>
        private static int CalculateFailedAuctionConfidence(double volumeRatio, double wickRatio)
        {
            int score = 50;

            // Lower volume = stronger rejection signal
            if (volumeRatio < 0.3)
            {
                score += 25;
            }
            else if (volumeRatio < 0.5)
            {
                score += 15;
            }

            // Larger wick = stronger rejection
            if (wickRatio > 5)
            {
                score += 25;
            }
            else if (wickRatio > 3)
            {
                score += 15;
            }

            return Math.Min(score, 100);
        }

        #endregion
    }
}
