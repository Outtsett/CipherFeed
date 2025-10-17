using System;
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
        private double sessionOpen;

        // Calculated values (in percentage space)
        private double twap;
        private double upperStdDev;
        private double lowerStdDev;

        #endregion

        #region Constructor

        public TWAPIndicator()
        {
            Name = "TWAP Indicator";
            Description = "Time-Weighted Average Price with 2σ Std Dev bands (in percentage space)";
            SeparateWindow = false;
        }

        #endregion

        #region Lifecycle Methods

        protected override void OnInit()
        {
            cumulativePrice = 0.0;
            cumulativePriceSquared = 0.0;
            barCount = 0;
            sessionOpen = 0.0;
        }

        protected override void OnUpdate(UpdateArgs args)
        {
            if (Count < 1)
            {
                return;
            }

            double high = High();
            double low = Low();
            double close = Close();

            // Set session open on first bar
            if (barCount == 0)
            {
                sessionOpen = UseTypicalPrice ? (high + low + close) / 3.0 : close;
            }

            if (sessionOpen == 0.0 || double.IsNaN(sessionOpen))
            {
                twap = double.NaN;
                upperStdDev = double.NaN;
                lowerStdDev = double.NaN;
                return;
            }

            if (double.IsNaN(high) || double.IsNaN(low))
            {
                twap = double.NaN;
                upperStdDev = double.NaN;
                lowerStdDev = double.NaN;
                return;
            }

            double price = UseTypicalPrice ? (high + low + close) / 3.0 : close;
            if (double.IsNaN(price) || price <= 0)
            {
                twap = double.NaN;
                upperStdDev = double.NaN;
                lowerStdDev = double.NaN;
                return;
            }

            // Convert price to percentage from session open
            double pricePct = (price - sessionOpen) / sessionOpen;

            // TWAP calculation in percentage space
            cumulativePrice += pricePct;
            cumulativePriceSquared += pricePct * pricePct;
            barCount++;

            twap = cumulativePrice / barCount;

            if (ShowStdDevBands && barCount > 1)
            {
                double variance = (cumulativePriceSquared / barCount) - (twap * twap);

                if (variance < 0)
                {
                    variance = 0;
                }

                double stdDev = Math.Sqrt(variance);
                upperStdDev = twap + (stdDev * 2.0);
                lowerStdDev = twap - (stdDev * 2.0);
            }
            else
            {
                upperStdDev = double.NaN;
                lowerStdDev = double.NaN;
            }
        }

        protected override void OnClear()
        {
            cumulativePrice = 0.0;
            cumulativePriceSquared = 0.0;
            barCount = 0;
            sessionOpen = 0.0;
        }

        #endregion

        #region Public Methods

        public double GetTWAP(int offset = 0)
        {
            return twap;
        }

        public double GetUpperStdDev(int offset = 0)
        {
            return upperStdDev;
        }

        public double GetLowerStdDev(int offset = 0)
        {
            return lowerStdDev;
        }

        public double GetSessionOpen()
        {
            return sessionOpen;
        }

        #endregion
    }
}