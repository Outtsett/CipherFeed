using System;
using System.Diagnostics.Metrics;
using TradingPlatform.BusinessLayer;

namespace CipherFeed.Core
{
    /// <summary>
    /// Manages Prometheus/OpenTelemetry metrics for CipherFeed strategy
    /// Provides session open prices and other observable metrics for external monitoring
    /// </summary>
    public class MetricsProvider
    {
        private readonly Func<string, (Symbol symbol, bool found)> getSymbol;
        private readonly Func<Symbol, (double price, bool found)> getSessionOpen;

        /// <summary>
        /// Initialize metrics provider with delegates to access strategy data
        /// </summary>
        /// <param name="getSymbol">Delegate to get symbol by root name</param>
        /// <param name="getSessionOpen">Delegate to get session open price for a symbol</param>
        public MetricsProvider(
            Func<string, (Symbol symbol, bool found)> getSymbol,
            Func<Symbol, (double price, bool found)> getSessionOpen)
        {
            this.getSymbol = getSymbol;
            this.getSessionOpen = getSessionOpen;
        }

        /// <summary>
        /// Initialize all metrics on the provided meter
        /// </summary>
        public void InitializeMetrics(Meter meter)
        {
            InitializeNasdaqMetrics(meter);
            InitializeSP500Metrics(meter);
            InitializeRussell2000Metrics(meter);
            InitializeDowJonesMetrics(meter);
        }

        /// <summary>
        /// Initialize Nasdaq-100 futures metrics
        /// </summary>
        private void InitializeNasdaqMetrics(Meter meter)
        {
            _ = meter.CreateObservableGauge("MNQ_session_open", () =>
            {
                (Symbol symbol, bool found) = getSymbol("MNQ");
                if (!found)
                {
                    return 0.0;
                }

                (double price, bool priceFound) = getSessionOpen(symbol);
                return priceFound ? Math.Round(price, 2) : 0.0;
            }, unit: "price", description: "Session open price for MNQ (Micro Nasdaq-100)");

            _ = meter.CreateObservableGauge("ENQ_session_open", () =>
            {
                (Symbol symbol, bool found) = getSymbol("ENQ");
                if (!found)
                {
                    return 0.0;
                }

                (double price, bool priceFound) = getSessionOpen(symbol);
                return priceFound ? Math.Round(price, 2) : 0.0;
            }, unit: "price", description: "Session open price for ENQ (E-mini Nasdaq-100)");
        }

        /// <summary>
        /// Initialize S&P 500 futures metrics
        /// </summary>
        private void InitializeSP500Metrics(Meter meter)
        {
            _ = meter.CreateObservableGauge("MES_session_open", () =>
            {
                (Symbol symbol, bool found) = getSymbol("MES");
                if (!found)
                {
                    return 0.0;
                }

                (double price, bool priceFound) = getSessionOpen(symbol);
                return priceFound ? Math.Round(price, 2) : 0.0;
            }, unit: "price", description: "Session open price for MES (Micro E-mini S&P 500)");

            _ = meter.CreateObservableGauge("EP_session_open", () =>
            {
                (Symbol symbol, bool found) = getSymbol("EP");
                if (!found)
                {
                    return 0.0;
                }

                (double price, bool priceFound) = getSessionOpen(symbol);
                return priceFound ? Math.Round(price, 2) : 0.0;
            }, unit: "price", description: "Session open price for EP (E-mini S&P 500)");
        }

        /// <summary>
        /// Initialize Russell 2000 futures metrics
        /// </summary>
        private void InitializeRussell2000Metrics(Meter meter)
        {
            _ = meter.CreateObservableGauge("M2K_session_open", () =>
            {
                (Symbol symbol, bool found) = getSymbol("M2K");
                if (!found)
                {
                    return 0.0;
                }

                (double price, bool priceFound) = getSessionOpen(symbol);
                return priceFound ? Math.Round(price, 2) : 0.0;
            }, unit: "price", description: "Session open price for M2K (Micro E-mini Russell 2000)");

            _ = meter.CreateObservableGauge("RTY_session_open", () =>
            {
                (Symbol symbol, bool found) = getSymbol("RTY");
                if (!found)
                {
                    return 0.0;
                }

                (double price, bool priceFound) = getSessionOpen(symbol);
                return priceFound ? Math.Round(price, 2) : 0.0;
            }, unit: "price", description: "Session open price for RTY (E-mini Russell 2000)");
        }

        /// <summary>
        /// Initialize Dow Jones futures metrics
        /// </summary>
        private void InitializeDowJonesMetrics(Meter meter)
        {
            _ = meter.CreateObservableGauge("MYM_session_open", () =>
            {
                (Symbol symbol, bool found) = getSymbol("MYM");
                if (!found)
                {
                    return 0.0;
                }

                (double price, bool priceFound) = getSessionOpen(symbol);
                return priceFound ? Math.Round(price, 2) : 0.0;
            }, unit: "price", description: "Session open price for MYM (Micro E-mini Dow)");

            _ = meter.CreateObservableGauge("YM_session_open", () =>
            {
                (Symbol symbol, bool found) = getSymbol("YM");
                if (!found)
                {
                    return 0.0;
                }

                (double price, bool priceFound) = getSessionOpen(symbol);
                return priceFound ? Math.Round(price, 2) : 0.0;
            }, unit: "price", description: "Session open price for YM (E-mini Dow)");
        }
    }
}
