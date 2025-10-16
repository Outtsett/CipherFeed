using System;
using System.Collections.Generic;
using System.Linq;

namespace CipherFeed.Indicators
{
    /// <summary>
    /// TWAP (Time-Weighted Average Price) Indicator
    /// Calculates TWAP and deviation bands
    /// </summary>
    public class TWAPIndicator
    {
        #region Constants

        // TWAP band deviation multiplier
        private const double BAND_MULTIPLIER = 1.5;

        #endregion

        #region Properties

        /// <summary>
        /// Time-Weighted Average Price
        /// </summary>
        public double TWAP { get; private set; }

        /// <summary>
        /// Upper 1.5? band
        /// </summary>
        public double UpperBand { get; private set; }

        /// <summary>
        /// Lower 1.5? band
        /// </summary>
        public double LowerBand { get; private set; }

        /// <summary>
        /// Band deviation as percentage
        /// </summary>
        public double BandPercent { get; private set; }

        #endregion

        #region Calculation

        /// <summary>
        /// Calculate TWAP and bands from price-time data
        /// </summary>
        /// <param name="priceTimePoints">List of price-time data points</param>
        public void Calculate(List<PriceTimePoint> priceTimePoints)
        {
            if (priceTimePoints == null || priceTimePoints.Count == 0)
            {
                return;
            }

            // Calculate TWAP: Simple average of prices over time
            double sumPrices = priceTimePoints.Sum(p => p.Price);
            TWAP = sumPrices / priceTimePoints.Count;

            // Calculate standard deviation for TWAP bands
            double sumSquaredDiff = 0;
            foreach (PriceTimePoint point in priceTimePoints)
            {
                double diff = point.Price - TWAP;
                sumSquaredDiff += diff * diff;
            }

            double variance = sumSquaredDiff / priceTimePoints.Count;
            double stdDev = Math.Sqrt(variance);

            // TWAP Bands - Absolute Prices
            UpperBand = TWAP + (BAND_MULTIPLIER * stdDev);
            LowerBand = TWAP - (BAND_MULTIPLIER * stdDev);

            // Calculate percentage deviation
            if (TWAP > 0)
            {
                BandPercent = stdDev / TWAP * 100 * BAND_MULTIPLIER;
            }
        }

        /// <summary>
        /// Reset all calculated values
        /// </summary>
        public void Reset()
        {
            TWAP = 0;
            UpperBand = 0;
            LowerBand = 0;
            BandPercent = 0;
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Check if price is above TWAP
        /// </summary>
        public bool IsPriceAboveTWAP(double price)
        {
            return TWAP > 0 && price > TWAP;
        }

        /// <summary>
        /// Get distance from TWAP in percentage
        /// </summary>
        public double GetDistanceFromTWAP(double price)
        {
            return TWAP == 0 ? 0 : (price - TWAP) / TWAP * 100;
        }

        /// <summary>
        /// Check if price is within TWAP bands
        /// </summary>
        public bool IsPriceWithinBands(double price)
        {
            return price >= LowerBand && price <= UpperBand;
        }

        #endregion
    }
}
