using System;
using System.Collections.Generic;
using System.Linq;

namespace CipherFeed.Indicators
{
    /// <summary>
    /// VPOC (Volume Point of Control) and Value Area Indicator
    /// Calculates the price level with the highest traded volume and the value area boundaries (VAH/VAL)
    /// </summary>
    public class VPOCIndicator
    {
        #region Constants

        // Percentage of volume to include in value area
        private const double VALUE_AREA_VOLUME_PERCENT = 0.70;

        #endregion

        #region Properties

        /// <summary>
        /// Volume Point of Control - price with highest volume
        /// </summary>
        public double VPOC { get; private set; }

        /// <summary>
        /// Volume at the VPOC level
        /// </summary>
        public double VPOCVolume { get; private set; }

        /// <summary>
        /// Value Area High - upper boundary of value area
        /// </summary>
        public double VAH { get; private set; }

        /// <summary>
        /// Value Area Low - lower boundary of value area
        /// </summary>
        public double VAL { get; private set; }

        /// <summary>
        /// Value area range as percentage
        /// </summary>
        public double ValueAreaRangePercent { get; private set; }

        /// <summary>
        /// VAH distance from VPOC as percentage
        /// </summary>
        public double VAHDistancePercent { get; private set; }

        /// <summary>
        /// VAL distance from VPOC as percentage
        /// </summary>
        public double VALDistancePercent { get; private set; }

        #endregion

        #region Calculation

        /// <summary>
        /// Calculate VPOC and Value Area from volume profile data
        /// </summary>
        /// <param name="volumeProfile">Dictionary of price levels and their volumes</param>
        public void Calculate(Dictionary<double, double> volumeProfile)
        {
            if (volumeProfile == null || volumeProfile.Count == 0)
            {
                return;
            }

            // Find price level with maximum volume (VPOC)
            KeyValuePair<double, double> maxVolumeEntry = volumeProfile
                .OrderByDescending(kvp => kvp.Value)
                .FirstOrDefault();

            VPOC = maxVolumeEntry.Key;
            VPOCVolume = maxVolumeEntry.Value;

            // Calculate Value Area (VAH and VAL)
            CalculateValueArea(volumeProfile);
        }

        /// <summary>
        /// Calculate Value Area High (VAH) and Value Area Low (VAL)
        /// The value area contains 70% of the traded volume
        /// </summary>
        private void CalculateValueArea(Dictionary<double, double> volumeProfile)
        {
            if (volumeProfile == null || volumeProfile.Count == 0 || VPOC == 0)
            {
                return;
            }

            // Sort volume profile by price
            List<KeyValuePair<double, double>> sortedProfile = volumeProfile
                .OrderBy(kvp => kvp.Key)
                .ToList();

            // Calculate total volume
            double totalVolume = sortedProfile.Sum(kvp => kvp.Value);
            double targetVolume = totalVolume * VALUE_AREA_VOLUME_PERCENT;

            // Start from VPOC and expand outward to capture 70% of volume
            int vpocIndex = sortedProfile.FindIndex(kvp => kvp.Key == VPOC);
            if (vpocIndex == -1)
            {
                vpocIndex = sortedProfile.Count / 2;
            }

            double accumulatedVolume = sortedProfile[vpocIndex].Value;
            int lowIndex = vpocIndex;
            int highIndex = vpocIndex;

            // Expand the value area until we reach 70% of volume
            while (accumulatedVolume < targetVolume)
            {
                double volumeAbove = (highIndex < sortedProfile.Count - 1)
                    ? sortedProfile[highIndex + 1].Value
                    : 0;
                double volumeBelow = (lowIndex > 0)
                    ? sortedProfile[lowIndex - 1].Value
                    : 0;

                if (volumeAbove >= volumeBelow && highIndex < sortedProfile.Count - 1)
                {
                    highIndex++;
                    accumulatedVolume += sortedProfile[highIndex].Value;
                }
                else if (lowIndex > 0)
                {
                    lowIndex--;
                    accumulatedVolume += sortedProfile[lowIndex].Value;
                }
                else
                {
                    break;
                }
            }

            VAL = sortedProfile[lowIndex].Key;
            VAH = sortedProfile[highIndex].Key;

            // Calculate percentage-based value area metrics
            double valueAreaRange = VAH - VAL;
            double midPoint = (VAH + VAL) / 2;

            if (midPoint > 0)
            {
                ValueAreaRangePercent = valueAreaRange / midPoint * 100;
            }

            if (VPOC > 0)
            {
                VAHDistancePercent = (VAH - VPOC) / VPOC * 100;
                VALDistancePercent = (VPOC - VAL) / VPOC * 100;
            }
        }

        /// <summary>
        /// Reset all calculated values
        /// </summary>
        public void Reset()
        {
            VPOC = 0;
            VPOCVolume = 0;
            VAH = 0;
            VAL = 0;
            ValueAreaRangePercent = 0;
            VAHDistancePercent = 0;
            VALDistancePercent = 0;
        }

        #endregion

        #region Helper Methods - VPOC

        /// <summary>
        /// Check if price is above VPOC
        /// </summary>
        public bool IsPriceAboveVPOC(double price)
        {
            return VPOC > 0 && price > VPOC;
        }

        /// <summary>
        /// Get distance from VPOC in percentage
        /// </summary>
        public double GetDistanceFromVPOC(double price)
        {
            return VPOC == 0 ? 0 : (price - VPOC) / VPOC * 100;
        }

        /// <summary>
        /// Check if price is near VPOC (within threshold)
        /// </summary>
        public bool IsPriceNearVPOC(double price, double thresholdPercent = 0.1)
        {
            return Math.Abs(GetDistanceFromVPOC(price)) <= thresholdPercent;
        }

        #endregion

        #region Helper Methods - Value Area

        /// <summary>
        /// Check if price is in value area
        /// </summary>
        public bool IsPriceInValueArea(double price)
        {
            return price >= VAL && price <= VAH;
        }

        /// <summary>
        /// Get distance from VAH in percentage
        /// </summary>
        public double GetDistanceFromVAH(double price)
        {
            return VAH == 0 ? 0 : (price - VAH) / VAH * 100;
        }

        /// <summary>
        /// Get distance from VAL in percentage
        /// </summary>
        public double GetDistanceFromVAL(double price)
        {
            return VAL == 0 ? 0 : (price - VAL) / VAL * 100;
        }

        /// <summary>
        /// Check if price is near VAH (within threshold)
        /// </summary>
        public bool IsPriceNearVAH(double price, double thresholdPercent = 0.1)
        {
            return Math.Abs(GetDistanceFromVAH(price)) <= thresholdPercent;
        }

        /// <summary>
        /// Check if price is near VAL (within threshold)
        /// </summary>
        public bool IsPriceNearVAL(double price, double thresholdPercent = 0.1)
        {
            return Math.Abs(GetDistanceFromVAL(price)) <= thresholdPercent;
        }

        /// <summary>
        /// Get value area position description
        /// </summary>
        public string GetValueAreaPosition(double price)
        {
            return price > VAH ? "ABOVE Value Area (Overbought)" : price < VAL ? "BELOW Value Area (Oversold)" : "WITHIN Value Area (Balanced)";
        }

        #endregion
    }
}
