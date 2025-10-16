using System;
using System.Drawing;
using TradingPlatform.BusinessLayer;

namespace CipherFeed.Indicators
{
    public class TWAPIndicator : Indicator
    {
        #region Input Parameters

        [InputParameter("Use Typical Price", 0)]
        public bool UseTypicalPrice { get; set; } = true;

        [InputParameter("Show Standard Deviation Bands", 1)]
        public bool ShowStdDevBands { get; set; } = true;

        #endregion

        #region Private Fields

        private double cumulativePrice;
        private double cumulativePriceSquared;
        private int barCount;

        private const int LINE_TWAP = 0;
        private const int LINE_UPPER_STDDEV = 1;
        private const int LINE_LOWER_STDDEV = 2;

        #endregion

        #region Constructor

        public TWAPIndicator()
        {
            Name = "TWAP Indicator";
            Description = "Time-Weighted Average Price with 2σ Std Dev bands";
            SeparateWindow = false;

            AddLineSeries("TWAP", Color.Purple, 2, LineStyle.Solid);
            AddLineSeries("Upper Std Dev 2σ", Color.Magenta, 1, LineStyle.Solid);
            AddLineSeries("Lower Std Dev 2σ", Color.Magenta, 1, LineStyle.Solid);
        }

        #endregion

        #region Lifecycle Methods

        protected override void OnInit()
        {
            cumulativePrice = 0.0;
            cumulativePriceSquared = 0.0;
            barCount = 0;
        }

        protected override void OnUpdate(UpdateArgs args)
        {
            if (Count < 1)
                return;

            double high = High();
            double low = Low();

            if (double.IsNaN(high) || double.IsNaN(low))
            {
                SetValueToAllLines(double.NaN);
                return;
            }

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

            cumulativePrice += price;
            cumulativePriceSquared += price * price;
            barCount++;

            double twap = cumulativePrice / barCount;
            SetValue(twap, LINE_TWAP);

            if (ShowStdDevBands && barCount > 1)
            {
                double variance = (cumulativePriceSquared / barCount) - (twap * twap);

                if (variance < 0)
                    variance = 0;

                double stdDev = Math.Sqrt(variance);
                double upper2Sigma = twap + (stdDev * 2.0);
                double lower2Sigma = twap - (stdDev * 2.0);

                SetValue(upper2Sigma, LINE_UPPER_STDDEV);
                SetValue(lower2Sigma, LINE_LOWER_STDDEV);
            }
            else
            {
                SetValue(double.NaN, LINE_UPPER_STDDEV);
                SetValue(double.NaN, LINE_LOWER_STDDEV);
            }
        }

        protected override void OnClear()
        {
            cumulativePrice = 0.0;
            cumulativePriceSquared = 0.0;
            barCount = 0;
        }

        #endregion

        #region Helper Methods

        private void SetValueToAllLines(double value)
        {
            for (int i = 0; i < 3; i++)
            {
                SetValue(value, i);
            }
        }

        #endregion

        #region Public Methods

        public double GetTWAP(int offset = 0)
        {
            return GetValue(offset, LINE_TWAP);
        }

        public double GetUpperStdDev(int offset = 0)
        {
            return GetValue(offset, LINE_UPPER_STDDEV);
        }

        public double GetLowerStdDev(int offset = 0)
        {
            return GetValue(offset, LINE_LOWER_STDDEV);
        }

        #endregion
    }
}