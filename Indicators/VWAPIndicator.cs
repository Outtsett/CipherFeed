using System;
using System.Collections.Generic;
using System.Linq;

namespace CipherFeed.Indicators
{
    /// <summary>
    /// Price and volume data point for VWAP calculations
    /// </summary>
    public class PriceVolumePoint
    {
        public DateTime Time { get; set; }
        public double Price { get; set; }
        public double Volume { get; set; }
        public double TypicalPrice { get; set; } // (High + Low + Close) / 3
        public double High { get; set; }
        public double Low { get; set; }
    }

    /// <summary>
    /// Price and time data point for TWAP calculations
    /// </summary>
    public class PriceTimePoint
    {
        public DateTime Time { get; set; }
        public double Price { get; set; }
    }

    /// <summary>
    /// Trading session type
    /// </summary>
    public enum TradingSession
    {
        RTH,  // Regular Trading Hours
        ETH   // Extended Trading Hours
    }

    /// <summary>
    /// VWAP (Volume-Weighted Average Price) Indicator
    /// Calculates VWAP and standard deviation bands
    /// </summary>
    public class VWAPIndicator
    {
        #region Constants

        // Standard deviation multiplier for VWAP bands
        private const double STD_DEV_MULTIPLIER = 2.0;

        // Session times (in local time)
        // RTH: 5:00 AM - 1:45 PM
        // ETH: 3:00 PM - 5:00 AM (next day)
        private static readonly TimeSpan RTH_START = new(5, 0, 0);    // 5:00 AM
        private static readonly TimeSpan RTH_END = new(13, 45, 0);    // 1:45 PM
        private static readonly TimeSpan ETH_START = new(15, 0, 0);   // 3:00 PM
        private static readonly TimeSpan ETH_END = new(5, 0, 0);      // 5:00 AM (next day)

        #endregion

        #region Properties

        /// <summary>
        /// Volume-Weighted Average Price
        /// </summary>
        public double VWAP { get; private set; }

        /// <summary>
        /// Upper 2σ standard deviation band
        /// </summary>
        public double UpperStdDev { get; private set; }

        /// <summary>
        /// Lower 2σ standard deviation band
        /// </summary>
        public double LowerStdDev { get; private set; }

        /// <summary>
        /// Upper MPD (Maximum Permissible Deviation) band
        /// Calculated as VWAP + MPD where MPD = (period high - period low) / 2
        /// </summary>
        public double UpperMPD { get; private set; }

        /// <summary>
        /// Lower MPD band
        /// Calculated as VWAP - MPD where MPD = (period high - period low) / 2
        /// </summary>
        public double LowerMPD { get; private set; }

        /// <summary>
        /// Standard deviation as percentage (2σ)
        /// </summary>
        public double StdDevPercent { get; private set; }

        /// <summary>
        /// MPD as percentage
        /// </summary>
        public double MPDPercent { get; private set; }

        /// <summary>
        /// MPD value (period high - period low) / 2
        /// </summary>
        public double MPD { get; private set; }

        /// <summary>
        /// Current trading session
        /// </summary>
        public TradingSession CurrentSession { get; private set; }

        #endregion

        #region Session Detection

        /// <summary>
        /// Determine which trading session a timestamp belongs to
        /// RTH: 5:00 AM - 1:45 PM
        /// ETH: 3:00 PM - 5:00 AM (next day)
        /// </summary>
        public static TradingSession GetTradingSession(DateTime timestamp)
        {
            TimeSpan time = timestamp.TimeOfDay;

            // RTH: 5:00 AM - 1:45 PM
            if (time >= RTH_START && time <= RTH_END)
            {
                return TradingSession.RTH;
            }
            // ETH: 3:00 PM - 5:00 AM (crosses midnight)
            else if (time >= ETH_START || time < ETH_END)
            {
                return TradingSession.ETH;
            }

            // Default to RTH for gap period (1:45 PM - 3:00 PM)
            return TradingSession.RTH;
        }

        /// <summary>
        /// Get the session start time for a given timestamp
        /// </summary>
        public static DateTime GetSessionStart(DateTime timestamp)
        {
            TradingSession session = GetTradingSession(timestamp);
            TimeSpan time = timestamp.TimeOfDay;

            if (session == TradingSession.RTH)
            {
                // RTH starts at 5:00 AM on the same day
                return timestamp.Date.Add(RTH_START);
            }
            else // ETH
            {
                // If current time is before 5 AM, session started previous day at 3 PM
                return time < ETH_END
                    ? timestamp.Date.AddDays(-1).Add(ETH_START)
                    // Otherwise, session starts today at 3 PM
                    : timestamp.Date.Add(ETH_START);
            }
        }

        /// <summary>
        /// Filter price-volume points to only include data from the current session
        /// </summary>
        private static List<PriceVolumePoint> FilterByCurrentSession(List<PriceVolumePoint> priceVolumePoints, DateTime currentTime)
        {
            if (priceVolumePoints == null || priceVolumePoints.Count == 0)
            {
                return priceVolumePoints;
            }

            DateTime sessionStart = GetSessionStart(currentTime);
            TradingSession currentSession = GetTradingSession(currentTime);

            // Filter points that are:
            // 1. After session start
            // 2. In the same session as current time
            return priceVolumePoints
                .Where(point => point.Time >= sessionStart && GetTradingSession(point.Time) == currentSession)
                .ToList();
        }

        #endregion

        #region Calculation

        /// <summary>
        /// Calculate VWAP and bands from price-volume data
        /// Automatically filters data to current session period (RTH or ETH)
        /// </summary>
        /// <param name="priceVolumePoints">List of price-volume data points</param>
        public void Calculate(List<PriceVolumePoint> priceVolumePoints)
        {
            Calculate(priceVolumePoints, DateTime.Now);
        }

        /// <summary>
        /// Calculate VWAP and bands from price-volume data with explicit current time
        /// Filters data to the session period containing currentTime
        /// </summary>
        /// <param name="priceVolumePoints">List of price-volume data points</param>
        /// <param name="currentTime">Current time to determine active session</param>
        public void Calculate(List<PriceVolumePoint> priceVolumePoints, DateTime currentTime)
        {
            if (priceVolumePoints == null || priceVolumePoints.Count == 0)
            {
                return;
            }

            // Determine current session
            CurrentSession = GetTradingSession(currentTime);

            // Filter data to only include current session
            List<PriceVolumePoint> sessionData = FilterByCurrentSession(priceVolumePoints, currentTime);

            if (sessionData.Count == 0)
            {
                return;
            }

            // Calculate VWAP: Sum(Price * Volume) / Sum(Volume)
            double sumPriceVolume = 0;
            double sumVolume = 0;
            double periodHigh = double.MinValue;
            double periodLow = double.MaxValue;

            foreach (PriceVolumePoint point in sessionData)
            {
                sumPriceVolume += point.TypicalPrice * point.Volume;
                sumVolume += point.Volume;

                // Track period high and low
                if (point.High > periodHigh)
                {
                    periodHigh = point.High;
                }
                if (point.Low < periodLow)
                {
                    periodLow = point.Low;
                }
            }

            if (sumVolume > 0)
            {
                VWAP = sumPriceVolume / sumVolume;

                // Calculate variance for standard deviation
                double sumSquaredDiff = 0;
                foreach (PriceVolumePoint point in sessionData)
                {
                    double diff = point.TypicalPrice - VWAP;
                    sumSquaredDiff += diff * diff * point.Volume;
                }

                double variance = sumSquaredDiff / sumVolume;
                double stdDev = Math.Sqrt(variance);

                // VWAP Standard Deviation Bands (2σ) - Absolute Prices
                UpperStdDev = VWAP + (STD_DEV_MULTIPLIER * stdDev);
                LowerStdDev = VWAP - (STD_DEV_MULTIPLIER * stdDev);

                // Calculate MPD using the formula: MPD = (period high - period low) / 2
                MPD = (periodHigh - periodLow) / 2.0;

                // VWAP MPD Bands - Absolute Prices
                UpperMPD = VWAP + MPD;
                LowerMPD = VWAP - MPD;

                // Calculate percentage deviations
                if (VWAP > 0)
                {
                    StdDevPercent = stdDev / VWAP * 100 * STD_DEV_MULTIPLIER;
                    MPDPercent = MPD / VWAP * 100;
                }
            }
        }

        /// <summary>
        /// Reset all calculated values
        /// </summary>
        public void Reset()
        {
            VWAP = 0;
            UpperStdDev = 0;
            LowerStdDev = 0;
            UpperMPD = 0;
            LowerMPD = 0;
            StdDevPercent = 0;
            MPDPercent = 0;
            MPD = 0;
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Check if price is above VWAP
        /// </summary>
        public bool IsPriceAboveVWAP(double price)
        {
            return VWAP > 0 && price > VWAP;
        }

        /// <summary>
        /// Get distance from VWAP in percentage
        /// </summary>
        public double GetDistanceFromVWAP(double price)
        {
            return VWAP == 0 ? 0 : (price - VWAP) / VWAP * 100;
        }

        /// <summary>
        /// Determine which band the price is in
        /// </summary>
        public string GetBandPosition(double price)
        {
            return price >= UpperMPD
                ? "ABOVE MPD (Extreme Overbought)"
                : price >= UpperStdDev
                    ? "Between 2σ and MPD (Strong Overbought)"
                    : price > VWAP
                                    ? "Between VWAP and 2σ Upper (Above VWAP)"
                                    : price == VWAP
                                                    ? "AT VWAP (Equilibrium)"
                                                    : price > LowerStdDev
                                                                    ? "Between VWAP and 2σ Lower (Below VWAP)"
                                                                    : price > LowerMPD ? "Between 2σ and MPD (Strong Oversold)" : "BELOW MPD (Extreme Oversold)";
        }

        #endregion
    }
}
