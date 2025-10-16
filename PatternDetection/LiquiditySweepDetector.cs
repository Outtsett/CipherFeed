using System;
using System.Collections.Generic;
using System.Linq;
using TradingPlatform.BusinessLayer;

namespace CipherFeed.PatternDetection
{
    /// <summary>
    /// Liquidity sweep pattern (stop hunt)
    /// </summary>
    public class LiquiditySweep
    {
        public double Price { get; set; }
        public string Direction { get; set; }
        public double Volume { get; set; }
        public double VolumeRatio { get; set; }
        public bool ReversalConfirmed { get; set; }
        public DateTime Timestamp { get; set; }
    }

    /// <summary>
    /// Volume bar data structure
    /// </summary>
    public class VolumeBar
    {
        public DateTime Timestamp { get; set; }
        public double Open { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double Close { get; set; }
        public double Volume { get; set; }
    }

    /// <summary>
    /// Detects liquidity sweeps (stop hunts) at key levels
    /// </summary>
    public class LiquiditySweepDetector
    {
        #region Configuration

        private const double SWEEP_VOLUME_SPIKE = 2.5;              // Volume spike multiplier
        private const int SWEEP_TIME_WINDOW_MS = 500;               // Time window for sweep

        #endregion

        #region Detection Methods

        /// <summary>
        /// Detect liquidity sweeps (stop hunts) at key levels
        /// </summary>
        public static List<LiquiditySweep> Detect(Symbol symbol, DOMQuote dom, List<VolumeBar> recentBars)
        {
            List<LiquiditySweep> sweeps = [];

            if (recentBars == null || recentBars.Count < 3)
            {
                return sweeps;
            }

            // Get recent volume statistics
            double avgVolume = recentBars.Take(20).Average(b => b.Volume);
            List<VolumeBar> latestBars = recentBars.Take(3).ToList();

            foreach (VolumeBar bar in latestBars)
            {
                // Detect volume spike with quick reversal (sweep signature)
                if (bar.Volume > avgVolume * SWEEP_VOLUME_SPIKE)
                {
                    bool isUpperSweep = bar.High > bar.Close && (bar.High - bar.Close) > (bar.Close - bar.Open) * 2;
                    bool isLowerSweep = bar.Low < bar.Close && (bar.Close - bar.Low) > (bar.Open - bar.Close) * 2;

                    if (isUpperSweep)
                    {
                        sweeps.Add(new LiquiditySweep
                        {
                            Price = bar.High,
                            Direction = "Upside Sweep",
                            Volume = bar.Volume,
                            VolumeRatio = bar.Volume / avgVolume,
                            ReversalConfirmed = bar.Close < bar.Open,
                            Timestamp = bar.Timestamp
                        });
                    }
                    else if (isLowerSweep)
                    {
                        sweeps.Add(new LiquiditySweep
                        {
                            Price = bar.Low,
                            Direction = "Downside Sweep",
                            Volume = bar.Volume,
                            VolumeRatio = bar.Volume / avgVolume,
                            ReversalConfirmed = bar.Close > bar.Open,
                            Timestamp = bar.Timestamp
                        });
                    }
                }
            }

            return sweeps;
        }

        #endregion
    }
}
