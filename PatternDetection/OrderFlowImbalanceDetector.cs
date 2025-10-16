using System;
using System.Collections.Generic;
using System.Linq;
using TradingPlatform.BusinessLayer;

namespace CipherFeed.PatternDetection
{
    /// <summary>
    /// Order flow imbalance analysis (10-level DOM aggregation)
    /// </summary>
    public class OrderFlowImbalance
    {
        public DateTime Timestamp { get; set; }
        public double CurrentPrice { get; set; }
        public double BidLiquidityScore { get; set; }
        public double AskLiquidityScore { get; set; }
        public double NetLiquidity { get; set; }
        public double Imbalance3Level { get; set; }
        public double Imbalance5Level { get; set; }
        public double Imbalance10Level { get; set; }
        public double AggregateImbalance { get; set; }
        public double BidGradient { get; set; }
        public double AskGradient { get; set; }
        public bool BidStackingDetected { get; set; }
        public bool AskStackingDetected { get; set; }
        public string StackingType { get; set; }
        public double BidVolatility { get; set; }
        public double AskVolatility { get; set; }
        public string UnstableSide { get; set; }
        public List<AbsorptionLevel> BidAbsorptionLevels { get; set; } = [];
        public List<AbsorptionLevel> AskAbsorptionLevels { get; set; } = [];
        public double NetAbsorption { get; set; }
        public double SpreadAtBest { get; set; }
        public double AverageSpread { get; set; }
        public double SpreadCompression { get; set; }
        public string SpreadCondition { get; set; }
        public string Direction { get; set; }
        public int Confidence { get; set; }
        public double SignalStrength { get; set; }
    }

    /// <summary>
    /// Absorption level (institutional size concentration)
    /// </summary>
    public class AbsorptionLevel
    {
        public double Price { get; set; }
        public double Size { get; set; }
        public double AbsorptionRatio { get; set; }
        public string Side { get; set; }
    }

    /// <summary>
    /// Detects order flow imbalance using 10-level DOM aggregation
    /// Analyzes weighted liquidity, depth imbalance, gradients, volatility, absorption, and spreads
    /// </summary>
    public class OrderFlowImbalanceDetector
    {
        #region Configuration

        private const int DOM_DEPTH_LEVELS = 10;                    // Analyze top 10 levels each side
        private const double ABSORPTION_SIZE_MULTIPLIER = 3.0;      // Size > 3x average = absorption
        private const double IMBALANCE_THRESHOLD = 0.15;            // 15% imbalance triggers signal
        private const double GRADIENT_THRESHOLD = 0.10;             // 10% gradient for stacking detection

        #endregion

        #region Detection Methods

        /// <summary>
        /// Detect order flow imbalance using 10-level DOM aggregation
        /// </summary>
        public static OrderFlowImbalance Detect(Symbol symbol, DOMQuote dom)
        {
            if (dom == null || dom.Bids == null || dom.Asks == null)
            {
                return null;
            }

            // Get top 10 levels each side
            List<Level2Quote> bids = dom.Bids.OrderByDescending(b => b.Price).Take(DOM_DEPTH_LEVELS).ToList();
            List<Level2Quote> asks = dom.Asks.OrderBy(a => a.Price).Take(DOM_DEPTH_LEVELS).ToList();

            if (bids.Count < 3 || asks.Count < 3)
            {
                return null;
            }

            OrderFlowImbalance imbalance = new()
            {
                Timestamp = DateTime.UtcNow,
                CurrentPrice = symbol.Last
            };

            // Calculate all metrics
            CalculateWeightedLiquidity(bids, asks, imbalance);
            CalculateDepthImbalance(bids, asks, imbalance);
            CalculateLiquidityGradient(bids, asks, imbalance);
            CalculateLiquidityVolatility(bids, asks, imbalance);
            CalculateAbsorptionLevels(bids, asks, imbalance);
            CalculateSpreadAnalysis(bids, asks, imbalance);
            CalculateImbalanceSignal(imbalance);

            return imbalance;
        }

        #endregion

        #region Private Calculation Methods

        private static void CalculateWeightedLiquidity(List<Level2Quote> bids, List<Level2Quote> asks, OrderFlowImbalance imbalance)
        {
            double bidLiquidity = 0;
            double askLiquidity = 0;

            for (int i = 0; i < Math.Min(bids.Count, DOM_DEPTH_LEVELS); i++)
            {
                double weight = (DOM_DEPTH_LEVELS + 1.0 - (i + 1)) / 55.0;
                bidLiquidity += bids[i].Size * weight;
            }

            for (int i = 0; i < Math.Min(asks.Count, DOM_DEPTH_LEVELS); i++)
            {
                double weight = (DOM_DEPTH_LEVELS + 1.0 - (i + 1)) / 55.0;
                askLiquidity += asks[i].Size * weight;
            }

            imbalance.BidLiquidityScore = bidLiquidity;
            imbalance.AskLiquidityScore = askLiquidity;

            double totalLiquidity = bidLiquidity + askLiquidity;
            if (totalLiquidity > 0)
            {
                imbalance.NetLiquidity = (bidLiquidity - askLiquidity) / totalLiquidity;
            }
        }

        private static void CalculateDepthImbalance(List<Level2Quote> bids, List<Level2Quote> asks, OrderFlowImbalance imbalance)
        {
            int[] depths = { 3, 5, 10 };
            double[] weights = { 0.5, 0.3, 0.2 };
            double aggregateImbalance = 0;

            for (int d = 0; d < depths.Length; d++)
            {
                int depth = Math.Min(depths[d], Math.Min(bids.Count, asks.Count));

                double bidSum = bids.Take(depth).Sum(b => b.Size);
                double askSum = asks.Take(depth).Sum(a => a.Size);
                double total = bidSum + askSum;

                if (total > 0)
                {
                    double imbalance_slice = (bidSum - askSum) / total;
                    aggregateImbalance += imbalance_slice * weights[d];

                    if (depth == 3)
                    {
                        imbalance.Imbalance3Level = imbalance_slice;
                    }
                    else if (depth == 5)
                    {
                        imbalance.Imbalance5Level = imbalance_slice;
                    }
                    else if (depth == 10)
                    {
                        imbalance.Imbalance10Level = imbalance_slice;
                    }
                }
            }

            imbalance.AggregateImbalance = aggregateImbalance;
        }

        private static void CalculateLiquidityGradient(List<Level2Quote> bids, List<Level2Quote> asks, OrderFlowImbalance imbalance)
        {
            if (bids.Count >= 2 && asks.Count >= 2)
            {
                imbalance.BidGradient = (bids[^1].Size - bids[0].Size) / (bids.Count - 1);
                imbalance.AskGradient = (asks[^1].Size - asks[0].Size) / (asks.Count - 1);

                if (imbalance.BidGradient > GRADIENT_THRESHOLD)
                {
                    imbalance.BidStackingDetected = true;
                    imbalance.StackingType = "Bid Stacking (Bullish Support)";
                }
                else if (imbalance.AskGradient > GRADIENT_THRESHOLD)
                {
                    imbalance.AskStackingDetected = true;
                    imbalance.StackingType = "Ask Stacking (Bearish Resistance)";
                }
            }
        }

        private static void CalculateLiquidityVolatility(List<Level2Quote> bids, List<Level2Quote> asks, OrderFlowImbalance imbalance)
        {
            if (bids.Count > 1)
            {
                double avgBidSize = bids.Average(b => b.Size);
                double bidVariance = bids.Sum(b => Math.Pow(b.Size - avgBidSize, 2)) / bids.Count;
                imbalance.BidVolatility = Math.Sqrt(bidVariance);
            }

            if (asks.Count > 1)
            {
                double avgAskSize = asks.Average(a => a.Size);
                double askVariance = asks.Sum(a => Math.Pow(a.Size - avgAskSize, 2)) / asks.Count;
                imbalance.AskVolatility = Math.Sqrt(askVariance);
            }

            imbalance.UnstableSide = imbalance.BidVolatility > 2 * imbalance.AskVolatility
                ? "Bid (Support Failure Risk)"
                : imbalance.AskVolatility > 2 * imbalance.BidVolatility ? "Ask (Resistance Failure Risk)" : "Stable";
        }

        private static void CalculateAbsorptionLevels(List<Level2Quote> bids, List<Level2Quote> asks, OrderFlowImbalance imbalance)
        {
            double avgBidSize = bids.Average(b => b.Size);
            double avgAskSize = asks.Average(a => a.Size);

            imbalance.BidAbsorptionLevels = bids
                .Where(b => b.Size > avgBidSize * ABSORPTION_SIZE_MULTIPLIER)
                .Select(b => new AbsorptionLevel
                {
                    Price = b.Price,
                    Size = b.Size,
                    AbsorptionRatio = b.Size / avgBidSize,
                    Side = "Bid"
                })
                .ToList();

            imbalance.AskAbsorptionLevels = asks
                .Where(a => a.Size > avgAskSize * ABSORPTION_SIZE_MULTIPLIER)
                .Select(a => new AbsorptionLevel
                {
                    Price = a.Price,
                    Size = a.Size,
                    AbsorptionRatio = a.Size / avgAskSize,
                    Side = "Ask"
                })
                .ToList();

            double totalBidAbsorption = imbalance.BidAbsorptionLevels.Sum(a => a.AbsorptionRatio);
            double totalAskAbsorption = imbalance.AskAbsorptionLevels.Sum(a => a.AbsorptionRatio);
            imbalance.NetAbsorption = totalBidAbsorption - totalAskAbsorption;
        }

        private static void CalculateSpreadAnalysis(List<Level2Quote> bids, List<Level2Quote> asks, OrderFlowImbalance imbalance)
        {
            if (bids.Count == 0 || asks.Count == 0)
            {
                return;
            }

            List<double> spreads = [];
            int maxDepth = Math.Min(bids.Count, asks.Count);

            for (int i = 0; i < maxDepth; i++)
            {
                double spread = asks[i].Price - bids[bids.Count - 1 - i].Price;
                spreads.Add(spread);
            }

            if (spreads.Count > 0)
            {
                imbalance.AverageSpread = spreads.Average();
                imbalance.SpreadAtBest = asks[0].Price - bids[0].Price;

                if (spreads.Count > 1 && imbalance.SpreadAtBest > 0)
                {
                    double worstSpread = spreads.Max();
                    imbalance.SpreadCompression = (imbalance.SpreadAtBest - worstSpread) / imbalance.SpreadAtBest;

                    imbalance.SpreadCondition = imbalance.SpreadCompression > 0.5
                        ? "Tight Market (Low Volatility Expected)"
                        : imbalance.SpreadCompression < 0 ? "Fragmented Liquidity (High Volatility Expected)" : "Normal";
                }
            }
        }

        private static void CalculateImbalanceSignal(OrderFlowImbalance imbalance)
        {
            int confidence = 50;
            double signal = 0;

            if (Math.Abs(imbalance.NetLiquidity) > IMBALANCE_THRESHOLD)
            {
                signal += imbalance.NetLiquidity * 30;
                confidence += 15;
            }

            if (Math.Abs(imbalance.AggregateImbalance) > IMBALANCE_THRESHOLD)
            {
                signal += imbalance.AggregateImbalance * 30;
                confidence += 15;
            }

            if (imbalance.BidStackingDetected)
            {
                signal += 20;
                confidence += 10;
            }
            else if (imbalance.AskStackingDetected)
            {
                signal -= 20;
                confidence += 10;
            }

            if (Math.Abs(imbalance.NetAbsorption) > 2.0)
            {
                signal += Math.Sign(imbalance.NetAbsorption) * 20;
                confidence += 10;
            }

            if (imbalance.UnstableSide.Contains("Bid"))
            {
                signal -= 10;
            }
            else if (imbalance.UnstableSide.Contains("Ask"))
            {
                signal += 10;
            }

            if (signal > 10)
            {
                imbalance.Direction = "Bullish";
                imbalance.Confidence = Math.Min(confidence, 100);
            }
            else if (signal < -10)
            {
                imbalance.Direction = "Bearish";
                imbalance.Confidence = Math.Min(confidence, 100);
            }
            else
            {
                imbalance.Direction = "Neutral";
                imbalance.Confidence = Math.Max(confidence - 20, 30);
            }

            imbalance.SignalStrength = Math.Abs(signal);
        }

        #endregion
    }
}
