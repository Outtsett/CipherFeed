/*
 * ============================================================================
 * METRICS PROVIDER
 * ============================================================================
 * 
 * Prometheus/OpenTelemetry metrics integration for Quantower Strategy Metrics
 * panel. Provides observable gauges for real-time monitoring of session open
 * prices across all 8 symbols.
 * 
 * EXPOSED METRICS (8 gauges):
 *   - MNQ_session_open: Micro Nasdaq-100 session open price
 *   - MES_session_open: Micro E-mini S&P 500 session open price
 *   - M2K_session_open: Micro E-mini Russell 2000 session open price
 *   - MYM_session_open: Micro E-mini Dow session open price
 *   - ENQ_session_open: E-mini Nasdaq-100 session open price
 *   - EP_session_open: E-mini S&P 500 session open price
 *   - RTY_session_open: E-mini Russell 2000 session open price
 *   - YM_session_open: E-mini Dow session open price
 * 
 * ============================================================================
 * QUANTOWER API REFERENCES
 * ============================================================================
 * 
 * Metrics Framework:
 *   - Strategy.OnInitializeMetrics(): quantower-api\Strategies\Strategy.md
 *     URL: https://api.quantower.com/docs/TradingPlatform.BusinessLayer.Strategy.html
 *     Method: OnInitializeMetrics(Meter meter)
 *     Purpose: Override to register custom metrics with Quantower
 *     Called: Once during strategy initialization
 * 
 * System.Diagnostics.Metrics API:
 *   - Meter Class: https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.metrics.meter
 *     Purpose: Container for instrument instances (gauges, counters, histograms)
 *     Usage: meter.CreateObservableGauge(...)
 * 
 *   - ObservableGauge<T>: https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.metrics.meter.createobservablegauge
 *     Purpose: Metric that reports value when observed (pull-based)
 *     Pattern: CreateObservableGauge(name, observeValue, unit, description)
 *     Usage: Returns current value from delegate when Quantower polls
 * 
 * Quantower Metrics Panel:
 *   - Location: Quantower UI ? Strategy ? Metrics panel
 *   - Display: Real-time gauge values updated at panel refresh rate
 *   - Export: Can be exported to external monitoring (Prometheus/Grafana)
 * 
 * Symbol Access:
 *   - Symbol Class: quantower-api\Core\BusinessObjects\Symbol.md
 *     URL: https://api.quantower.com/docs/TradingPlatform.BusinessLayer.Symbol.html
 *     Purpose: Symbol reference retrieved via getSymbol delegate
 * 
 * ============================================================================
 * DELEGATE PATTERN
 * ============================================================================
 * 
 * Why Delegates?
 *   MetricsProvider is created in CipherFeed.OnCreated(), but at that point
 *   the strategy's data structures (symbolManager, sessionOpenPrices) don't
 *   exist yet. Delegates allow late binding - metrics pull data when polled.
 * 
 * Delegate 1: getSymbol
 *   Signature: Func<string, (Symbol symbol, bool found)>
 *   Purpose: Retrieve symbol by root name ("MNQ" ? Symbol instance)
 *   Bound to: symbolManager.TryGetSymbol()
 *   Returns: Tuple (symbol, found) to avoid exceptions
 * 
 * Delegate 2: getSessionOpen
 *   Signature: Func<Symbol, (double price, bool found)>
 *   Purpose: Retrieve session open price for a symbol
 *   Bound to: sessionOpenPrices dictionary lookup
 *   Returns: Tuple (price, found) to handle missing keys
 * 
 * Execution Flow:
 *   1. Quantower Metrics panel polls gauge
 *   2. Observable lambda fires: () => { getSymbol("MNQ") }
 *   3. getSymbol delegate calls symbolManager.TryGetSymbol("MNQ", out symbol)
 *   4. If found, getSessionOpen delegate looks up sessionOpenPrices[symbol]
 *   5. If found, return price rounded to 2 decimals
 *   6. If not found at any step, return 0.0
 * 
 * ============================================================================
 * METRIC NAMING CONVENTION
 * ============================================================================
 * 
 * Format: {SYMBOL}_session_open
 * Unit: "price" (Quantower recognizes this for formatting)
 * Description: Human-readable explanation for Metrics panel
 * 
 * Examples:
 *   MNQ_session_open: "Session open price for MNQ (Micro Nasdaq-100)"
 *   MES_session_open: "Session open price for MES (Micro E-mini S&P 500)"
 * 
 * Grouping:
 *   - InitializeNasdaqMetrics(): MNQ, ENQ
 *   - InitializeSP500Metrics(): MES, EP
 *   - InitializeRussell2000Metrics(): M2K, RTY
 *   - InitializeDowJonesMetrics(): MYM, YM
 * 
 * ============================================================================
 * USAGE IN CIPHERFEED STRATEGY
 * ============================================================================
 * 
 * Created in: CipherFeed.cs::OnCreated()
 *   metricsProvider = new MetricsProvider(
 *       getSymbol: (symbolRoot) => { ... },
 *       getSessionOpen: (symbol) => { ... }
 *   );
 * 
 * Registered in: CipherFeed.cs::OnInitializeMetrics(Meter meter)
 *   metricsProvider?.InitializeMetrics(meter);
 * 
 * Data Flow:
 *   CipherFeed.OnNewLast_ForSymbol()
 *     ? sessionOpenPrices[symbol] = last.Price (first tick)
 *       ? Quantower polls metrics
 *         ? metricsProvider delegates retrieve current values
 *           ? Metrics panel displays updated session opens
 * 
 * Session Changes:
 *   On CipherFeed.OnSessionChanged():
 *     - sessionOpenPrices.Clear() ? metrics return 0.0 temporarily
 *     - First tick of new session ? sessionOpenPrices populated
 *       ? metrics reflect new session opens
 * 
 * ============================================================================
 */

using System;
using System.Collections.Generic;
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
                var (symbol, found) = getSymbol("MNQ");
                if (!found) return 0.0;

                var (price, priceFound) = getSessionOpen(symbol);
                return priceFound ? Math.Round(price, 2) : 0.0;
            }, unit: "price", description: "Session open price for MNQ (Micro Nasdaq-100)");

            _ = meter.CreateObservableGauge("ENQ_session_open", () =>
            {
                var (symbol, found) = getSymbol("ENQ");
                if (!found) return 0.0;

                var (price, priceFound) = getSessionOpen(symbol);
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
                var (symbol, found) = getSymbol("MES");
                if (!found) return 0.0;

                var (price, priceFound) = getSessionOpen(symbol);
                return priceFound ? Math.Round(price, 2) : 0.0;
            }, unit: "price", description: "Session open price for MES (Micro E-mini S&P 500)");

            _ = meter.CreateObservableGauge("EP_session_open", () =>
            {
                var (symbol, found) = getSymbol("EP");
                if (!found) return 0.0;

                var (price, priceFound) = getSessionOpen(symbol);
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
                var (symbol, found) = getSymbol("M2K");
                if (!found) return 0.0;

                var (price, priceFound) = getSessionOpen(symbol);
                return priceFound ? Math.Round(price, 2) : 0.0;
            }, unit: "price", description: "Session open price for M2K (Micro E-mini Russell 2000)");

            _ = meter.CreateObservableGauge("RTY_session_open", () =>
            {
                var (symbol, found) = getSymbol("RTY");
                if (!found) return 0.0;

                var (price, priceFound) = getSessionOpen(symbol);
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
                var (symbol, found) = getSymbol("MYM");
                if (!found) return 0.0;

                var (price, priceFound) = getSessionOpen(symbol);
                return priceFound ? Math.Round(price, 2) : 0.0;
            }, unit: "price", description: "Session open price for MYM (Micro E-mini Dow)");

            _ = meter.CreateObservableGauge("YM_session_open", () =>
            {
                var (symbol, found) = getSymbol("YM");
                if (!found) return 0.0;

                var (price, priceFound) = getSessionOpen(symbol);
                return priceFound ? Math.Round(price, 2) : 0.0;
            }, unit: "price", description: "Session open price for YM (E-mini Dow)");
        }
    }
}
