# Microstructure Pattern - Level 2 Data Mapping

**Purpose**: Reference document mapping market microstructure patterns to specific Level 2/DOM/Time&Sales data fields for signal confirmation

**Date**: 2025-10-03

**Usage**: When implementing pattern detection algorithms, reference this document to identify which Level 2 data fields are required

---

## Pattern Categories

1. [Market Intelligence Enhancement](#1-market-intelligence-enhancement)
2. [Convergence Detection](#2-convergence-detection)
3. [Divergence Signals](#3-divergence-signals)
4. [Institutional Footprints](#4-institutional-footprints)
5. [Scalper Activity Detection](#5-scalper-activity-detection)
6. [Spatial Pattern Categories](#6-spatial-pattern-categories)
7. [Nested Support/Resistance](#7-nested-supportresistance)
8. [Compression Patterns](#8-compression-patterns)
9. [Expansion Patterns](#9-expansion-patterns)
10. [Regime Transitions](#10-regime-transitions)
11. [Hidden Liquidity Detection](#11-hidden-liquidity-detection)
12. [Momentum Quality Assessment](#12-momentum-quality-assessment)
13. [Algorithmic Behavior Recognition](#13-algorithmic-behavior-recognition)
14. [Market Participant Identification](#14-market-participant-identification)
15. [Trend Strength Measurement](#15-trend-strength-measurement)
16. [Reversal Prediction](#16-reversal-prediction)

---

## 1. Market Intelligence Enhancement

**Purpose:** Superior market understanding through multi-timeframe analysis convergence

**Primary Features:**
- VWAP bid (multi-timeframe)
- VWAP ask (multi-timeframe)
- Delta (across timeframes)
- Cumulative delta (across timeframes)
- Volume (per timeframe)

**Secondary Features:**
- Trades (frequency analysis)
- Buy/Sell volume ratio
- Value Area High (VAH)
- Value Area Low (VAL)

**Detection Logic:**
```
FOR each timeframe (1min, 5min, 15min, 30min, 2hr):
    Calculate VWAP, Delta, Volume
    Store snapshot

Convergence score = similarity(all_timeframe_VWAPs)
IF convergence_score > threshold:
    High-confidence market view
```

**Quantower API Source:**
- `VolumeAnalysisItem.Volume`
- `VolumeAnalysisItem.Delta`
- `VolumeAnalysisItem.BuyVolume`
- `VolumeAnalysisItem.SellVolume`
- Custom VWAP calculation per timeframe
- `VolumeAnalysisData.ValueAreaHigh`
- `VolumeAnalysisData.ValueAreaLow`

---

## 2. Convergence Detection

**Purpose:** Identify strong directional agreement across multiple timeframes

**Primary Features:**
- VWAP (all timeframes)
- Delta (all timeframes)
- Cumulative delta (all timeframes)
- Buy volume % (all timeframes)
- Sell volume % (all timeframes)

**Secondary Features:**
- Imbalance % (current)
- Aggressor direction (recent trades)
- Tick direction (bid/ask)

**Detection Logic:**
```
all_vwaps_aligned = (std_dev(vwap_1m, vwap_5m, vwap_15m, vwap_30m) < threshold)
all_deltas_positive = (delta_1m > 0 AND delta_5m > 0 AND delta_15m > 0)
all_cumulative_delta_rising = (cumdelta_slope_all_timeframes > 0)

IF all_vwaps_aligned AND all_deltas_positive AND all_cumulative_delta_rising:
    CONVERGENCE = BULLISH
```

**Quantower API Source:**
- `VolumeAnalysisItem.Delta` (per timeframe)
- `VolumeAnalysisItem.BuyVolume`
- `VolumeAnalysisItem.SellVolume`
- `Level2Quote.Aggressor` (from Time & Sales)
- `Last.TickDirection`

---

## 3. Divergence Signals

**Purpose:** Detect timeframe conflicts indicating potential reversals

**Primary Features:**
- VWAP (short vs long timeframe)
- Delta (short vs long timeframe)
- Cumulative delta (trend direction)
- Buy/Sell volume % (shifting ratios)

**Secondary Features:**
- Imbalance % (extreme values)
- Delta % (magnitude of disagreement)
- POC drift rate (value area migration)

**Detection Logic:**
```
short_term_bullish = (vwap_1m > vwap_5m) AND (delta_1m > 0)
long_term_bearish = (vwap_30m < vwap_2hr) AND (cumulative_delta_30m < 0)

IF short_term_bullish AND long_term_bearish:
    DIVERGENCE = WARNING (potential reversal down)
```

**Quantower API Source:**
- `VolumeAnalysisItem.Delta`
- `VolumeAnalysisItem.DeltaPercent`
- `VolumeAnalysisItem.BuyVolumePercent`
- `VolumeAnalysisItem.SellVolumePercent`
- Custom POC drift calculation
- Level 2 imbalance calculation

---

## 4. Institutional Footprints

**Purpose:** Identify large player activity (smart money)

**Primary Features:**
- Max one trade volume
- Max one trade volume %
- Filtered volume (large trades)
- Filtered volume %
- Average size (compared to normal)
- Average buy size
- Average sell size

**Secondary Features:**
- VWAP bid (large orders sitting)
- VWAP ask (large orders sitting)
- Cumulative size bid
- Cumulative size ask
- Time bid (how long resting)
- Time ask (how long resting)

**Detection Logic:**
```
large_trade_threshold = average_size * 5
institutional_activity = (
    max_one_trade_volume > large_trade_threshold AND
    filtered_volume_percent > 20% AND
    (average_buy_size > 2 * average_size OR average_sell_size > 2 * average_size)
)

IF institutional_activity:
    IF average_buy_size > average_sell_size:
        INSTITUTIONAL = ACCUMULATION
    ELSE:
        INSTITUTIONAL = DISTRIBUTION
```

**Quantower API Source:**
- `VolumeAnalysisItem.MaxOneTradeVolume`
- `VolumeAnalysisItem.MaxOneTradeVolumePercent`
- `VolumeAnalysisItem.FilteredVolume`
- `VolumeAnalysisItem.FilteredVolumePercent`
- `VolumeAnalysisItem.AverageSize`
- `VolumeAnalysisItem.AverageBuySize`
- `VolumeAnalysisItem.AverageSellSize`
- `Level2Item` (via DepthOfMarket for cumulative sizes)

---

## 5. Scalper Activity Detection

**Purpose:** Identify short-term players creating noise or momentum

**Primary Features:**
- Bids number of changes (high frequency)
- Asks number of changes (high frequency)
- Trades (high count)
- Average size (small)
- Max one trade volume % (low)

**Secondary Features:**
- Delta (rapid fluctuations)
- Imbalance % (shifting quickly)
- Tick direction (frequent changes)
- Bid tick direction
- Ask tick direction

**Detection Logic:**
```
high_frequency = (bids_changes_per_minute > 100 AND asks_changes_per_minute > 100)
small_trades = (average_size < normal_average * 0.5)
low_institutional = (max_one_trade_volume_percent < 5%)

IF high_frequency AND small_trades AND low_institutional:
    SCALPER_ACTIVITY = DETECTED
```

**Quantower API Source:**
- `Level2Quote` tracking changes per minute
- `VolumeAnalysisItem.Trades`
- `VolumeAnalysisItem.AverageSize`
- `VolumeAnalysisItem.MaxOneTradeVolumePercent`
- `VolumeAnalysisItem.Delta` (rapid changes)
- `Last.TickDirection`

---

## 6. Spatial Pattern Categories

**Purpose:** Classify market structure for strategy adaptation

**Primary Features:**
- Value Area High (VAH)
- Value Area Low (VAL)
- VPOC (Point of Control)
- POC drift rate
- Percentage of volume in value area vs tails

**Secondary Features:**
- Previous Day High (PDH)
- Previous Day Low (PDL)
- Daily High (DH)
- Daily Low (DL)
- Volume distribution across price levels

**Detection Logic:**
```
va_width = VAH - VAL
tail_volume_percent = 100 - percentage_volume_in_va

PATTERN = CLASSIFY(
    va_width: NARROW/NORMAL/WIDE,
    tail_volume_percent: LOW/MEDIUM/HIGH,
    poc_drift_rate: STABLE/RISING/FALLING
)

Examples:
- NARROW va_width + HIGH tail_volume = BREAKOUT SETUP
- WIDE va_width + LOW tail_volume = BALANCED AUCTION
- STABLE poc + NARROW va = CONSOLIDATION
```

**Quantower API Source:**
- `VolumeAnalysisData.ValueAreaHigh`
- `VolumeAnalysisData.ValueAreaLow`
- `VolumeAnalysisData.POC` (custom calculation from PriceLevels)
- `VolumeAnalysisData.PriceLevels` (dictionary of price → volume)
- Custom PDH/PDL/DH/DL tracking

---

## 7. Nested Support/Resistance

**Purpose:** Multiple timeframe support/resistance levels for layered entries/exits

**Primary Features:**
- VWAP (all timeframes)
- Value Area High (all timeframes)
- Value Area Low (all timeframes)
- Previous Day High/Low
- Daily High/Low

**Secondary Features:**
- VPOC (all timeframes)
- Bid size (clusters at levels)
- Ask size (clusters at levels)
- Cumulative size bid/ask (level strength)

**Detection Logic:**
```
support_levels = [VAL_1m, VAL_5m, VAL_15m, PDL, DL, VWAP_30m_lower_band]
resistance_levels = [VAH_1m, VAH_5m, VAH_15m, PDH, DH, VWAP_30m_upper_band]

FOR each support_level:
    IF price approaching AND bid_size_cluster_at_level > threshold:
        NESTED_SUPPORT[level] = STRONG

FOR each resistance_level:
    IF price approaching AND ask_size_cluster_at_level > threshold:
        NESTED_RESISTANCE[level] = STRONG
```

**Quantower API Source:**
- `VolumeAnalysisData.ValueAreaHigh` (multiple timeframes)
- `VolumeAnalysisData.ValueAreaLow` (multiple timeframes)
- Custom VWAP bands (multiple timeframes)
- `DepthOfMarket` for bid/ask size clustering
- Custom PDH/PDL/DH/DL tracking

---

## 8. Compression Patterns

**Purpose:** Detect volatility contraction preceding breakouts

**Primary Features:**
- Value Area width (VAH - VAL, decreasing)
- VWAP band width (all timeframes, narrowing)
- TWAP band width (all timeframes, narrowing)
- Bid-Ask spread (tightening)
- Bids liquidity changes (decreasing)
- Asks liquidity changes (decreasing)

**Secondary Features:**
- Volume (decreasing)
- Trades (decreasing frequency)
- Average size (stabilizing)
- Delta (low magnitude)

**Detection Logic:**
```
va_width_current = VAH - VAL
va_width_previous = VAH_prev - VAL_prev
compression_ratio = va_width_current / va_width_previous

IF compression_ratio < 0.8 FOR 5+ consecutive bars:
    IF vwap_band_width_ratio < 0.8:
        IF bid_ask_spread < normal_spread * 0.7:
            COMPRESSION = DETECTED (breakout imminent)
```

**Quantower API Source:**
- `VolumeAnalysisData.ValueAreaHigh`
- `VolumeAnalysisData.ValueAreaLow`
- Custom VWAP/TWAP band width tracking
- `Quote.BestBid` and `Quote.BestAsk` (spread calculation)
- `Level2Quote` (liquidity changes tracking)
- `VolumeAnalysisItem.Volume`

---

## 9. Expansion Patterns

**Purpose:** Detect volatility expansion indicating trend acceleration

**Primary Features:**
- Value Area width (VAH - VAL, increasing)
- VWAP band width (expanding)
- TWAP band width (expanding)
- Bid-Ask spread (widening)
- Bids liquidity changes (increasing)
- Asks liquidity changes (increasing)

**Secondary Features:**
- Volume (increasing)
- Trades (increasing frequency)
- Max one trade volume (spiking)
- Delta (high magnitude)
- Cumulative delta (strong trend)

**Detection Logic:**
```
va_width_rate = d(VAH - VAL) / dt
vwap_width_rate = d(vwap_upper - vwap_lower) / dt

IF va_width_rate > expansion_threshold:
    IF vwap_width_rate > expansion_threshold:
        IF volume > avg_volume * 1.5:
            IF cumulative_delta strongly trending:
                EXPANSION = TREND_ACCELERATION
```

**Quantower API Source:**
- `VolumeAnalysisData.ValueAreaHigh`
- `VolumeAnalysisData.ValueAreaLow`
- Custom VWAP/TWAP band tracking
- `Quote.BestBid` and `Quote.BestAsk`
- `VolumeAnalysisItem.Volume`
- `VolumeAnalysisItem.Delta`
- `VolumeAnalysisItem.MaxOneTradeVolume`

---

## 10. Regime Transitions

**Purpose:** Detect market state changes for strategy adaptation

**Primary Features:**
- POC drift rate (changing from stable to moving)
- Value Area percentage (changing distribution)
- Delta (sign changes)
- Cumulative delta (slope changes)
- Volume (pattern shifts)

**Secondary Features:**
- Imbalance % (regime shift)
- Buy/Sell volume % (ratio changes)
- Aggressor (directional shift)
- VWAP/TWAP divergence (appearing/disappearing)

**Detection Logic:**
```
PREVIOUS_REGIME = RANGING (POC stable, low delta, balanced volume)
CURRENT_STATE = (
    POC_drift_rate increased 3x,
    Cumulative_delta slope changed from flat to steep,
    Imbalance % > 60% (was 50%)
)

IF CURRENT_STATE != PREVIOUS_REGIME:
    REGIME_TRANSITION = RANGING → TRENDING
    Adapt strategy accordingly
```

**Quantower API Source:**
- Custom POC drift calculation
- `VolumeAnalysisData.PriceLevels` (volume distribution)
- `VolumeAnalysisItem.Delta`
- `VolumeAnalysisItem.CumulativeDelta` (slope calculation)
- `VolumeAnalysisItem.BuyVolumePercent`
- `VolumeAnalysisItem.SellVolumePercent`
- `Level2Quote.Aggressor`

---

## 11. Hidden Liquidity Detection

**Purpose:** Find institutional support/resistance levels via multi-timeframe analysis

**Primary Features:**
- Cumulative size bid (across timeframes)
- Cumulative size ask (across timeframes)
- VWAP bid (persistent levels)
- VWAP ask (persistent levels)
- Time bid (duration at level)
- Time ask (duration at level)

**Secondary Features:**
- Bids liquidity changes (refreshing patterns)
- Asks liquidity changes (refreshing patterns)
- Filtered volume (large orders at level)
- Max one trade volume (iceberg detection)

**Detection Logic:**
```
FOR each price_level:
    IF cumulative_bid_size > threshold:
        IF time_bid_at_level > 5_minutes:
            IF bids_refresh_pattern (liquidity_changes show replenishment):
                HIDDEN_LIQUIDITY_BID[price_level] = DETECTED (iceberg/institutional)

    IF cumulative_ask_size > threshold:
        IF time_ask_at_level > 5_minutes:
            IF asks_refresh_pattern:
                HIDDEN_LIQUIDITY_ASK[price_level] = DETECTED
```

**Quantower API Source:**
- `Level2Item.Size` (cumulative tracking)
- `Level2Item.Price`
- `DepthOfMarket.Aggregated` collections
- Custom time tracking at price levels
- `Level2Quote` change tracking
- `VolumeAnalysisItem.FilteredVolume`
- `VolumeAnalysisItem.MaxOneTradeVolume`

---

## 12. Momentum Quality Assessment

**Purpose:** Distinguish real moves from false breakouts via multi-timeframe confirmation

**Primary Features:**
- Delta (all timeframes, alignment)
- Cumulative delta (all timeframes, trend)
- Aggressor (directional consistency)
- Buy volume % (increasing in trend direction)
- Sell volume % (decreasing in trend direction)

**Secondary Features:**
- Tick direction (consistency)
- Bid tick direction
- Ask tick direction
- Volume (supporting move)
- Max one trade volume (institutional participation)

**Detection Logic:**
```
price_breakout_up = price > resistance

momentum_quality = CALCULATE(
    delta_1m > 0 AND delta_5m > 0 AND delta_15m > 0: +3 points,
    cumulative_delta rising all timeframes: +3 points,
    aggressor = BUY consistently: +2 points,
    buy_volume_percent > 60%: +2 points,
    volume > avg * 1.5: +2 points,
    max_one_trade_volume spiking: +2 points
)

IF price_breakout_up AND momentum_quality >= 10:
    QUALITY = HIGH (real move)
ELSE:
    QUALITY = LOW (likely false breakout)
```

**Quantower API Source:**
- `VolumeAnalysisItem.Delta` (multi-timeframe)
- `VolumeAnalysisItem.CumulativeDelta`
- `Level2Quote.Aggressor`
- `VolumeAnalysisItem.BuyVolumePercent`
- `VolumeAnalysisItem.SellVolumePercent`
- `Last.TickDirection`
- `VolumeAnalysisItem.Volume`
- `VolumeAnalysisItem.MaxOneTradeVolume`

---

## 13. Algorithmic Behavior Recognition

**Purpose:** Detect algo trading patterns via regular timeframe touches and periodic behavior

**Primary Features:**
- Bids number of changes (regularity analysis)
- Asks number of changes (regularity analysis)
- VWAP touches (periodic pattern)
- Time bid (regular intervals)
- Time ask (regular intervals)

**Secondary Features:**
- Average size (consistent sizing)
- Trades (periodic clustering)
- Bid size (algorithmic sizing patterns)
- Ask size (algorithmic sizing patterns)

**Detection Logic:**
```
changes_per_second = bids_changes / time_window
periodicity = FFT_analysis(changes_per_second_timeseries)

IF periodicity shows regular peaks (e.g., every 5 seconds):
    IF average_size shows consistent values (low variance):
        IF vwap_touches at regular intervals:
            ALGO_BEHAVIOR = DETECTED
            Pattern: MARKET_MAKING / VWAP_EXECUTION / TWAP_EXECUTION
```

**Quantower API Source:**
- `Level2Quote` (change tracking with timestamps)
- Custom periodicity analysis
- VWAP touch tracking
- `VolumeAnalysisItem.AverageSize` (variance calculation)
- `VolumeAnalysisItem.Trades` (clustering analysis)
- `Level2Item.Size` (sizing patterns)

---

## 14. Market Participant Identification

**Purpose:** Identify dominant players (retail, institutions, algos) via volume distribution

**Primary Features:**
- Average size (participant profiling)
- Average buy size
- Average sell size
- Max one trade volume
- Max one trade volume %
- Filtered volume (institutional threshold)
- Filtered buy volume
- Filtered sell volume

**Secondary Features:**
- Trades (frequency indicates participant type)
- Buy trades
- Sell trades
- Delta (retail vs institutional bias)
- Bids/Asks number of changes (algo indicators)

**Detection Logic:**
```
PARTICIPANT_PROFILE = CLASSIFY(
    avg_size: SMALL (<10 contracts) = RETAIL,
    avg_size: MEDIUM (10-50) = SMALL_INSTITUTIONAL,
    avg_size: LARGE (>50) = LARGE_INSTITUTIONAL,

    filtered_volume_percent: HIGH (>30%) = INSTITUTIONAL_DOMINANT,
    max_one_trade_volume_percent: HIGH (>15%) = WHALE_ACTIVE,

    changes_per_minute: VERY_HIGH (>200) = ALGO_DOMINANT,
    changes_per_minute: LOW (<20) = MANUAL_TRADERS
)

Current market = weighted_average(PARTICIPANT_PROFILE across timeframes)
```

**Quantower API Source:**
- `VolumeAnalysisItem.AverageSize`
- `VolumeAnalysisItem.AverageBuySize`
- `VolumeAnalysisItem.AverageSellSize`
- `VolumeAnalysisItem.MaxOneTradeVolume`
- `VolumeAnalysisItem.MaxOneTradeVolumePercent`
- `VolumeAnalysisItem.FilteredVolume`
- `VolumeAnalysisItem.FilteredBuyVolume`
- `VolumeAnalysisItem.FilteredSellVolume`
- `VolumeAnalysisItem.Trades`
- `VolumeAnalysisItem.BuyTrades`
- `VolumeAnalysisItem.SellTrades`

---

## 15. Trend Strength Measurement

**Purpose:** Quantify trend reliability via timeframe alignment percentage

**Primary Features:**
- Delta (all timeframes)
- Cumulative delta (all timeframes)
- VWAP direction (all timeframes)
- TWAP direction (all timeframes)
- Buy/Sell volume % (all timeframes)

**Secondary Features:**
- Aggressor consistency
- POC drift rate (alignment with trend)
- Value Area migration (supporting trend)

**Detection Logic:**
```
timeframes = [1m, 5m, 15m, 30m, 2hr]
trend_score = 0

FOR each timeframe:
    IF delta > 0: trend_score += 1
    IF cumulative_delta rising: trend_score += 1
    IF vwap rising: trend_score += 1
    IF twap rising: trend_score += 1
    IF buy_volume_percent > 55%: trend_score += 1

max_score = 5 * len(timeframes) = 25
trend_strength_percent = (trend_score / max_score) * 100

IF trend_strength_percent > 80%:
    TREND_STRENGTH = VERY_STRONG (all timeframes aligned)
ELIF trend_strength_percent > 60%:
    TREND_STRENGTH = STRONG
ELSE:
    TREND_STRENGTH = WEAK
```

**Quantower API Source:**
- `VolumeAnalysisItem.Delta` (multi-timeframe)
- `VolumeAnalysisItem.CumulativeDelta`
- Custom VWAP/TWAP direction tracking
- `VolumeAnalysisItem.BuyVolumePercent`
- `VolumeAnalysisItem.SellVolumePercent`
- `Level2Quote.Aggressor`
- Custom POC drift tracking

---

## 16. Reversal Prediction

**Purpose:** Anticipate trend changes via divergence patterns

**Primary Features:**
- Delta (sign changes)
- Cumulative delta (slope changes, divergence from price)
- Imbalance % (extreme values → mean reversion)
- POC drift rate (deceleration or reversal)
- Delta % (magnitude shifts)

**Secondary Features:**
- Buy/Sell volume % (ratio flipping)
- Aggressor (directional shift)
- Max one trade volume (capitulation/exhaustion)
- Value Area percentage (distribution shift)
- VWAP/TWAP divergence (early warning)

**Detection Logic:**
```
price_rising = price > price_5bars_ago
cumulative_delta_falling = cumulative_delta_slope < 0

IF price_rising AND cumulative_delta_falling:
    DIVERGENCE = BEARISH (price up, volume pressure down)

    IF imbalance_percent < 30% (extreme sell pressure):
        IF POC_drift_rate decelerating:
            IF max_one_trade_volume spiking (capitulation):
                REVERSAL_PREDICTION = DOWN (high probability)

price_falling = price < price_5bars_ago
cumulative_delta_rising = cumulative_delta_slope > 0

IF price_falling AND cumulative_delta_rising:
    DIVERGENCE = BULLISH (price down, volume pressure up)

    IF imbalance_percent > 70% (extreme buy pressure):
        IF POC_drift_rate decelerating:
            REVERSAL_PREDICTION = UP (high probability)
```

**Quantower API Source:**
- `VolumeAnalysisItem.Delta`
- `VolumeAnalysisItem.CumulativeDelta` (slope calculation)
- `VolumeAnalysisItem.DeltaPercent`
- Level 2 imbalance calculation
- Custom POC drift tracking
- `VolumeAnalysisItem.BuyVolumePercent`
- `VolumeAnalysisItem.SellVolumePercent`
- `Level2Quote.Aggressor`
- `VolumeAnalysisItem.MaxOneTradeVolume`
- `VolumeAnalysisData` volume distribution percentage

---

## Summary: Data Source Requirements

### Level 2 / Depth of Market (DOM)
- Bid/Ask prices, sizes
- Cumulative bid/ask sizes
- Liquidity changes tracking
- Number of changes per minute
- Time at bid/ask levels

### Time & Sales
- Last trade size
- Bid/Ask trade sizes
- Aggressor flag
- Tick direction (bid/ask)

### Volume Analysis (VolumeAnalysisItem)
- Volume, Delta, Cumulative Delta
- Buy/Sell volume and percentages
- Buy/Sell trades count
- Average sizes (total, buy, sell)
- Max one trade volume
- Filtered volume (large trades)
- Imbalance percentage

### Value Area (VolumeAnalysisData)
- VAH (Value Area High)
- VAL (Value Area Low)
- POC (Point of Control)
- Volume distribution across price levels

### Historical Benchmarks
- Previous Day: High, Low, Close, Settlement
- Daily: High, Low

### Custom Indicators
- VWAP (multi-timeframe)
- TWAP (multi-timeframe)
- VWAP bands (upper/lower)
- TWAP bands (upper/lower)

---

**Last Updated**: 2025-10-03
**Next Steps**: Implement pattern detection classes using these mappings
**Reference**: See IMPLEMENTATION_RESEARCH.md for Quantower API usage patterns
