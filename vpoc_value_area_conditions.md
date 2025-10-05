# VPOC & Value Area Signals - IF-THEN Conditions

**Purpose**: Convert VPOC and Value Area movement scenarios into executable IF-THEN logic

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

## Core Value Area Scenarios

### Scenario 1: VAH UP, VAL DOWN, VPOC UP

```python
IF vah_dir == UP AND val_dir == DOWN AND vpoc_dir == UP:
    # VALUE AREA EXPANDING BOTH DIRECTIONS
    va_expanding = 1

    # VPOC SHIFTING HIGHER
    IF vpoc_drift_rate > drift_threshold:
        vpoc_rising = 1
    ELSE:
        vpoc_rising = 0

    # BULLISH EXPANSION
    IF vah_velocity > val_velocity:
        bullish_expansion = 1
    ELSE:
        bullish_expansion = 0

    # PRICE DISCOVERY UPWARD
    IF price > vpoc AND price > vwap:
        price_discovery_up = 1
    ELSE:
        price_discovery_up = 0

    # SIGNAL
    signal = BULLISH  # 1
    signal_type = 'BULLISH_EXPLORATION'
    signal_strength = 2
```

---

### Scenario 2: VAH UP, VAL DOWN, VPOC DOWN

```python
IF vah_dir == UP AND val_dir == DOWN AND vpoc_dir == DOWN:
    # VALUE AREA EXPANDING BOTH DIRECTIONS
    va_expanding = 1

    # VPOC SHIFTING LOWER
    IF vpoc_drift_rate < -drift_threshold:
        vpoc_falling = 1
    ELSE:
        vpoc_falling = 0

    # BEARISH EXPANSION
    IF abs(val_velocity) > vah_velocity:
        bearish_expansion = 1
    ELSE:
        bearish_expansion = 0

    # PRICE DISCOVERY DOWNWARD
    IF price < vpoc AND price < vwap:
        price_discovery_down = 1
    ELSE:
        price_discovery_down = 0

    # SIGNAL
    signal = BEARISH  # 0
    signal_type = 'BEARISH_EXPLORATION'
    signal_strength = 2
```

---

### Scenario 3: VAH UP, VAL DOWN, VPOC FLAT

```python
IF vah_dir == UP AND val_dir == DOWN AND vpoc_dir == FLAT:
    # VALUE AREA EXPLODING OUTWARD
    va_exploding = 1

    # VPOC UNCHANGED
    vpoc_anchored = 1

    # EXTREME VOLATILITY
    IF (vah - val) > extreme_width_threshold:
        extreme_volatility = 1
    ELSE:
        extreme_volatility = 0

    # BALANCED CHAOS
    balanced_chaos = 1

    # SIGNAL
    signal = NEUTRAL  # None
    signal_type = 'WAIT_VPOC_SHIFT'
    signal_strength = 0
```

---

### Scenario 4: VAH UP, VAL UP, VPOC UP

```python
IF vah_dir == UP AND val_dir == UP AND vpoc_dir == UP:
    # ENTIRE VALUE AREA SHIFTING HIGHER
    va_shifting_higher = 1

    # VPOC CONFIRMING UPWARD
    IF vpoc_drift_rate > strong_drift_threshold:
        vpoc_confirming = 1
    ELSE:
        vpoc_confirming = 0

    # BULLISH CONSENSUS
    consensus_bullish = 1

    # TREND CONTINUATION UP
    IF price > vpoc AND price > vah:
        trend_continuation = 1
    ELSE:
        trend_continuation = 0

    # SIGNAL
    signal = BULLISH  # 1
    signal_type = 'STRONG_LONG'
    signal_strength = 3  # Complete agreement
```

---

### Scenario 5: VAH UP, VAL UP, VPOC DOWN

```python
IF vah_dir == UP AND val_dir == UP AND vpoc_dir == DOWN:
    # VALUE AREA RISING
    va_rising = 1

    # VPOC FALLING WHILE AREA RISES
    divergence = 1

    # WEAK RALLY
    IF volume_at_vpoc > volume_threshold:
        weak_rally = 1
    ELSE:
        weak_rally = 0

    # SIGNAL
    signal = BEARISH  # 0 (fade the rally)
    signal_type = 'FADE_RALLY'
    signal_strength = 2
```

---

### Scenario 6: VAH DOWN, VAL DOWN, VPOC DOWN

```python
IF vah_dir == DOWN AND val_dir == DOWN AND vpoc_dir == DOWN:
    # ENTIRE VALUE AREA SHIFTING LOWER
    va_shifting_lower = 1

    # VPOC CONFIRMING DOWNWARD
    IF vpoc_drift_rate < -strong_drift_threshold:
        vpoc_confirming = 1
    ELSE:
        vpoc_confirming = 0

    # BEARISH CONSENSUS
    consensus_bearish = 1

    # TREND CONTINUATION DOWN
    IF price < vpoc AND price < val:
        trend_continuation = 1
    ELSE:
        trend_continuation = 0

    # SIGNAL
    signal = BEARISH  # 0
    signal_type = 'STRONG_SHORT'
    signal_strength = 3  # Complete agreement
```

---

### Scenario 7: VAH DOWN, VAL DOWN, VPOC UP

```python
IF vah_dir == DOWN AND val_dir == DOWN AND vpoc_dir == UP:
    # VALUE AREA FALLING
    va_falling = 1

    # VPOC RISING WHILE AREA FALLS
    divergence = 1

    # WEAK SELLOFF
    IF volume_at_vpoc > volume_threshold:
        weak_selloff = 1
    ELSE:
        weak_selloff = 0

    # SIGNAL
    signal = BULLISH  # 1 (fade the drop)
    signal_type = 'FADE_DROP'
    signal_strength = 2
```

---

### Scenario 10: VAH UP, VAL FLAT, VPOC UP

```python
IF vah_dir == UP AND val_dir == FLAT AND vpoc_dir == UP:
    # UPPER VALUE AREA EXPANDING
    upper_expanding = 1

    # LOWER SUPPORT HOLDING
    lower_stable = 1

    # VPOC RISING
    vpoc_rising = 1

    # ASYMMETRIC BULLISH
    asymmetric_bullish = 1

    # SIGNAL
    signal = BULLISH  # 1
    signal_type = 'AGGRESSIVE_LONG'
    signal_strength = 3
```

---

### Scenario 13: VAH DOWN, VAL FLAT, VPOC DOWN

```python
IF vah_dir == DOWN AND val_dir == FLAT AND vpoc_dir == DOWN:
    # UPPER RESISTANCE FORMING
    upper_resistance = 1

    # LOWER SUPPORT HOLDING
    lower_stable = 1

    # VPOC FALLING
    vpoc_falling = 1

    # ASYMMETRIC BEARISH
    asymmetric_bearish = 1

    # SIGNAL
    signal = BEARISH  # 0
    signal_type = 'AGGRESSIVE_SHORT'
    signal_strength = 3
```

---

### Scenario 16: VAH FLAT, VAL UP, VPOC UP

```python
IF vah_dir == FLAT AND val_dir == UP AND vpoc_dir == UP:
    # UPPER CEILING LOCKED
    upper_locked = 1

    # LOWER FLOOR RISING
    lower_rising = 1

    # VPOC RISING
    vpoc_rising = 1

    # BULLISH SQUEEZE FROM BELOW
    bullish_squeeze = 1

    # SIGNAL
    signal = BULLISH  # 1
    signal_type = 'RIDE_SQUEEZE_LONG'
    signal_strength = 3
```

---

### Scenario 19: VAH FLAT, VAL DOWN, VPOC DOWN

```python
IF vah_dir == FLAT AND val_dir == DOWN AND vpoc_dir == DOWN:
    # UPPER CEILING LOCKED
    upper_locked = 1

    # LOWER SUPPORT COLLAPSING
    lower_collapsing = 1

    # VPOC FALLING
    vpoc_falling = 1

    # BEARISH SQUEEZE FROM ABOVE
    bearish_squeeze = 1

    # SIGNAL
    signal = BEARISH  # 0
    signal_type = 'RIDE_SQUEEZE_SHORT'
    signal_strength = 3
```

---

## VPOC Position Signals

### VPOC Outside Value Area - Above VAH

```python
IF vpoc > vah:
    # MOST VOLUME AT EXTREMES
    volume_at_extremes = 1

    # UPTREND STRUCTURE
    uptrend_structure = 1

    # BULLISH DISTRIBUTION
    bullish_distribution = 1

    # BREAKOUT CONFIRMATION
    IF price > vpoc:
        breakout_confirmed = 1
    ELSE:
        breakout_confirmed = 0

    # SIGNAL
    signal = BULLISH  # 1
    signal_type = 'TREND_LONG'
    signal_strength = 3
```

---

### VPOC Outside Value Area - Below VAL

```python
IF vpoc < val:
    # MOST VOLUME AT EXTREMES
    volume_at_extremes = 1

    # DOWNTREND STRUCTURE
    downtrend_structure = 1

    # BEARISH DISTRIBUTION
    bearish_distribution = 1

    # BREAKDOWN CONFIRMATION
    IF price < vpoc:
        breakdown_confirmed = 1
    ELSE:
        breakdown_confirmed = 0

    # SIGNAL
    signal = BEARISH  # 0
    signal_type = 'TREND_SHORT'
    signal_strength = 3
```

---

### VPOC at VAH

```python
IF abs(vpoc - vah) < tolerance:
    # FAIR VALUE AT UPPER EDGE
    fair_value_top = 1

    # BUYING EXHAUSTION POTENTIAL
    buying_exhaustion = 1

    # RESISTANCE FORMING
    resistance_forming = 1

    # REJECTION ZONE
    rejection_zone = 1

    # SIGNAL
    signal = BEARISH  # 0 (fade / short)
    signal_type = 'FADE_TOP'
    signal_strength = 2
```

---

### VPOC at VAL

```python
IF abs(vpoc - val) < tolerance:
    # FAIR VALUE AT LOWER EDGE
    fair_value_bottom = 1

    # SELLING EXHAUSTION POTENTIAL
    selling_exhaustion = 1

    # SUPPORT FORMING
    support_forming = 1

    # BOUNCE ZONE
    bounce_zone = 1

    # SIGNAL
    signal = BULLISH  # 1 (buy / long)
    signal_type = 'BUY_BOTTOM'
    signal_strength = 2
```

---

### VPOC at Center of Value Area

```python
IF abs(vpoc - ((vah + val) / 2)) < tolerance:
    # PERFECT BALANCE
    perfect_balance = 1

    # NEUTRAL MARKET
    neutral_market = 1

    # HEALTHY STRUCTURE
    healthy_structure = 1

    # RANGE BOUND
    range_bound = 1

    # SIGNAL
    signal = NEUTRAL  # None (range trade)
    signal_type = 'RANGE_TRADE'
    signal_strength = 0
```

---

## Price Relative to VPOC/Value Area

### Price Above VAH, VPOC Rising

```python
IF price > vah AND vpoc_drift_rate > drift_threshold:
    # BREAKOUT ABOVE VALUE AREA
    breakout_va = 1

    # VPOC FOLLOWING
    vpoc_confirming = 1

    # ACCEPTANCE OF HIGHER PRICES
    acceptance_higher = 1

    # TREND EXTENSION
    trend_extension = 1

    # SIGNAL
    signal = BULLISH  # 1
    signal_type = 'CONTINUATION_LONG'
    signal_strength = 3
```

---

### Price Above VAH, VPOC Falling

```python
IF price > vah AND vpoc_drift_rate < -drift_threshold:
    # BREAKOUT ABOVE VALUE AREA
    breakout_va = 1

    # VPOC NOT FOLLOWING
    vpoc_not_confirming = 1

    # REJECTION PATTERN
    rejection_pattern = 1

    # FAILED BREAKOUT
    failed_breakout = 1

    # SIGNAL
    signal = BEARISH  # 0 (fade / short)
    signal_type = 'FADE_BREAKOUT'
    signal_strength = 2
```

---

### Price Below VAL, VPOC Falling

```python
IF price < val AND vpoc_drift_rate < -drift_threshold:
    # BREAKDOWN BELOW VALUE AREA
    breakdown_va = 1

    # VPOC FOLLOWING
    vpoc_confirming = 1

    # ACCEPTANCE OF LOWER PRICES
    acceptance_lower = 1

    # TREND EXTENSION
    trend_extension = 1

    # SIGNAL
    signal = BEARISH  # 0
    signal_type = 'CONTINUATION_SHORT'
    signal_strength = 3
```

---

### Price Below VAL, VPOC Rising

```python
IF price < val AND vpoc_drift_rate > drift_threshold:
    # BREAKDOWN BELOW VALUE AREA
    breakdown_va = 1

    # VPOC NOT FOLLOWING
    vpoc_not_confirming = 1

    # REJECTION PATTERN
    rejection_pattern = 1

    # FAILED BREAKDOWN
    failed_breakdown = 1

    # SIGNAL
    signal = BULLISH  # 1 (fade / long)
    signal_type = 'FADE_BREAKDOWN'
    signal_strength = 2
```

---

### Price Inside Value Area, VPOC Above Price

```python
IF val < price < vah AND price < vpoc:
    # PRICE BELOW FAIR VALUE
    below_fair_value = 1

    # VPOC ACTING AS MAGNET
    vpoc_magnet = 1

    # MEAN REVERSION SETUP
    mean_reversion = 1

    # SUPPORT BUILDING
    support_building = 1

    # SIGNAL
    signal = BULLISH  # 1 (long / buy)
    signal_type = 'MEAN_REVERSION_LONG'
    signal_strength = 2
```

---

### Price Inside Value Area, VPOC Below Price

```python
IF val < price < vah AND price > vpoc:
    # PRICE ABOVE FAIR VALUE
    above_fair_value = 1

    # VPOC ACTING AS MAGNET
    vpoc_magnet = 1

    # MEAN REVERSION SETUP
    mean_reversion = 1

    # RESISTANCE BUILDING
    resistance_building = 1

    # SIGNAL
    signal = BEARISH  # 0 (short / sell)
    signal_type = 'MEAN_REVERSION_SHORT'
    signal_strength = 2
```

---

## Volume Distribution Patterns

### Value Area Narrow, High Volume at VPOC

```python
va_width = vah - val

IF va_width < narrow_threshold AND volume_at_vpoc_pct > 0.5:
    # TIGHT CONSENSUS
    tight_consensus = 1

    # STRONG ACCEPTANCE
    strong_acceptance = 1

    # STABLE EQUILIBRIUM
    stable_equilibrium = 1

    # LOW VOLATILITY
    low_volatility = 1

    # SIGNAL
    signal = NEUTRAL  # None (range bound)
    signal_type = 'RANGE_BOUND'
    signal_strength = 0
```

---

### Value Area Wide, Volume Dispersed

```python
va_width = vah - val

IF va_width > wide_threshold AND volume_at_vpoc_pct < 0.3:
    # NO CONSENSUS
    no_consensus = 1

    # WEAK ACCEPTANCE
    weak_acceptance = 1

    # UNSTABLE EQUILIBRIUM
    unstable_equilibrium = 1

    # HIGH VOLATILITY
    high_volatility = 1

    # SIGNAL
    signal = NEUTRAL  # None (breakout pending)
    signal_type = 'BREAKOUT_PENDING'
    signal_strength = 0
```

---

## VPOC Drift Rate Analysis

### VPOC Drift Rate Accelerating

```python
IF vpoc_drift_acceleration > accel_threshold:
    # MOMENTUM INCREASING
    momentum_increasing = 1

    # TREND STRENGTHENING
    trend_strengthening = 1

    # CONVICTION BUILDING
    conviction_building = 1

    # CHASE BEHAVIOR
    chase_behavior = 1

    # SIGNAL
    IF vpoc_drift_rate > 0:
        signal = BULLISH  # 1 (add to position)
        signal_type = 'ADD_LONG'
    ELSE:
        signal = BEARISH  # 0 (add to position)
        signal_type = 'ADD_SHORT'

    signal_strength = 3
```

---

### VPOC Drift Rate Decelerating

```python
IF vpoc_drift_acceleration < -accel_threshold:
    # MOMENTUM SLOWING
    momentum_slowing = 1

    # TREND WEAKENING
    trend_weakening = 1

    # CONVICTION FADING
    conviction_fading = 1

    # EXHAUSTION POTENTIAL
    exhaustion = 1

    # SIGNAL
    signal = NEUTRAL  # None (tighten stops)
    signal_type = 'TIGHTEN_STOPS'
    signal_strength = 0
```

---

## Composite Feature Vector

```python
vpoc_va_features = {
    # Direction inputs (categorical: -1, 0, 1)
    'vah_dir': UP,      # 1
    'val_dir': UP,      # 1
    'vpoc_dir': UP,     # 1

    # Component flags (binary: 0 or 1)
    'va_expanding': 0,
    'va_shifting_higher': 1,
    'vpoc_rising': 1,
    'vpoc_confirming': 1,
    'consensus_bullish': 1,
    'trend_continuation': 1,
    'divergence': 0,
    'weak_rally': 0,
    'asymmetric_bullish': 0,
    'bullish_squeeze': 0,
    'volume_at_extremes': 0,
    'fair_value_top': 0,
    'perfect_balance': 0,
    'breakout_va': 0,
    'mean_reversion': 0,
    # ... all condition flags

    # VPOC position (categorical)
    'vpoc_position': 'CENTER_VA',  # or ABOVE_VAH, BELOW_VAL, AT_VAH, AT_VAL

    # Value area width (continuous)
    'va_width': vah - val,
    'va_width_percentile': 0.65,  # Normalized

    # Volume concentration (continuous)
    'volume_at_vpoc_pct': 0.42,

    # VPOC drift metrics (continuous)
    'vpoc_drift_rate': 0.0023,
    'vpoc_drift_acceleration': 0.00015,

    # Output signal (categorical: 0, 1, or None)
    'signal': BULLISH,  # 1

    # Signal type (categorical encoded as integer)
    'signal_type': 'STRONG_LONG',  # Or: 3

    # Signal strength (0-3)
    'signal_strength': 3,
}
```

---

## Signal Type Encoding

```python
VPOC_SIGNAL_TYPE_ENCODING = {
    'BULLISH_EXPLORATION': 0,
    'BEARISH_EXPLORATION': 1,
    'WAIT_VPOC_SHIFT': 2,
    'STRONG_LONG': 3,
    'FADE_RALLY': 4,
    'STRONG_SHORT': 5,
    'FADE_DROP': 6,
    'AGGRESSIVE_LONG': 7,
    'AGGRESSIVE_SHORT': 8,
    'RIDE_SQUEEZE_LONG': 9,
    'RIDE_SQUEEZE_SHORT': 10,
    'TREND_LONG': 11,
    'TREND_SHORT': 12,
    'FADE_TOP': 13,
    'BUY_BOTTOM': 14,
    'RANGE_TRADE': 15,
    'CONTINUATION_LONG': 16,
    'FADE_BREAKOUT': 17,
    'CONTINUATION_SHORT': 18,
    'FADE_BREAKDOWN': 19,
    'MEAN_REVERSION_LONG': 20,
    'MEAN_REVERSION_SHORT': 21,
    'RANGE_BOUND': 22,
    'BREAKOUT_PENDING': 23,
    'ADD_LONG': 24,
    'ADD_SHORT': 25,
    'TIGHTEN_STOPS': 26,
}
```

---

**Last Updated**: 2025-09-30
**Use Case**: Executable VPOC/Value Area signal conditions for macro-state encoding
**Total Scenarios**: 26 key conditions covered (44 total in conceptual doc)
