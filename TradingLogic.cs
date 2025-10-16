using CipherFeed.PatternDetection;
using System;
using System.Collections.Generic;
using System.Linq;
using TradingPlatform.BusinessLayer;

namespace CipherFeed
{
    /// <summary>
    /// Contains trading execution and exit logic based on market profile indicators and relative price analysis
    /// </summary>
    public class TradingLogic
    {
        #region Configuration

        // Entry thresholds
        private const double VWAP_ENTRY_THRESHOLD_PERCENT = 0.15;      // Enter when 0.15% away from VWAP
        private const double VALUE_AREA_ENTRY_THRESHOLD_PERCENT = 0.10; // Enter near VAH/VAL boundaries
        private const double VOLATILITY_BAND_WIDTH_MIN = 0.20;          // Minimum band width for entries
        private const double VOLATILITY_BAND_WIDTH_MAX = 1.50;          // Maximum band width for entries

        // Exit thresholds
        private const double VWAP_EXIT_TARGET_PERCENT = 0.30;           // Exit when profit target reached
        private const double STOP_LOSS_PERCENT = 0.20;                  // Stop loss threshold
        private const double TRAILING_STOP_PERCENT = 0.15;              // Trailing stop distance

        #endregion

        #region Entry Signal Detection

        /// <summary>
        /// Evaluate if there's a valid entry signal for the symbol
        /// </summary>
        public static EntrySignal EvaluateEntrySignal(Symbol symbol, string symbolName, MarketProfileData mpData)
        {
            if (symbol == null || mpData == null || !mpData.HasSufficientData())
            {
                return null;
            }

            // Check volatility - only trade in optimal volatility range
            if (!IsVolatilityOptimal(mpData))
            {
                return null;
            }

            // Evaluate different entry strategies
            // 1. VWAP Mean Reversion Strategy
            EntrySignal signal = EvaluateVWAPMeanReversion(symbol, mpData);
            if (signal != null)
            {
                return signal;
            }

            // 2. Value Area Reversal Strategy
            signal = EvaluateValueAreaReversal(symbol, mpData);
            if (signal != null)
            {
                return signal;
            }

            // 3. VPOC Support/Resistance Strategy
            signal = EvaluateVPOCBounce(symbol, mpData);
            if (signal != null)
            {
                return signal;
            }

            // 4. Band Breakout Strategy
            signal = EvaluateBandBreakout(symbol, mpData);
            return signal ?? null;
        }

        /// <summary>
        /// Check if volatility is in optimal range for trading
        /// </summary>
        private static bool IsVolatilityOptimal(MarketProfileData mpData)
        {
            // Check VWAP band width
            return mpData.VWAPStdDevPercent is >= VOLATILITY_BAND_WIDTH_MIN and
                <= VOLATILITY_BAND_WIDTH_MAX;
        }

        /// <summary>
        /// VWAP Mean Reversion: Enter when price deviates from VWAP and approaches bands
        /// </summary>
        private static EntrySignal EvaluateVWAPMeanReversion(Symbol symbol, MarketProfileData mpData)
        {
            double currentPrice = symbol.Last;
            double vwapDistance = mpData.GetDistanceFromVWAP(currentPrice);
            double vwapPercent = ((mpData.VWAP / currentPrice) - 1) * 100;

            // BUY Signal: Price at lower VWAP band, expect reversion to VWAP
            if (currentPrice <= mpData.VWAPLowerStdDev &&
                Math.Abs(vwapDistance) >= VWAP_ENTRY_THRESHOLD_PERCENT &&
                vwapDistance < 0) // Price is below VWAP
            {
                return new EntrySignal
                {
                    Side = Side.Buy,
                    Strategy = "VWAP Mean Reversion",
                    Reason = $"Price at lower VWAP band ({vwapDistance:F2}%), expect reversion to VWAP",
                    ConfidenceScore = CalculateConfidenceScore(mpData, vwapDistance, true),
                    TargetPercent = vwapPercent, // Target is VWAP
                    StopLossPercent = -STOP_LOSS_PERCENT
                };
            }

            // SELL Signal: Price at upper VWAP band, expect reversion to VWAP
            return currentPrice >= mpData.VWAPUpperStdDev &&
                Math.Abs(vwapDistance) >= VWAP_ENTRY_THRESHOLD_PERCENT &&
                vwapDistance > 0
                ? new EntrySignal
                {
                    Side = Side.Sell,
                    Strategy = "VWAP Mean Reversion",
                    Reason = $"Price at upper VWAP band ({vwapDistance:F2}%), expect reversion to VWAP",
                    ConfidenceScore = CalculateConfidenceScore(mpData, vwapDistance, false),
                    TargetPercent = vwapPercent, // Target is VWAP
                    StopLossPercent = STOP_LOSS_PERCENT
                }
                : null;
        }

        /// <summary>
        /// Value Area Reversal: Enter at VAH/VAL boundaries expecting reversal
        /// </summary>
        private static EntrySignal EvaluateValueAreaReversal(Symbol symbol, MarketProfileData mpData)
        {
            double currentPrice = symbol.Last;
            double vahPercent = ((mpData.VAH / currentPrice) - 1) * 100;
            double valPercent = ((mpData.VAL / currentPrice) - 1) * 100;

            // BUY Signal: Price near VAL (support), expect bounce
            if (Math.Abs(valPercent) <= VALUE_AREA_ENTRY_THRESHOLD_PERCENT && currentPrice <= mpData.VAL)
            {
                return new EntrySignal
                {
                    Side = Side.Buy,
                    Strategy = "Value Area Reversal",
                    Reason = $"Price at VAL support ({valPercent:F2}%), expect bounce to VPOC/VAH",
                    ConfidenceScore = CalculateValueAreaConfidence(mpData, true),
                    TargetPercent = ((mpData.VPOC / currentPrice) - 1) * 100,
                    StopLossPercent = -STOP_LOSS_PERCENT
                };
            }

            // SELL Signal: Price near VAH (resistance), expect rejection
            return Math.Abs(vahPercent) <= VALUE_AREA_ENTRY_THRESHOLD_PERCENT && currentPrice >= mpData.VAH
                ? new EntrySignal
                {
                    Side = Side.Sell,
                    Strategy = "Value Area Reversal",
                    Reason = $"Price at VAH resistance ({vahPercent:F2}%), expect rejection to VPOC/VAL",
                    ConfidenceScore = CalculateValueAreaConfidence(mpData, false),
                    TargetPercent = ((mpData.VPOC / currentPrice) - 1) * 100,
                    StopLossPercent = STOP_LOSS_PERCENT
                }
                : null;
        }

        /// <summary>
        /// VPOC Support/Resistance: Enter when price bounces off VPOC
        /// </summary>
        private static EntrySignal EvaluateVPOCBounce(Symbol symbol, MarketProfileData mpData)
        {
            double currentPrice = symbol.Last;
            double vpocPercent = ((mpData.VPOC / currentPrice) - 1) * 100;
            double vpocDistanceThreshold = 0.08; // 0.08% from VPOC

            // Only trade near VPOC
            if (Math.Abs(vpocPercent) > vpocDistanceThreshold)
            {
                return null;
            }

            // BUY Signal: Price at VPOC with bullish bias (price above TWAP)
            if (currentPrice > mpData.TWAP && currentPrice <= mpData.VPOC)
            {
                return new EntrySignal
                {
                    Side = Side.Buy,
                    Strategy = "VPOC Support",
                    Reason = $"Price at VPOC support ({vpocPercent:F2}%) with bullish bias",
                    ConfidenceScore = 70,
                    TargetPercent = ((mpData.VAH / currentPrice) - 1) * 100,
                    StopLossPercent = -STOP_LOSS_PERCENT
                };
            }

            // SELL Signal: Price at VPOC with bearish bias (price below TWAP)
            return currentPrice < mpData.TWAP && currentPrice >= mpData.VPOC
                ? new EntrySignal
                {
                    Side = Side.Sell,
                    Strategy = "VPOC Resistance",
                    Reason = $"Price at VPOC resistance ({vpocPercent:F2}%) with bearish bias",
                    ConfidenceScore = 70,
                    TargetPercent = ((mpData.VAL / currentPrice) - 1) * 100,
                    StopLossPercent = STOP_LOSS_PERCENT
                }
                : null;
        }

        /// <summary>
        /// Band Breakout: Enter when price breaks through VWAP in direction of trend
        /// </summary>
        private static EntrySignal EvaluateBandBreakout(Symbol symbol, MarketProfileData mpData)
        {
            double currentPrice = symbol.Last;
            double vwapPercent = ((mpData.VWAP / currentPrice) - 1) * 100;

            // Check for strong trend: VWAP and TWAP aligned
            bool bullishTrend = mpData.VWAP < mpData.TWAP;
            bool bearishTrend = mpData.VWAP > mpData.TWAP;

            // BUY Signal: Bullish breakout above VWAP in uptrend
            if (bullishTrend && currentPrice > mpData.VWAP &&
                currentPrice < mpData.VWAPUpperStdDev &&
                Math.Abs(vwapPercent) < 0.15) // Just crossed VWAP
            {
                return new EntrySignal
                {
                    Side = Side.Buy,
                    Strategy = "VWAP Breakout",
                    Reason = $"Bullish breakout above VWAP, trending up",
                    ConfidenceScore = 75,
                    TargetPercent = ((mpData.VWAPUpperStdDev / currentPrice) - 1) * 100,
                    StopLossPercent = vwapPercent - STOP_LOSS_PERCENT
                };
            }

            // SELL Signal: Bearish breakdown below VWAP in downtrend
            return bearishTrend && currentPrice < mpData.VWAP &&
                currentPrice > mpData.VWAPLowerStdDev &&
                Math.Abs(vwapPercent) < 0.15
                ? new EntrySignal
                {
                    Side = Side.Sell,
                    Strategy = "VWAP Breakdown",
                    Reason = $"Bearish breakdown below VWAP, trending down",
                    ConfidenceScore = 75,
                    TargetPercent = ((mpData.VWAPLowerStdDev / currentPrice) - 1) * 100,
                    StopLossPercent = vwapPercent + STOP_LOSS_PERCENT
                }
                : null;
        }

        #endregion

        #region Exit Signal Detection

        /// <summary>
        /// Evaluate if position should be exited
        /// </summary>
        public static ExitSignal EvaluateExitSignal(Symbol symbol, Position position, MarketProfileData mpData, double entryPrice)
        {
            if (symbol == null || position == null || mpData == null)
            {
                return null;
            }

            double currentPrice = symbol.Last;
            double pnlPercent = position.Side == Side.Buy
                ? ((currentPrice / entryPrice) - 1) * 100
                : ((entryPrice / currentPrice) - 1) * 100;

            // 1. Check stop loss
            if (pnlPercent <= -STOP_LOSS_PERCENT)
            {
                return new ExitSignal
                {
                    Reason = $"Stop Loss Hit: {pnlPercent:F2}%",
                    ExitType = ExitType.StopLoss,
                    Urgency = ExitUrgency.Immediate
                };
            }

            // 2. Check profit target
            if (pnlPercent >= VWAP_EXIT_TARGET_PERCENT)
            {
                return new ExitSignal
                {
                    Reason = $"Profit Target Reached: {pnlPercent:F2}%",
                    ExitType = ExitType.TakeProfit,
                    Urgency = ExitUrgency.Normal
                };
            }

            // 3. Check band-based exits
            ExitSignal bandExit = CheckBandExits(symbol, position, mpData, currentPrice);
            if (bandExit != null)
            {
                return bandExit;
            }

            // 4. Check reversal signals
            ExitSignal reversalExit = CheckReversalSignals(symbol, position, mpData, currentPrice);
            if (reversalExit != null)
            {
                return reversalExit;
            }

            // 5. Trailing stop
            if (pnlPercent >= TRAILING_STOP_PERCENT)
            {
                _ = pnlPercent - TRAILING_STOP_PERCENT;
                // This would need state tracking to implement properly
            }

            return null;
        }

        /// <summary>
        /// Check if price has reached band extremes
        /// </summary>
        private static ExitSignal CheckBandExits(Symbol symbol, Position position, MarketProfileData mpData, double currentPrice)
        {
            // Exit long at upper VWAP band
            if (position.Side == Side.Buy && currentPrice >= mpData.VWAPUpperStdDev)
            {
                return new ExitSignal
                {
                    Reason = "Price reached upper VWAP 2? band",
                    ExitType = ExitType.TakeProfit,
                    Urgency = ExitUrgency.Normal
                };
            }

            // Exit short at lower VWAP band
            if (position.Side == Side.Sell && currentPrice <= mpData.VWAPLowerStdDev)
            {
                return new ExitSignal
                {
                    Reason = "Price reached lower VWAP 2? band",
                    ExitType = ExitType.TakeProfit,
                    Urgency = ExitUrgency.Normal
                };
            }

            // Exit long at VAH
            if (position.Side == Side.Buy && currentPrice >= mpData.VAH)
            {
                return new ExitSignal
                {
                    Reason = "Price reached Value Area High (VAH)",
                    ExitType = ExitType.TakeProfit,
                    Urgency = ExitUrgency.Normal
                };
            }

            // Exit short at VAL
            return position.Side == Side.Sell && currentPrice <= mpData.VAL
                ? new ExitSignal
                {
                    Reason = "Price reached Value Area Low (VAL)",
                    ExitType = ExitType.TakeProfit,
                    Urgency = ExitUrgency.Normal
                }
                : null;
        }

        /// <summary>
        /// Check for reversal signals that warrant exit
        /// </summary>
        private static ExitSignal CheckReversalSignals(Symbol symbol, Position position, MarketProfileData mpData, double currentPrice)
        {
            // Exit long if price crosses below VPOC with bearish momentum
            if (position.Side == Side.Buy && currentPrice < mpData.VPOC && currentPrice < mpData.TWAP)
            {
                return new ExitSignal
                {
                    Reason = "Bearish reversal: Price below VPOC and TWAP",
                    ExitType = ExitType.Reversal,
                    Urgency = ExitUrgency.High
                };
            }

            // Exit short if price crosses above VPOC with bullish momentum
            if (position.Side == Side.Sell && currentPrice > mpData.VPOC && currentPrice > mpData.TWAP)
            {
                return new ExitSignal
                {
                    Reason = "Bullish reversal: Price above VPOC and TWAP",
                    ExitType = ExitType.Reversal,
                    Urgency = ExitUrgency.High
                };
            }

            // Exit if volatility expands beyond MPD bands (extreme move)
            return position.Side == Side.Buy && currentPrice <= mpData.VWAPLowerMPD
                ? new ExitSignal
                {
                    Reason = "Extreme downside volatility: Below VWAP 3? MPD",
                    ExitType = ExitType.StopLoss,
                    Urgency = ExitUrgency.Immediate
                }
                : position.Side == Side.Sell && currentPrice >= mpData.VWAPUpperMPD
                ? new ExitSignal
                {
                    Reason = "Extreme upside volatility: Above VWAP 3? MPD",
                    ExitType = ExitType.StopLoss,
                    Urgency = ExitUrgency.Immediate
                }
                : null;
        }

        #endregion

        #region Confidence Scoring

        /// <summary>
        /// Calculate confidence score for VWAP mean reversion trades
        /// </summary>
        private static int CalculateConfidenceScore(MarketProfileData mpData, double vwapDistance, bool isBuy)
        {
            int score = 50; // Base score

            // Higher confidence if further from VWAP (stronger mean reversion)
            if (Math.Abs(vwapDistance) > 0.30)
            {
                score += 20;
            }
            else if (Math.Abs(vwapDistance) > 0.20)
            {
                score += 10;
            }

            // Higher confidence if price is in value area (institutional acceptance)
            if (mpData.IsPriceInValueArea(isBuy ? mpData.VAL : mpData.VAH))
            {
                score += 15;
            }

            // Higher confidence with moderate volatility
            if (mpData.VWAPStdDevPercent is >= 0.30 and <= 0.80)
            {
                score += 15;
            }

            return Math.Min(score, 100);
        }

        /// <summary>
        /// Calculate confidence score for value area reversals
        /// </summary>
        private static int CalculateValueAreaConfidence(MarketProfileData mpData, bool isBuy)
        {
            int score = 60; // Base score (slightly higher as VA is significant)

            // Higher confidence if value area is tight (strong agreement on price)
            if (mpData.ValueAreaRangePercent < 0.30)
            {
                score += 20;
            }
            else if (mpData.ValueAreaRangePercent < 0.50)
            {
                score += 10;
            }

            // Higher confidence if VPOC is centered (balanced profile)
            double vpocCenteredness = Math.Abs(mpData.VAHDistancePercent - mpData.VALDistancePercent);
            if (vpocCenteredness < 0.05)
            {
                score += 15;
            }

            return Math.Min(score, 100);
        }

        #endregion

        #region Pattern-Based Trading Strategies

        /// <summary>
        /// Evaluate pattern-based entry signals with position reversal capability
        /// Detects institutional order flow and adjusts positioning accordingly
        /// </summary>
        public static EntrySignal EvaluatePatternBasedEntry(
            Symbol symbol,
            string symbolName,
            MarketProfileData mpData,
            PatternDetectionResult patterns,
            Position existingPosition)
        {
            if (symbol == null || mpData == null || patterns == null)
            {
                return null;
            }

            // Check for position reversal opportunities (highest priority)
            EntrySignal reversalSignal = CheckForPositionReversal(symbol, patterns, existingPosition);
            if (reversalSignal != null)
            {
                return reversalSignal;
            }

            // Iceberg-based entries
            EntrySignal icebergSignal = EvaluateIcebergEntry(symbol, mpData, patterns.IcebergOrders);
            if (icebergSignal != null)
            {
                return icebergSignal;
            }

            // Sweep reversal entries
            EntrySignal sweepSignal = EvaluateSweepReversalEntry(symbol, mpData, patterns.LiquiditySweeps);
            if (sweepSignal != null)
            {
                return sweepSignal;
            }

            // Absorption zone breakout entries
            EntrySignal absorptionSignal = EvaluateAbsorptionBreakout(symbol, mpData, patterns.AbsorptionZones);
            if (absorptionSignal != null)
            {
                return absorptionSignal;
            }

            // Failed auction reversal entries
            EntrySignal failureSignal = EvaluateFailedAuctionReversal(symbol, mpData, patterns.FailedAuctions);
            return failureSignal ?? null;
        }

        /// <summary>
        /// Check for position reversal opportunities when institutional flow contradicts current position
        /// </summary>
        private static EntrySignal CheckForPositionReversal(Symbol symbol, PatternDetectionResult patterns, Position existingPosition)
        {
            if (existingPosition == null)
            {
                return null;
            }

            bool isLong = existingPosition.Side == Side.Buy;
            double currentPrice = symbol.Last;

            // SCENARIO 1: Long position + Iceberg detected on sell side = Institutional selling, reverse to short
            if (isLong && patterns.IcebergOrders.Any(i => i.Side == Side.Sell))
            {
                IcebergPattern iceberg = patterns.IcebergOrders.First(i => i.Side == Side.Sell);

                return new EntrySignal
                {
                    Side = Side.Sell,
                    Strategy = "Iceberg Reversal",
                    Reason = $"REVERSAL: Institutional iceberg selling detected at {iceberg.KeyLevel}. Exiting long and flipping to short.",
                    ConfidenceScore = 85 + (iceberg.Confidence / 5), // Very high confidence
                    TargetPercent = -VWAP_EXIT_TARGET_PERCENT,
                    StopLossPercent = STOP_LOSS_PERCENT,
                    IsReversal = true
                };
            }

            // SCENARIO 2: Short position + Iceberg detected on buy side = Institutional buying, reverse to long
            if (!isLong && patterns.IcebergOrders.Any(i => i.Side == Side.Buy))
            {
                IcebergPattern iceberg = patterns.IcebergOrders.First(i => i.Side == Side.Buy);

                return new EntrySignal
                {
                    Side = Side.Buy,
                    Strategy = "Iceberg Reversal",
                    Reason = $"REVERSAL: Institutional iceberg buying detected at {iceberg.KeyLevel}. Exiting short and flipping to long.",
                    ConfidenceScore = 85 + (iceberg.Confidence / 5),
                    TargetPercent = VWAP_EXIT_TARGET_PERCENT,
                    StopLossPercent = -STOP_LOSS_PERCENT,
                    IsReversal = true
                };
            }

            // SCENARIO 3: Long position + Downside sweep with reversal = Stop hunt completed, ride the reversal
            if (isLong && patterns.LiquiditySweeps.Any(s => s.Direction == "Downside Sweep" && s.ReversalConfirmed))
            {
                LiquiditySweep sweep = patterns.LiquiditySweeps.First(s => s.Direction == "Downside Sweep" && s.ReversalConfirmed);

                // Only reverse if sweep was aggressive (high volume)
                if (sweep.VolumeRatio > 3.0)
                {
                    return new EntrySignal
                    {
                        Side = Side.Sell,
                        Strategy = "Sweep Reversal",
                        Reason = $"REVERSAL: Aggressive downside sweep detected (Volume: {sweep.VolumeRatio:F1}x). Stops cleared, riding reversal down.",
                        ConfidenceScore = 80,
                        TargetPercent = -VWAP_EXIT_TARGET_PERCENT,
                        StopLossPercent = STOP_LOSS_PERCENT,
                        IsReversal = true
                    };
                }
            }

            // SCENARIO 4: Short position + Upside sweep with reversal = Stop hunt completed, ride the reversal
            if (!isLong && patterns.LiquiditySweeps.Any(s => s.Direction == "Upside Sweep" && s.ReversalConfirmed))
            {
                LiquiditySweep sweep = patterns.LiquiditySweeps.First(s => s.Direction == "Upside Sweep" && s.ReversalConfirmed);

                if (sweep.VolumeRatio > 3.0)
                {
                    return new EntrySignal
                    {
                        Side = Side.Buy,
                        Strategy = "Sweep Reversal",
                        Reason = $"REVERSAL: Aggressive upside sweep detected (Volume: {sweep.VolumeRatio:F1}x). Stops cleared, riding reversal up.",
                        ConfidenceScore = 80,
                        TargetPercent = VWAP_EXIT_TARGET_PERCENT,
                        StopLossPercent = -STOP_LOSS_PERCENT,
                        IsReversal = true
                    };
                }
            }

            return null;
        }

        /// <summary>
        /// Evaluate entries based on iceberg orders at key levels
        /// </summary>
        private static EntrySignal EvaluateIcebergEntry(Symbol symbol, MarketProfileData mpData, List<IcebergPattern> icebergs)
        {
            if (!icebergs.Any())
            {
                return null;
            }

            double currentPrice = symbol.Last;

            // Find the most confident iceberg near current price
            IcebergPattern nearbyIceberg = icebergs
                .Where(i => i.DistanceFromLevel < 0.5) // Within 0.5% of key level
                .OrderByDescending(i => i.Confidence)
                .FirstOrDefault();

            if (nearbyIceberg == null)
            {
                return null;
            }

            // Iceberg on bid side = Institutional buying support
            if (nearbyIceberg.Side == Side.Buy && currentPrice <= nearbyIceberg.Price * 1.002)
            {
                return new EntrySignal
                {
                    Side = Side.Buy,
                    Strategy = "Iceberg Support",
                    Reason = $"Institutional iceberg buying at {nearbyIceberg.KeyLevel} ({nearbyIceberg.EstimatedTotalSize:F0} size, {nearbyIceberg.RefillCount} refills)",
                    ConfidenceScore = nearbyIceberg.Confidence,
                    TargetPercent = (nearbyIceberg.KeyLevel == "VAL") ?
                        ((mpData.VPOC / currentPrice) - 1) * 100 :
                        ((mpData.VWAP / currentPrice) - 1) * 100,
                    StopLossPercent = -STOP_LOSS_PERCENT
                };
            }

            // Iceberg on ask side = Institutional selling resistance
            return nearbyIceberg.Side == Side.Sell && currentPrice >= nearbyIceberg.Price * 0.998
                ? new EntrySignal
                {
                    Side = Side.Sell,
                    Strategy = "Iceberg Resistance",
                    Reason = $"Institutional iceberg selling at {nearbyIceberg.KeyLevel} ({nearbyIceberg.EstimatedTotalSize:F0} size, {nearbyIceberg.RefillCount} refills)",
                    ConfidenceScore = nearbyIceberg.Confidence,
                    TargetPercent = (nearbyIceberg.KeyLevel == "VAH") ?
                        ((mpData.VPOC / currentPrice) - 1) * 100 :
                        ((mpData.VWAP / currentPrice) - 1) * 100,
                    StopLossPercent = STOP_LOSS_PERCENT
                }
                : null;
        }

        /// <summary>
        /// Evaluate entries based on liquidity sweep reversals
        /// </summary>
        private static EntrySignal EvaluateSweepReversalEntry(Symbol symbol, MarketProfileData mpData, List<LiquiditySweep> sweeps)
        {
            if (!sweeps.Any())
            {
                return null;
            }

            // Find the most recent confirmed sweep
            LiquiditySweep recentSweep = sweeps
                .Where(s => s.ReversalConfirmed)
                .OrderByDescending(s => s.Timestamp)
                .FirstOrDefault();

            if (recentSweep == null)
            {
                return null;
            }

            double currentPrice = symbol.Last;

            // Downside sweep reversal = Buy signal (stops cleared below, price reversing up)
            if (recentSweep.Direction == "Downside Sweep")
            {
                return new EntrySignal
                {
                    Side = Side.Buy,
                    Strategy = "Sweep Reversal Long",
                    Reason = $"Downside liquidity sweep completed at {recentSweep.Price:F2}. Stops cleared, reversal confirmed.",
                    ConfidenceScore = 75,
                    TargetPercent = ((mpData.VWAP / currentPrice) - 1) * 100,
                    StopLossPercent = -STOP_LOSS_PERCENT
                };
            }

            // Upside sweep reversal = Sell signal (stops cleared above, price reversing down)
            return recentSweep.Direction == "Upside Sweep"
                ? new EntrySignal
                {
                    Side = Side.Sell,
                    Strategy = "Sweep Reversal Short",
                    Reason = $"Upside liquidity sweep completed at {recentSweep.Price:F2}. Stops cleared, reversal confirmed.",
                    ConfidenceScore = 75,
                    TargetPercent = ((mpData.VWAP / currentPrice) - 1) * 100,
                    StopLossPercent = STOP_LOSS_PERCENT
                }
                : null;
        }

        /// <summary>
        /// Evaluate entries based on absorption zone breakouts
        /// </summary>
        private static EntrySignal EvaluateAbsorptionBreakout(Symbol symbol, MarketProfileData mpData, List<AbsorptionZone> zones)
        {
            if (!zones.Any())
            {
                return null;
            }

            AbsorptionZone zone = zones.OrderByDescending(z => z.Confidence).First();
            double currentPrice = symbol.Last;

            // Accumulation zone breakout = Buy signal
            if (zone.Type == "Accumulation" && currentPrice > zone.PriceHigh)
            {
                return new EntrySignal
                {
                    Side = Side.Buy,
                    Strategy = "Accumulation Breakout",
                    Reason = $"Breakout from accumulation zone (Vol: {zone.VolumeRatio:F1}x, Range: {zone.PriceMovement:F2}%)",
                    ConfidenceScore = zone.Confidence,
                    TargetPercent = ((mpData.VAH / currentPrice) - 1) * 100,
                    StopLossPercent = -STOP_LOSS_PERCENT
                };
            }

            // Distribution zone breakdown = Sell signal
            return zone.Type == "Distribution" && currentPrice < zone.PriceLow
                ? new EntrySignal
                {
                    Side = Side.Sell,
                    Strategy = "Distribution Breakdown",
                    Reason = $"Breakdown from distribution zone (Vol: {zone.VolumeRatio:F1}x, Range: {zone.PriceMovement:F2}%)",
                    ConfidenceScore = zone.Confidence,
                    TargetPercent = ((mpData.VAL / currentPrice) - 1) * 100,
                    StopLossPercent = STOP_LOSS_PERCENT
                }
                : null;
        }

        /// <summary>
        /// Evaluate entries based on failed auction reversals
        /// </summary>
        private static EntrySignal EvaluateFailedAuctionReversal(Symbol symbol, MarketProfileData mpData, List<FailedAuction> failures)
        {
            if (!failures.Any())
            {
                return null;
            }

            // Find the most recent high-confidence failure at a key level
            FailedAuction recentFailure = failures
                .Where(f => f.NearKeyLevel != "None" && f.Confidence > 60)
                .OrderByDescending(f => f.Timestamp)
                .FirstOrDefault();

            if (recentFailure == null)
            {
                return null;
            }

            double currentPrice = symbol.Last;

            // Bearish rejection = Sell signal
            if (recentFailure.Direction == "Bearish Rejection")
            {
                return new EntrySignal
                {
                    Side = Side.Sell,
                    Strategy = "Failed Auction Short",
                    Reason = $"Bearish rejection at {recentFailure.NearKeyLevel} (Vol: {recentFailure.VolumeRatio:F2}x, Wick: {recentFailure.WickToBodyRatio:F1}x)",
                    ConfidenceScore = recentFailure.Confidence,
                    TargetPercent = ((mpData.VPOC / currentPrice) - 1) * 100,
                    StopLossPercent = STOP_LOSS_PERCENT
                };
            }

            // Bullish rejection = Buy signal
            return recentFailure.Direction == "Bullish Rejection"
                ? new EntrySignal
                {
                    Side = Side.Buy,
                    Strategy = "Failed Auction Long",
                    Reason = $"Bullish rejection at {recentFailure.NearKeyLevel} (Vol: {recentFailure.VolumeRatio:F2}x, Wick: {recentFailure.WickToBodyRatio:F1}x)",
                    ConfidenceScore = recentFailure.Confidence,
                    TargetPercent = ((mpData.VPOC / currentPrice) - 1) * 100,
                    StopLossPercent = -STOP_LOSS_PERCENT
                }
                : null;
        }

        /// <summary>
        /// Evaluate pattern-based exits using Liquidity Reversion Principle
        /// Price tends to revert to areas of high liquidity after sweeps/imbalances
        /// </summary>
        public static ExitSignal EvaluatePatternBasedExit(
            Symbol symbol,
            Position position,
            MarketProfileData mpData,
            PatternDetectionResult patterns,
            double entryPrice)
        {
            if (symbol == null || position == null || mpData == null || patterns == null)
            {
                return null;
            }

            double currentPrice = symbol.Last;
            bool isLong = position.Side == Side.Buy;
            double pnlPercent = isLong
                ? ((currentPrice / entryPrice) - 1) * 100
                : ((entryPrice / currentPrice) - 1) * 100;

            // LIQUIDITY REVERSION PRINCIPLE:
            // After price sweeps liquidity or creates imbalance, it tends to revert to balanced zones

            // Exit 1: Liquidity gap ahead = Price will accelerate through, take profits before reversal
            LiquidityGap gapAhead = patterns.LiquidityGaps.FirstOrDefault(g =>
                (isLong && g.Price > currentPrice && g.Price < currentPrice * 1.01) ||
                (!isLong && g.Price < currentPrice && g.Price > currentPrice * 0.99));

            if (gapAhead != null)
            {
                return new ExitSignal
                {
                    Reason = $"Liquidity gap ahead at {gapAhead.Price:F2} ({gapAhead.Significance}). Price will accelerate and likely reverse.",
                    ExitType = ExitType.TakeProfit,
                    Urgency = ExitUrgency.High
                };
            }

            // Exit 2: Price reverted to VWAP after sweep = Liquidity balance restored
            if (pnlPercent > 0.15) // In profit
            {
                bool nearVWAP = Math.Abs(currentPrice - mpData.VWAP) / mpData.VWAP < 0.002; // Within 0.2%

                if (nearVWAP && patterns.LiquiditySweeps.Any())
                {
                    return new ExitSignal
                    {
                        Reason = "Price reverted to VWAP after liquidity sweep. Balance restored, taking profits.",
                        ExitType = ExitType.TakeProfit,
                        Urgency = ExitUrgency.Normal
                    };
                }
            }

            // Exit 3: Opposite iceberg detected = Institutional resistance/support
            if (isLong && patterns.IcebergOrders.Any(i => i.Side == Side.Sell && i.Price > currentPrice && i.Price < currentPrice * 1.005))
            {
                IcebergPattern iceberg = patterns.IcebergOrders.First(i => i.Side == Side.Sell);
                return new ExitSignal
                {
                    Reason = $"Institutional iceberg selling ahead at {iceberg.KeyLevel}. Strong resistance detected.",
                    ExitType = ExitType.TakeProfit,
                    Urgency = ExitUrgency.High
                };
            }

            if (!isLong && patterns.IcebergOrders.Any(i => i.Side == Side.Buy && i.Price < currentPrice && i.Price > currentPrice * 0.995))
            {
                IcebergPattern iceberg = patterns.IcebergOrders.First(i => i.Side == Side.Buy);
                return new ExitSignal
                {
                    Reason = $"Institutional iceberg buying below at {iceberg.KeyLevel}. Strong support detected.",
                    ExitType = ExitType.TakeProfit,
                    Urgency = ExitUrgency.High
                };
            }

            // Exit 4: Absorption zone entered = Likely consolidation/reversal zone
            AbsorptionZone absorptionZone = patterns.AbsorptionZones.FirstOrDefault(z =>
                currentPrice >= z.PriceLow && currentPrice <= z.PriceHigh);

            if (absorptionZone != null && pnlPercent > 0.20)
            {
                // Check if absorption type conflicts with position direction
                bool conflictingAbsorption =
                    (isLong && absorptionZone.Type == "Distribution") ||
                    (!isLong && absorptionZone.Type == "Accumulation");

                if (conflictingAbsorption)
                {
                    return new ExitSignal
                    {
                        Reason = $"Entered {absorptionZone.Type} zone. Institutional flow conflicts with position direction.",
                        ExitType = ExitType.Reversal,
                        Urgency = ExitUrgency.High
                    };
                }
            }

            // Exit 5: Failed auction in direction of trade = Weak continuation
            FailedAuction recentFailure = patterns.FailedAuctions
                .Where(f => f.NearKeyLevel != "None")
                .OrderByDescending(f => f.Timestamp)
                .FirstOrDefault();

            if (recentFailure != null)
            {
                bool failureConflicts =
                    (isLong && recentFailure.Direction == "Bearish Rejection") ||
                    (!isLong && recentFailure.Direction == "Bullish Rejection");

                if (failureConflicts && Math.Abs(currentPrice - recentFailure.Price) / currentPrice < 0.005)
                {
                    return new ExitSignal
                    {
                        Reason = $"Failed auction at {recentFailure.NearKeyLevel}. Weak {(isLong ? "upside" : "downside")} momentum.",
                        ExitType = ExitType.Reversal,
                        Urgency = ExitUrgency.Normal
                    };
                }
            }

            // Exit 6: Sweep detected against position = Stops being hunted on our side
            LiquiditySweep opposingSweep = patterns.LiquiditySweeps
                .Where(s =>
                    (isLong && s.Direction == "Downside Sweep") ||
                    (!isLong && s.Direction == "Upside Sweep"))
                .OrderByDescending(s => s.Timestamp)
                .FirstOrDefault();

            return opposingSweep != null && !opposingSweep.ReversalConfirmed
                ? new ExitSignal
                {
                    Reason = "Liquidity sweep detected against position direction. Exit before stop hunt continues.",
                    ExitType = ExitType.StopLoss,
                    Urgency = ExitUrgency.Immediate
                }
                : null;
        }

        #endregion

        #region Order Execution

        /// <summary>
        /// Execute entry order based on signal
        /// Returns null if parameters are invalid, otherwise returns the result from PlaceOrder
        /// </summary>
        public static TradingOperationResult ExecuteEntry(
            Symbol symbol,
            EntrySignal signal,
            Account account,
            double quantity)
        {
            if (symbol == null || signal == null || account == null)
            {
                return null;
            }

            // Place market order
            return Core.Instance.PlaceOrder(new PlaceOrderRequestParameters
            {
                Account = account,
                Symbol = symbol,
                OrderTypeId = OrderType.Market,
                Quantity = quantity,
                Side = signal.Side
            });
        }

        /// <summary>
        /// Execute exit order by closing position
        /// Returns null if position is invalid, otherwise returns the result from Close
        /// </summary>
        public static TradingOperationResult ExecuteExit(Position position)
        {
            return position?.Close();
        }

        /// <summary>
        /// Execute position reversal (close current and open opposite)
        /// Returns a tuple: (closeResult, openResult)
        /// Either value may be null if validation fails
        /// </summary>
        public static (TradingOperationResult closeResult, TradingOperationResult openResult) ExecuteReversal(
            Position currentPosition,
            EntrySignal reversalSignal,
            Account account,
            Symbol symbol,
            double quantity)
        {
            if (currentPosition == null || reversalSignal == null || !reversalSignal.IsReversal)
            {
                return (null, null);
            }

            // Step 1: Close existing position
            TradingOperationResult closeResult = currentPosition.Close();

            if (closeResult == null || closeResult.Status != TradingOperationResultStatus.Success)
            {
                return (closeResult, null);
            }

            // Step 2: Open opposite position
            TradingOperationResult openResult = Core.Instance.PlaceOrder(new PlaceOrderRequestParameters
            {
                Account = account,
                Symbol = symbol,
                OrderTypeId = OrderType.Market,
                Quantity = quantity,
                Side = reversalSignal.Side
            });

            return (closeResult, openResult);
        }

        #endregion
    }

    #region Signal Classes

    /// <summary>
    /// Represents an entry signal with strategy details
    /// </summary>
    public class EntrySignal
    {
        public Side Side { get; set; }
        public string Strategy { get; set; }
        public string Reason { get; set; }
        public int ConfidenceScore { get; set; }
        public double TargetPercent { get; set; }
        public double StopLossPercent { get; set; }
        public bool IsReversal { get; set; } = false; // Flag for position reversal signals
    }

    /// <summary>
    /// Represents an exit signal with urgency
    /// </summary>
    public class ExitSignal
    {
        public string Reason { get; set; }
        public ExitType ExitType { get; set; }
        public ExitUrgency Urgency { get; set; }
    }

    /// <summary>
    /// Types of exits
    /// </summary>
    public enum ExitType
    {
        TakeProfit,
        StopLoss,
        Reversal,
        TimeOut
    }

    /// <summary>
    /// Exit urgency levels
    /// </summary>
    public enum ExitUrgency
    {
        Low,
        Normal,
        High,
        Immediate
    }

    #endregion
}
