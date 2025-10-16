using System;
using System.Drawing;
using TradingPlatform.BusinessLayer;

namespace CipherFeed.Indicators
{
    public class SessionAnchoredVWAP : Indicator
    {
        #region Input Parameters

        [InputParameter("Use Typical Price", 0)]
        public bool UseTypicalPrice { get; set; } = true;

        [InputParameter("Show Standard Deviation Bands", 1)]
        public bool ShowStdDevBands { get; set; } = true;

        [InputParameter("Show MPD Bands", 2)]
        public bool ShowMPDBands { get; set; } = true;

        #endregion

        #region Private Fields

        private double cumulativePriceVolume;
        private double cumulativeVolume;
        private double cumulativePriceSquaredVolume;
        private int barCount;

        private double sessionHigh;
        private double sessionLow;

        private const int LINE_VWAP = 0;
        private const int LINE_UPPER_STDDEV = 1;
        private const int LINE_LOWER_STDDEV = 2;
        private const int LINE_UPPER_MPD = 3;
        private const int LINE_LOWER_MPD = 4;

        #endregion

        #region Constructor

        public SessionAnchoredVWAP()
        {
            Name = "Session Anchored VWAP";
            Description = "Volume-Weighted Average Price with 2σ Std Dev and MPD bands";
            SeparateWindow = false;

            AddLineSeries("VWAP", Color.Yellow, 2, LineStyle.Solid);
            AddLineSeries("Upper Std Dev 2σ", Color.Orange, 1, LineStyle.Solid);
            AddLineSeries("Lower Std Dev 2σ", Color.Orange, 1, LineStyle.Solid);
            AddLineSeries("Upper MPD", Color.Red, 2, LineStyle.Dot);
            AddLineSeries("Lower MPD", Color.Red, 2, LineStyle.Dot);
        }

        #endregion

        #region Lifecycle Methods

        protected override void OnInit()
        {
            cumulativePriceVolume = 0.0;
            cumulativeVolume = 0.0;
            cumulativePriceSquaredVolume = 0.0;
            barCount = 0;
            sessionHigh = double.MinValue;
            sessionLow = double.MaxValue;
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

            if (high > sessionHigh)
                sessionHigh = high;
            if (low < sessionLow)
                sessionLow = low;

            double price;
            if (UseTypicalPrice)
            {
                double close = Close();
                price = (high + low + close) / 3.0;
            }
            else
            {
                price = Close();
            }

            if (double.IsNaN(price) || price <= 0)
            {
                SetValueToAllLines(double.NaN);
                return;
            }

            double priceVolume = price * volume;
            cumulativePriceVolume += priceVolume;
            cumulativeVolume += volume;
            cumulativePriceSquaredVolume += price * priceVolume;
            barCount++;

            double vwap = cumulativePriceVolume / cumulativeVolume;
            SetValue(vwap, LINE_VWAP);

            if (ShowStdDevBands && barCount > 1)
            {
                double variance = (cumulativePriceSquaredVolume / cumulativeVolume) - (vwap * vwap);

                if (variance < 0)
                    variance = 0;

                double stdDev = Math.Sqrt(variance);
                double upper2Sigma = vwap + (stdDev * 2.0);
                double lower2Sigma = vwap - (stdDev * 2.0);

                SetValue(upper2Sigma, LINE_UPPER_STDDEV);
                SetValue(lower2Sigma, LINE_LOWER_STDDEV);
            }
            else
            {
                SetValue(double.NaN, LINE_UPPER_STDDEV);
                SetValue(double.NaN, LINE_LOWER_STDDEV);
            }

            if (ShowMPDBands && barCount > 1 && sessionHigh > sessionLow)
            {
                double mpd = (sessionHigh - sessionLow) / 2.0;
                double upperMPD = vwap + mpd;
                double lowerMPD = vwap - mpd;

                SetValue(upperMPD, LINE_UPPER_MPD);
                SetValue(lowerMPD, LINE_LOWER_MPD);
            }
            else
            {
                SetValue(double.NaN, LINE_UPPER_MPD);
                SetValue(double.NaN, LINE_LOWER_MPD);
            }
        }

        protected override void OnClear()
        {
            cumulativePriceVolume = 0.0;
            cumulativeVolume = 0.0;
            cumulativePriceSquaredVolume = 0.0;
            barCount = 0;
            sessionHigh = double.MinValue;
            sessionLow = double.MaxValue;
        }

        #endregion

        #region Helper Methods

        private void SetValueToAllLines(double value)
        {
            for (int i = 0; i < 5; i++)
            {
                SetValue(value, i);
            }
        }

        #endregion

        #region Public Methods

        public double GetVWAP(int offset = 0)
        {
            return GetValue(offset, LINE_VWAP);
        }

        public double GetUpperStdDev(int offset = 0)
        {
            return GetValue(offset, LINE_UPPER_STDDEV);
        }

        public double GetLowerStdDev(int offset = 0)
        {
            return GetValue(offset, LINE_LOWER_STDDEV);
        }

        public double GetUpperMPD(int offset = 0)
        {
            return GetValue(offset, LINE_UPPER_MPD);
        }

        public double GetLowerMPD(int offset = 0)
        {
            return GetValue(offset, LINE_LOWER_MPD);
        }

        #endregion
    }
}