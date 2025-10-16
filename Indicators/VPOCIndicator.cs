using System;
using System.Collections.Generic;
using System.Linq;
using TradingPlatform.BusinessLayer;

namespace CipherFeed.Indicators
{
    public class VPOCIndicator : Indicator
    {
        #region Private Fields

        private Dictionary<double, double> volumeByPrice;
        private double tickSize;
        private double sessionOpen;
        private int barCount;

        // Calculated values (in percentage space)
        private double vpoc;
        private double vah;
        private double val;

        #endregion

        #region Constructor

        public VPOCIndicator()
        {
            Name = "VPOC Indicator";
            Description = "Volume Point of Control with Value Area High/Low (in percentage space)";
            SeparateWindow = false;
        }

        #endregion

        #region Lifecycle Methods

        protected override void OnInit()
        {
            volumeByPrice = new Dictionary<double, double>();
            tickSize = Symbol?.TickSize ?? 0.01;
            sessionOpen = 0.0;
            barCount = 0;
        }

        protected override void OnUpdate(UpdateArgs args)
        {
            if (Count < 1)
                return;

            double volume = Volume();

            if (volume <= 0 || double.IsNaN(volume))
            {
                vpoc = double.NaN;
                vah = double.NaN;
                val = double.NaN;
                return;
            }

            double high = High();
            double low = Low();
            double close = Close();

            // Set session open on first bar
            if (barCount == 0)
            {
                sessionOpen = (high + low + close) / 3.0;
            }

            if (sessionOpen == 0.0 || double.IsNaN(sessionOpen))
            {
                vpoc = double.NaN;
                vah = double.NaN;
                val = double.NaN;
                return;
            }

            if (double.IsNaN(high) || double.IsNaN(low) || high < low)
            {
                vpoc = double.NaN;
                vah = double.NaN;
                val = double.NaN;
                return;
            }

            barCount++;

            // Convert prices to percentage space
            double highPct = (high - sessionOpen) / sessionOpen;
            double lowPct = (low - sessionOpen) / sessionOpen;
            double closePct = (close - sessionOpen) / sessionOpen;

            // Distribute volume across price levels in the bar
            DistributeVolume(highPct, lowPct, closePct, volume);

            // Calculate VPOC, VAH, VAL
            CalculateVolumeProfile();
        }

        protected override void OnClear()
        {
            volumeByPrice?.Clear();
            sessionOpen = 0.0;
            barCount = 0;
        }

        #endregion

        #region Volume Profile Calculation

        private void DistributeVolume(double high, double low, double close, double volume)
        {
            // Convert tick size to percentage space
            double tickSizePct = tickSize / sessionOpen;

            // Round prices to tick size
            double priceStart = Math.Floor(low / tickSizePct) * tickSizePct;
            double priceEnd = Math.Ceiling(high / tickSizePct) * tickSizePct;

            int numLevels = (int)Math.Round((priceEnd - priceStart) / tickSizePct) + 1;

            if (numLevels <= 0)
                numLevels = 1;

            double volumePerLevel = volume / numLevels;

            for (double price = priceStart; price <= priceEnd; price += tickSizePct)
            {
                double roundedPrice = Math.Round(price / tickSizePct) * tickSizePct;

                if (!volumeByPrice.ContainsKey(roundedPrice))
                    volumeByPrice[roundedPrice] = 0;

                volumeByPrice[roundedPrice] += volumePerLevel;
            }
        }

        private void CalculateVolumeProfile()
        {
            if (volumeByPrice.Count == 0)
            {
                vpoc = double.NaN;
                vah = double.NaN;
                val = double.NaN;
                return;
            }

            // Find VPOC (price with maximum volume) - already in percentage space
            vpoc = volumeByPrice.OrderByDescending(kvp => kvp.Value).First().Key;

            // Calculate total volume
            double totalVolume = volumeByPrice.Values.Sum();

            // Sort prices by volume descending
            var sortedByVolume = volumeByPrice.OrderByDescending(kvp => kvp.Value).ToList();

            // Build value area (70% of volume)
            double targetVolume = totalVolume * 0.70;
            double accumulatedVolume = 0;
            List<double> valueAreaPrices = new List<double>();

            foreach (var kvp in sortedByVolume)
            {
                valueAreaPrices.Add(kvp.Key);
                accumulatedVolume += kvp.Value;

                if (accumulatedVolume >= targetVolume)
                    break;
            }

            if (valueAreaPrices.Count > 0)
            {
                vah = valueAreaPrices.Max();
                val = valueAreaPrices.Min();
            }
            else
            {
                vah = double.NaN;
                val = double.NaN;
            }
        }

        #endregion

        #region Public Methods

        public double GetVPOC(int offset = 0)
        {
            return vpoc;
        }

        public double GetVAH(int offset = 0)
        {
            return vah;
        }

        public double GetVAL(int offset = 0)
        {
            return val;
        }

        public double GetSessionOpen()
        {
            return sessionOpen;
        }

        #endregion
    }
}