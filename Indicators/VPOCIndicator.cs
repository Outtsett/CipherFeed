using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TradingPlatform.BusinessLayer;

namespace CipherFeed.Indicators
{
    public class VPOCIndicator : Indicator
    {
        #region Private Fields

        private Dictionary<double, double> volumeByPrice;
        private double tickSize;

        private const int LINE_VPOC = 0;
        private const int LINE_VAH = 1;
        private const int LINE_VAL = 2;

        #endregion

        #region Constructor

        public VPOCIndicator()
        {
            Name = "VPOC Indicator";
            Description = "Volume Point of Control with Value Area High/Low";
            SeparateWindow = false;

            AddLineSeries("VPOC", Color.Cyan, 2, LineStyle.Solid);
            AddLineSeries("VAH", Color.Green, 1, LineStyle.Dot);
            AddLineSeries("VAL", Color.Red, 1, LineStyle.Dot);
        }

        #endregion

        #region Lifecycle Methods

        protected override void OnInit()
        {
            volumeByPrice = new Dictionary<double, double>();
            tickSize = Symbol?.TickSize ?? 0.01;
        }

        protected override void OnUpdate(UpdateArgs args)
        {
            if (Count < 1)
                return;

            double volume = Volume();

            if (volume <= 0 || double.IsNaN(volume))
            {
                SetValueToAllLines(double.NaN);
                return;
            }

            double high = High();
            double low = Low();
            double close = Close();

            if (double.IsNaN(high) || double.IsNaN(low) || high < low)
            {
                SetValueToAllLines(double.NaN);
                return;
            }

            // Distribute volume across price levels in the bar
            DistributeVolume(high, low, close, volume);

            // Calculate VPOC, VAH, VAL
            CalculateVolumeProfile();
        }

        protected override void OnClear()
        {
            volumeByPrice?.Clear();
        }

        #endregion

        #region Volume Profile Calculation

        private void DistributeVolume(double high, double low, double close, double volume)
        {
            // Round prices to tick size
            double priceStart = Math.Floor(low / tickSize) * tickSize;
            double priceEnd = Math.Ceiling(high / tickSize) * tickSize;

            int numLevels = (int)Math.Round((priceEnd - priceStart) / tickSize) + 1;

            if (numLevels <= 0)
                numLevels = 1;

            double volumePerLevel = volume / numLevels;

            for (double price = priceStart; price <= priceEnd; price += tickSize)
            {
                double roundedPrice = Math.Round(price / tickSize) * tickSize;

                if (!volumeByPrice.ContainsKey(roundedPrice))
                    volumeByPrice[roundedPrice] = 0;

                volumeByPrice[roundedPrice] += volumePerLevel;
            }
        }

        private void CalculateVolumeProfile()
        {
            if (volumeByPrice.Count == 0)
            {
                SetValueToAllLines(double.NaN);
                return;
            }

            // Find VPOC (price with maximum volume)
            double vpoc = volumeByPrice.OrderByDescending(kvp => kvp.Value).First().Key;
            SetValue(vpoc, LINE_VPOC);

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
                double vah = valueAreaPrices.Max();
                double val = valueAreaPrices.Min();

                SetValue(vah, LINE_VAH);
                SetValue(val, LINE_VAL);
            }
            else
            {
                SetValue(double.NaN, LINE_VAH);
                SetValue(double.NaN, LINE_VAL);
            }
        }

        private void SetValueToAllLines(double value)
        {
            for (int i = 0; i < 3; i++)
            {
                SetValue(value, i);
            }
        }

        #endregion

        #region Public Methods

        public double GetVPOC(int offset = 0)
        {
            return GetValue(offset, LINE_VPOC);
        }

        public double GetVAH(int offset = 0)
        {
            return GetValue(offset, LINE_VAH);
        }

        public double GetVAL(int offset = 0)
        {
            return GetValue(offset, LINE_VAL);
        }

        #endregion
    }
}