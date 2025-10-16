# TWAP Band Signals - IF-THEN Conditions

**Purpose**: Convert TWAP band movement and TWAP vs VWAP divergence scenarios into executable IF-THEN logic

**Output Format**: Binary (0/1) signals and categorical states

---

## Direction Encoding

```python
UP = 1
FLAT = 0
DOWN = -1

BULLISH = 1
BEARISH = 0
NEUTRAL = None
```

---

## Core TWAP Band Scenarios

### Scenario 1: Top UP, Bottom DOWN, Price UP

```python
IF twap_top_band_dir == UP AND twap_bottom_band_dir == DOWN AND price_dir == UP:
    # EXTREME PRICE VOLATILITY
    extreme_volatility = 1

    # UPWARD MOMENTUM
    IF price_velocity > momentum_threshold:
        upward_momentum = 1
    ELSE:
        upward_momentum = 0

    # TIME-BASED EXPANSION
    time_expansion = 1

    # TECHNICAL BREAKOUT
    IF price > twap_upper_band:
        technical_breakout = 1
    ELSE:
        technical_breakout = 0

    # SIGNAL
    signal = BULLISH  # 1
    signal_type = 'MOMENTUM_LONG'
    signal_strength = 2
```

---

### Scenario 2: Top UP, Bottom DOWN, Price DOWN

```python
IF twap_top_band_dir == UP AND twap_bottom_band_dir == DOWN AND price_dir == DOWN:
    # EXTREME PRICE VOLATILITY
    extreme_volatility = 1

    # DOWNWARD MOMENTUM
    IF price_velocity < -momentum_threshold:
        downward_momentum = 1
    ELSE:
        downward_momentum = 0

    # TIME-BASED EXPANSION
    time_expansion = 1

    # TECHNICAL BREAKDOWN
    IF price < twap_lower_band:
        technical_breakdown = 1
    ELSE:
        technical_breakdown = 0

    # SIGNAL
    signal = BEARISH  # 0
    signal_type = 'MOMENTUM_SHORT'
    signal_strength = 2
```

---

### Scenario 4: Top UP, Bottom UP, Price UP

```python
IF twap_top_band_dir == UP AND twap_bottom_band_dir == UP AND price_dir == UP:
    # TWAP SHIFTING HIGHER
    twap_shifting_higher = 1

    # PRICE CONFIRMING TREND
    IF price > twap AND price_velocity > 0:
        price_confirming = 1
    ELSE:
        price_confirming = 0

    # BULLISH TIME CONSENSUS
    bullish_time_consensus = 1

    # TREND CONTINUATION
    trend_continuation = 1

    # SIGNAL
    signal = BULLISH  # 1
    signal_type = 'STRONG_LONG'
    signal_strength = 3
```

---

### Scenario 5: Top UP, Bottom UP, Price DOWN

```python
IF twap_top_band_dir == UP AND twap_bottom_band_dir == UP AND price_dir == DOWN:
    # TWAP SHIFTING HIGHER
    twap_shifting_higher = 1

    # PRICE FALLING WHILE BANDS RISE
    IF price < twap AND twap_velocity > 0:
        bearish_divergence = 1
    ELSE:
        bearish_divergence = 0

    # MEAN REVERSION SETUP
    IF abs(price - twap) > mean_reversion_threshold:
        mean_reversion = 1
    ELSE:
        mean_reversion = 0

    # SIGNAL
    signal = BULLISH  # 1 (buy the dip)
    signal_type = 'BUY_DIP'
    signal_strength = 2
```

---

### Scenario 7: Top DOWN, Bottom DOWN, Price DOWN

```python
IF twap_top_band_dir == DOWN AND twap_bottom_band_dir == DOWN AND price_dir == DOWN:
    # TWAP SHIFTING LOWER
    twap_shifting_lower = 1

    # PRICE CONFIRMING TREND
    IF price < twap AND price_velocity < 0:
        price_confirming = 1
    ELSE:
        price_confirming = 0

    # BEARISH TIME CONSENSUS
    bearish_time_consensus = 1

    # TREND CONTINUATION
    trend_continuation = 1

    # SIGNAL
    signal = BEARISH  # 0
    signal_type = 'STRONG_SHORT'
    signal_strength = 3
```

---

### Scenario 8: Top DOWN, Bottom DOWN, Price UP

```python
IF twap_top_band_dir == DOWN AND twap_bottom_band_dir == DOWN AND price_dir == UP:
    # TWAP SHIFTING LOWER
    twap_shifting_lower = 1

    # PRICE RISING WHILE BANDS FALL
    IF price > twap AND twap_velocity < 0:
        bullish_divergence = 1
    ELSE:
        bullish_divergence = 0

    # MEAN REVERSION SETUP
    IF abs(price - twap) > mean_reversion_threshold:
        mean_reversion = 1
    ELSE:
        mean_reversion = 0

    # SIGNAL
    signal = BEARISH  # 0 (short the rip)
    signal_type = 'SHORT_RIP'
    signal_strength = 2
```

---

## TWAP vs VWAP Divergence (KEY SIGNALS)

### TWAP UP, VWAP DOWN (DISTRIBUTION)

```python
IF twap_dir == UP AND vwap_dir == DOWN:
    # TIME SAYS BULLISH
    time_bullish = 1

    # VOLUME SAYS BEARISH
    volume_bearish = 1

    # RETAIL PUSHING UP
    IF avg_trade_size < retail_size_threshold:
        retail_driving = 1
    ELSE:
        retail_driving = 0

    # INSTITUTIONS SELLING
    IF avg_trade_size > institutional_size_threshold AND vwap_velocity < 0:
        institutional_selling = 1
    ELSE:
        institutional_selling = 0

    # DISTRIBUTION PATTERN
    IF retail_driving AND institutional_selling:
        distribution = 1
    ELSE:
        distribution = 0

    # SIGNAL
    IF distribution:
        signal = BEARISH  # 0 (fade rally / short)
        signal_type = 'DISTRIBUTION'
        signal_strength = 3  # Strong institutional signal
    ELSE:
        signal = NEUTRAL
        signal_type = 'CONFLICTED'
```

---

### TWAP DOWN, VWAP UP (ACCUMULATION)

```python
IF twap_dir == DOWN AND vwap_dir == UP:
    # TIME SAYS BEARISH
    time_bearish = 1

    # VOLUME SAYS BULLISH
    volume_bullish = 1

    # RETAIL PANICKING
    IF avg_trade_size < retail_size_threshold:
        retail_panicking = 1
    ELSE:
        retail_panicking = 0

    # INSTITUTIONS BUYING
    IF avg_trade_size > institutional_size_threshold AND vwap_velocity > 0:
        institutional_buying = 1
    ELSE:
        institutional_buying = 0

    # ACCUMULATION PATTERN
    IF retail_panicking AND institutional_buying:
        accumulation = 1
    ELSE:
        accumulation = 0

    # SIGNAL
    IF accumulation:
        signal = BULLISH  # 1 (buy dip / long)
        signal_type = 'ACCUMULATION'
        signal_strength = 3  # Strong institutional signal
    ELSE:
        signal = NEUTRAL
        signal_type = 'CONFLICTED'
```

---

### TWAP Rising Faster Than VWAP (RETAIL-DRIVEN RALLY)

```python
twap_velocity = d(twap) / dt
vwap_velocity = d(vwap) / dt

IF twap_velocity > vwap_velocity AND twap_velocity > 0:
    # PRICE MOMENTUM OUTPACING VOLUME
    price_outpacing_volume = 1

    # LOW VOLUME RALLY
    IF total_volume < avg_volume:
        low_volume_rally = 1
    ELSE:
        low_volume_rally = 0

    # RETAIL-DRIVEN MOVE
    IF num_small_trades > num_large_trades:
        retail_driven = 1
    ELSE:
        retail_driven = 0

    # WEAK FOUNDATION
    weak_foundation = 1

    # UNSUSTAINABLE TREND
    unsustainable = 1

    # SIGNAL
    signal = BEARISH  # 0 (fade / short)
    signal_type = 'FADE_WEAK_RALLY'
    signal_strength = 2
```

---

### VWAP Rising Faster Than TWAP (INSTITUTIONAL RALLY)

```python
twap_velocity = d(twap) / dt
vwap_velocity = d(vwap) / dt

IF vwap_velocity > twap_velocity AND vwap_velocity > 0:
    # VOLUME MOMENTUM OUTPACING PRICE
    volume_outpacing_price = 1

    # HIGH VOLUME ACCUMULATION
    IF total_volume > avg_volume * volume_multiplier:
        high_volume_accumulation = 1
    ELSE:
        high_volume_accumulation = 0

    # INSTITUTIONAL POSITIONING
    IF num_large_trades > num_small_trades:
        institutional_positioning = 1
    ELSE:
        institutional_positioning = 0

    # STRONG FOUNDATION
    strong_foundation = 1

    # SUSTAINABLE TREND
    sustainable = 1

    # SIGNAL
    signal = BULLISH  # 1 (follow / long)
    signal_type = 'INSTITUTIONAL_RALLY'
    signal_strength = 3
```

---

### TWAP Falling Faster Than VWAP (RETAIL PANIC)

```python
twap_velocity = d(twap) / dt
vwap_velocity = d(vwap) / dt

IF twap_velocity < vwap_velocity AND twap_velocity < 0:
    # PRICE DECLINE OUTPACING VOLUME
    price_outpacing_volume_down = 1

    # LOW VOLUME SELLOFF
    IF total_volume < avg_volume:
        low_volume_selloff = 1
    ELSE:
        low_volume_selloff = 0

    # RETAIL-DRIVEN PANIC
    retail_panic = 1

    # WEAK BREAKDOWN
    weak_breakdown = 1

    # UNSUSTAINABLE DECLINE
    unsustainable = 1

    # SIGNAL
    signal = BULLISH  # 1 (fade / long)
    signal_type = 'FADE_WEAK_SELLOFF'
    signal_strength = 2
```

---

### VWAP Falling Faster Than TWAP (INSTITUTIONAL SELLING)

```python
twap_velocity = d(twap) / dt
vwap_velocity = d(vwap) / dt

IF vwap_velocity < twap_velocity AND vwap_velocity < 0:
    # VOLUME DECLINE OUTPACING PRICE
    volume_outpacing_price_down = 1

    # HIGH VOLUME DISTRIBUTION
    IF total_volume > avg_volume * volume_multiplier:
        high_volume_distribution = 1
    ELSE:
        high_volume_distribution = 0

    # INSTITUTIONAL DUMPING
    institutional_dumping = 1

    # STRONG BREAKDOWN
    strong_breakdown = 1

    # SUSTAINABLE DECLINE
    sustainable = 1

    # SIGNAL
    signal = BEARISH  # 0 (follow / short)
    signal_type = 'INSTITUTIONAL_SELLOFF'
    signal_strength = 3
```

---

## Band Width Analysis

### TWAP Bands Wider Than VWAP Bands (RETAIL CHOP)

```python
twap_band_width = twap_upper - twap_lower
vwap_band_width = vwap_upper - vwap_lower

IF twap_band_width > vwap_band_width * width_ratio_threshold:
    # TIME VOLATILITY EXCEEDS VOLUME VOLATILITY
    time_vol_dominant = 1

    # RETAIL ACTIVITY DOMINANT
    retail_dominant = 1

    # HIGH FREQUENCY CHOP
    high_freq_chop = 1

    # LOW CONVICTION MARKET
    low_conviction = 1

    # INEFFICIENT PRICING
    inefficient_pricing = 1

    # SIGNAL
    signal = NEUTRAL  # None (range trade)
    signal_type = 'RANGE_TRADE'
    signal_strength = 0
```

---

### VWAP Bands Wider Than TWAP Bands (INSTITUTIONAL FLOW)

```python
twap_band_width = twap_upper - twap_lower
vwap_band_width = vwap_upper - vwap_lower

IF vwap_band_width > twap_band_width * width_ratio_threshold:
    # VOLUME VOLATILITY EXCEEDS TIME VOLATILITY
    volume_vol_dominant = 1

    # INSTITUTIONAL ACTIVITY DOMINANT
    institutional_dominant = 1

    # HIGH IMPACT TRADES
    high_impact_trades = 1

    # HIGH CONVICTION MARKET
    high_conviction = 1

    # EFFICIENT PRICING
    efficient_pricing = 1

    # SIGNAL
    signal = NEUTRAL  # None (trend trade - follow institutional flow)
    signal_type = 'TREND_TRADE'
    signal_strength = 0  # Wait for direction
```

---

### TWAP Bands Narrowing, VWAP Bands Widening

```python
twap_band_width_change = d(twap_band_width) / dt
vwap_band_width_change = d(vwap_band_width) / dt

IF twap_band_width_change < 0 AND vwap_band_width_change > 0:
    # TIME VOLATILITY COLLAPSING
    time_vol_collapsing = 1

    # VOLUME VOLATILITY EXPLODING
    volume_vol_exploding = 1

    # RETAIL EXHAUSTION
    retail_exhaustion = 1

    # INSTITUTIONAL PREPARATION
    institutional_preparing = 1

    # PRE-BREAKOUT PATTERN
    pre_breakout = 1

    # SIGNAL
    signal = NEUTRAL  # None (prepare for big move)
    signal_type = 'PREPARE_BIG_MOVE'
    signal_strength = 0  # Wait for institutional direction
```

---

## TWAP Snapshot Divergence

### Current TWAP Above All Historical Snapshots

```python
twap_snapshots = [twap_1min_ago, twap_5min_ago, twap_15min_ago, twap_30min_ago, twap_2hr_ago]

IF current_twap > max(twap_snapshots):
    # CONTINUOUS UPTREND
    continuous_uptrend = 1

    # SUSTAINED BUYING PRESSURE
    sustained_buying = 1

    # STRONG MOMENTUM
    strong_momentum = 1

    # TREND ACCELERATION
    trend_acceleration = 1

    # SIGNAL
    signal = BULLISH  # 1
    signal_type = 'MOMENTUM_LONG'
    signal_strength = 3
```

---

### Current TWAP Below All Historical Snapshots

```python
twap_snapshots = [twap_1min_ago, twap_5min_ago, twap_15min_ago, twap_30min_ago, twap_2hr_ago]

IF current_twap < min(twap_snapshots):
    # CONTINUOUS DOWNTREND
    continuous_downtrend = 1

    # SUSTAINED SELLING PRESSURE
    sustained_selling = 1

    # STRONG MOMENTUM
    strong_momentum = 1

    # TREND ACCELERATION
    trend_acceleration = 1

    # SIGNAL
    signal = BEARISH  # 0
    signal_type = 'MOMENTUM_SHORT'
    signal_strength = 3
```

---

### TWAP Snapshots Converging

```python
twap_snapshots = [twap_1min_ago, twap_5min_ago, twap_15min_ago, twap_30min_ago, twap_2hr_ago]
snapshot_std = std_dev(twap_snapshots)

IF snapshot_std < convergence_threshold:
    # ALL TIMEFRAMES ALIGNING
    timeframes_aligning = 1

    # VOLATILITY COLLAPSING
    volatility_collapsing = 1

    # CONSOLIDATION TIGHTENING
    consolidation_tightening = 1

    # DECISION POINT APPROACHING
    decision_approaching = 1

    # SIGNAL
    signal = NEUTRAL  # None (breakout imminent)
    signal_type = 'BREAKOUT_IMMINENT'
    signal_strength = 0
```

---

### TWAP Snapshots Diverging

```python
twap_snapshots = [twap_1min_ago, twap_5min_ago, twap_15min_ago, twap_30min_ago, twap_2hr_ago]
snapshot_std = std_dev(twap_snapshots)

IF snapshot_std > divergence_threshold:
    # TIMEFRAMES DISAGREEING
    timeframes_disagreeing = 1

    # VOLATILITY EXPANDING
    volatility_expanding = 1

    # TREND UNCERTAINTY
    trend_uncertainty = 1

    # MULTI-TIMEFRAME CONFLICT
    mtf_conflict = 1

    # SIGNAL
    signal = NEUTRAL  # None (wait for alignment)
    signal_type = 'WAIT_ALIGNMENT'
    signal_strength = 0
```

---

## TWAP Rate of Change

### TWAP Slope Steepening (Acceleration)

```python
twap_velocity = d(twap) / dt
twap_acceleration = d²(twap) / dt²

IF twap_acceleration > accel_threshold:
    # MOMENTUM INCREASING
    momentum_increasing = 1

    # TREND STRENGTHENING
    trend_strengthening = 1

    # CONVICTION BUILDING
    conviction_building = 1

    # BREAKOUT EXTENSION
    breakout_extension = 1

    # SIGNAL
    IF twap_velocity > 0:
        signal = BULLISH  # 1 (add to position)
        signal_type = 'ADD_LONG'
    ELSE:
        signal = BEARISH  # 0 (add to position)
        signal_type = 'ADD_SHORT'

    signal_strength = 3
```

---

### TWAP Slope Flattening (Deceleration)

```python
twap_velocity = d(twap) / dt
twap_acceleration = d²(twap) / dt²

IF twap_acceleration < -accel_threshold:
    # MOMENTUM DECREASING
    momentum_decreasing = 1

    # TREND WEAKENING
    trend_weakening = 1

    # CONVICTION FADING
    conviction_fading = 1

    # EXHAUSTION APPROACHING
    exhaustion_approaching = 1

    # SIGNAL
    signal = NEUTRAL  # None (tighten stops)
    signal_type = 'TIGHTEN_STOPS'
    signal_strength = 0
```

---

### TWAP Slope Inflection (Reversal)

```python
previous_twap_velocity = d(twap_previous) / dt
current_twap_velocity = d(twap_current) / dt

IF sign(previous_twap_velocity) != sign(current_twap_velocity):
    # MOMENTUM REVERSING
    momentum_reversing = 1

    # TREND ENDING
    trend_ending = 1

    # NEW TREND BEGINNING
    new_trend_beginning = 1

    # TURNING POINT
    turning_point = 1

    # SIGNAL
    IF current_twap_velocity > 0:
        signal = BULLISH  # 1 (reverse to long)
        signal_type = 'REVERSE_LONG'
    ELSE:
        signal = BEARISH  # 0 (reverse to short)
        signal_type = 'REVERSE_SHORT'

    signal_strength = 2
```

---

## Combined VWAP + VPOC + TWAP Super Signals

### All Three Rising, Price UP

```python
IF vwap_dir == UP AND vpoc_dir == UP AND twap_dir == UP AND price_dir == UP:
    # COMPLETE BULLISH CONSENSUS
    complete_bullish_consensus = 1

    # ALL MEASURES CONFIRMING
    all_confirming = 1

    # MAXIMUM CONFIDENCE
    maximum_confidence = 1

    # SIGNAL
    signal = BULLISH  # 1
    signal_type = 'MAXIMUM_LONG_CONFIDENCE'
    signal_strength = 3
```

---

### All Three Falling, Price DOWN

```python
IF vwap_dir == DOWN AND vpoc_dir == DOWN AND twap_dir == DOWN AND price_dir == DOWN:
    # COMPLETE BEARISH CONSENSUS
    complete_bearish_consensus = 1

    # ALL MEASURES CONFIRMING
    all_confirming = 1

    # MAXIMUM CONFIDENCE
    maximum_confidence = 1

    # SIGNAL
    signal = BEARISH  # 0
    signal_type = 'MAXIMUM_SHORT_CONFIDENCE'
    signal_strength = 3
```

---

### TWAP UP, VWAP/VPOC DOWN

```python
IF twap_dir == UP AND vwap_dir == DOWN AND vpoc_dir == DOWN:
    # TIME SAYS BULLISH
    time_bullish = 1

    # VOLUME/DISTRIBUTION SAYS BEARISH
    volume_bearish = 1

    # DISTRIBUTION PATTERN
    distribution_pattern = 1

    # INSTITUTIONS SELLING TO RETAIL
    institutions_selling = 1

    # SIGNAL
    signal = BEARISH  # 0 (distribution)
    signal_type = 'DISTRIBUTION_PATTERN'
    signal_strength = 3
```

---

### TWAP DOWN, VWAP/VPOC UP

```python
IF twap_dir == DOWN AND vwap_dir == UP AND vpoc_dir == UP:
    # TIME SAYS BEARISH
    time_bearish = 1

    # VOLUME/DISTRIBUTION SAYS BULLISH
    volume_bullish = 1

    # ACCUMULATION PATTERN
    accumulation_pattern = 1

    # INSTITUTIONS BUYING FROM RETAIL
    institutions_buying = 1

    # SIGNAL
    signal = BULLISH  # 1 (accumulation)
    signal_type = 'ACCUMULATION_PATTERN'
    signal_strength = 3
```

---

### All Three Bands Expanding

```python
vwap_expanding = (d(vwap_band_width)/dt) > 0
vpoc_expanding = (d(vah - val)/dt) > 0
twap_expanding = (d(twap_band_width)/dt) > 0

IF vwap_expanding AND vpoc_expanding AND twap_expanding:
    # EXPLOSIVE VOLATILITY ACROSS ALL MEASURES
    explosive_volatility_all = 1

    # MAJOR BREAKOUT/BREAKDOWN
    major_move = 1

    # MASSIVE MOVE IN PROGRESS
    massive_move = 1

    # SIGNAL
    IF price > vwap AND price > vpoc AND price > twap:
        signal = BULLISH  # 1 (major breakout)
        signal_type = 'MAJOR_BREAKOUT'
    ELIF price < vwap AND price < vpoc AND price < twap:
        signal = BEARISH  # 0 (major breakdown)
        signal_type = 'MAJOR_BREAKDOWN'
    ELSE:
        signal = NEUTRAL  # None (wait for direction)
        signal_type = 'WAIT_DIRECTION'

    signal_strength = 3
```

---

### All Three Bands Contracting

```python
vwap_contracting = (d(vwap_band_width)/dt) < 0
vpoc_contracting = (d(vah - val)/dt) < 0
twap_contracting = (d(twap_band_width)/dt) < 0

IF vwap_contracting AND vpoc_contracting AND twap_contracting:
    # COMPRESSION ACROSS ALL MEASURES
    compression_all = 1

    # EXTREME SQUEEZE
    extreme_squeeze = 1

    # VOLCANIC ERUPTION IMMINENT
    eruption_imminent = 1

    # SIGNAL
    signal = NEUTRAL  # None (wait for explosion)
    signal_type = 'EXTREME_SQUEEZE'
    signal_strength = 0  # Massive move coming, direction unknown
```

---

## Composite Feature Vector

```python
twap_features = {
    # Direction inputs (categorical: -1, 0, 1)
    'twap_top_band_dir': UP,      # 1
    'twap_bottom_band_dir': UP,    # 1
    'twap_dir': UP,                # 1
    'price_dir': UP,               # 1

    # TWAP vs VWAP comparison
    'vwap_dir': DOWN,              # -1
    'divergence_twap_vwap': 1,     # TWAP up, VWAP down

    # Component flags (binary: 0 or 1)
    'time_bullish': 1,
    'volume_bearish': 1,
    'retail_driving': 1,
    'institutional_selling': 1,
    'distribution': 1,
    'accumulation': 0,
    'retail_dominant': 0,
    'institutional_dominant': 0,
    'pre_breakout': 0,
    'continuous_uptrend': 1,
    'timeframes_aligning': 0,
    'momentum_increasing': 1,
    # ... all condition flags

    # Band width metrics (continuous)
    'twap_band_width': 15.2,
    'vwap_band_width': 12.8,
    'band_width_ratio': 1.19,  # TWAP / VWAP

    # Velocity/acceleration (continuous)
    'twap_velocity': 0.0042,
    'twap_acceleration': 0.00018,
    'vwap_velocity': -0.0023,

    # Snapshot metrics (continuous)
    'snapshot_std_dev': 2.4,
    'snapshot_trend': 1,  # Increasing

    # Output signal (categorical: 0, 1, or None)
    'signal': BEARISH,  # 0 (distribution pattern)

    # Signal type (categorical encoded as integer)
    'signal_type': 'DISTRIBUTION',  # Or: 0

    # Signal strength (0-3)
    'signal_strength': 3,
}
```

---

## Signal Type Encoding

```python
TWAP_SIGNAL_TYPE_ENCODING = {
    'MOMENTUM_LONG': 0,
    'MOMENTUM_SHORT': 1,
    'STRONG_LONG': 2,
    'BUY_DIP': 3,
    'STRONG_SHORT': 4,
    'SHORT_RIP': 5,
    'DISTRIBUTION': 6,
    'ACCUMULATION': 7,
    'FADE_WEAK_RALLY': 8,
    'INSTITUTIONAL_RALLY': 9,
    'FADE_WEAK_SELLOFF': 10,
    'INSTITUTIONAL_SELLOFF': 11,
    'RANGE_TRADE': 12,
    'TREND_TRADE': 13,
    'PREPARE_BIG_MOVE': 14,
    'BREAKOUT_IMMINENT': 15,
    'WAIT_ALIGNMENT': 16,
    'ADD_LONG': 17,
    'ADD_SHORT': 18,
    'TIGHTEN_STOPS': 19,
    'REVERSE_LONG': 20,
    'REVERSE_SHORT': 21,
    'MAXIMUM_LONG_CONFIDENCE': 22,
    'MAXIMUM_SHORT_CONFIDENCE': 23,
    'DISTRIBUTION_PATTERN': 24,
    'ACCUMULATION_PATTERN': 25,
    'MAJOR_BREAKOUT': 26,
    'MAJOR_BREAKDOWN': 27,
    'WAIT_DIRECTION': 28,
    'EXTREME_SQUEEZE': 29,
}
```

---

**Last Updated**: 2025-09-30
**Use Case**: Executable TWAP band signal conditions + TWAP vs VWAP divergence for macro-state encoding
**Total Scenarios**: 25+ key conditions covered (48 total in conceptual doc)
**Key Feature**: Retail vs Institutional detection via TWAP/VWAP velocity comparison
