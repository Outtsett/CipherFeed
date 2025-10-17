using System;
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
        private double sessionOpen;

        // Calculated values (in percentage space)
        private double vwap;
        private double upperStdDev;
        private double lowerStdDev;
        private double upperMPD;
        private double lowerMPD;

        #endregion

        #region Constructor

        public SessionAnchoredVWAP()
        {
            Name = "Session Anchored VWAP";
            Description = "Volume-Weighted Average Price with 2σ Std Dev and MPD bands (in percentage space)";
            SeparateWindow = false;
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
            sessionOpen = 0.0;
        }

        protected override void OnUpdate(UpdateArgs args)
        {
            if (Count < 1)
            {
                return;
            }

            double volume = Volume();

            if (volume <= 0 || double.IsNaN(volume))
            {
                vwap = double.NaN;
                upperStdDev = double.NaN;
                lowerStdDev = double.NaN;
                upperMPD = double.NaN;
                lowerMPD = double.NaN;
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
                vwap = double.NaN;
                upperStdDev = double.NaN;
                lowerStdDev = double.NaN;
                upperMPD = double.NaN;
                lowerMPD = double.NaN;
                return;
            }

            if (high > sessionHigh)
            {
                sessionHigh = high;
            }

            if (low < sessionLow)
            {
                sessionLow = low;
            }

            double price = UseTypicalPrice ? (high + low + close) / 3.0 : close;
            if (double.IsNaN(price) || price <= 0)
            {
                vwap = double.NaN;
                upperStdDev = double.NaN;
                lowerStdDev = double.NaN;
                upperMPD = double.NaN;
                lowerMPD = double.NaN;
                return;
            }

            // Convert price to percentage from session open
            double pricePct = (price - sessionOpen) / sessionOpen;

            // VWAP calculation in percentage space
            double priceVolume = pricePct * volume;
            cumulativePriceVolume += priceVolume;
            cumulativeVolume += volume;
            cumulativePriceSquaredVolume += pricePct * priceVolume;
            barCount++;

            vwap = cumulativePriceVolume / cumulativeVolume;

            if (ShowStdDevBands && barCount > 1)
            {
                double variance = (cumulativePriceSquaredVolume / cumulativeVolume) - (vwap * vwap);

                if (variance < 0)
                {
                    variance = 0;
                }

                double stdDev = Math.Sqrt(variance);
                upperStdDev = vwap + (stdDev * 2.0);
                lowerStdDev = vwap - (stdDev * 2.0);
            }
            else
            {
                upperStdDev = double.NaN;
                lowerStdDev = double.NaN;
            }

            if (ShowMPDBands && barCount > 1 && sessionHigh > sessionLow)
            {
                // Convert session high/low to percentage space
                double sessionHighPct = (sessionHigh - sessionOpen) / sessionOpen;
                double sessionLowPct = (sessionLow - sessionOpen) / sessionOpen;

                double mpd = (sessionHighPct - sessionLowPct) / 2.0;
                upperMPD = vwap + mpd;
                lowerMPD = vwap - mpd;
            }
            else
            {
                upperMPD = double.NaN;
                lowerMPD = double.NaN;
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
            sessionOpen = 0.0;
        }

        #endregion

        #region Public Methods

        public double GetVWAP(int offset = 0)
        {
            return vwap;
        }

        public double GetUpperStdDev(int offset = 0)
        {
            return upperStdDev;
        }

        public double GetLowerStdDev(int offset = 0)
        {
            return lowerStdDev;
        }

        public double GetUpperMPD(int offset = 0)
        {
            return upperMPD;
        }

        public double GetLowerMPD(int offset = 0)
        {
            return lowerMPD;
        }

        public double GetSessionOpen()
        {
            return sessionOpen;
        }

        #endregion
    }
}