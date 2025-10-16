# VWAP Band Signals - IF-THEN Conditions

**Purpose**: Convert all 24 VWAP band movement scenarios into executable IF-THEN logic

**Output Format**: Binary (0/1) signals and categorical states

---

## Direction Encoding

```python
# Band and price directions
UP = 1
FLAT = 0
DOWN = -1

# Signal outputs
BULLISH = 1
BEARISH = 0
NEUTRAL = None
```

---

## Scenario 1: Top UP, Bottom DOWN, Price UP

```python
IF vwap_top_band_dir == UP AND vwap_bottom_band_dir == DOWN AND price_dir == UP:
    # EXPLOSIVE VOLATILITY
    explosive_volatility = 1

    # STRONG UPWARD MOMENTUM
    IF price_velocity > high_momentum_threshold:
        strong_upward_momentum = 1
    ELSE:
        strong_upward_momentum = 0

    # UNCERTAINTY/PANIC
    IF band_expansion_rate > panic_threshold:
        market_panic = 1
    ELSE:
        market_panic = 0

    # BREAKOUT IN PROGRESS
    IF price > vwap_upper_band:
        breakout_in_progress = 1
    ELSE:
        breakout_in_progress = 0

    # SIGNAL
    signal = BULLISH  # 1
    signal_type = 'MOMENTUM_LONG'
    signal_strength = 2  # High risk, high reward
```

---

## Scenario 2: Top UP, Bottom DOWN, Price DOWN

```python
IF vwap_top_band_dir == UP AND vwap_bottom_band_dir == DOWN AND price_dir == DOWN:
    # EXPLOSIVE VOLATILITY
    explosive_volatility = 1

    # STRONG DOWNWARD MOMENTUM
    IF price_velocity < -high_momentum_threshold:
        strong_downward_momentum = 1
    ELSE:
        strong_downward_momentum = 0

    # UNCERTAINTY/PANIC
    IF band_expansion_rate > panic_threshold:
        market_panic = 1
    ELSE:
        market_panic = 0

    # BREAKDOWN IN PROGRESS
    IF price < vwap_lower_band:
        breakdown_in_progress = 1
    ELSE:
        breakdown_in_progress = 0

    # SIGNAL
    signal = BEARISH  # 0
    signal_type = 'MOMENTUM_SHORT'
    signal_strength = 2  # High risk, high reward
```

---

## Scenario 3: Top UP, Bottom DOWN, Price FLAT

```python
IF vwap_top_band_dir == UP AND vwap_bottom_band_dir == DOWN AND price_dir == FLAT:
    # EXPLOSIVE VOLATILITY
    explosive_volatility = 1

    # EQUILIBRIUM BATTLE
    IF abs(price - vwap) < equilibrium_threshold:
        equilibrium_battle = 1
    ELSE:
        equilibrium_battle = 0

    # DECISION POINT
    decision_point = 1

    # COMPRESSION BUILDING
    IF band_width > compression_threshold:
        compression_building = 1
    ELSE:
        compression_building = 0

    # SIGNAL
    signal = NEUTRAL  # None
    signal_type = 'WAIT_BREAKOUT'
    signal_strength = 0  # Don't trade yet
```

---

## Scenario 4: Top UP, Bottom UP, Price UP

```python
IF vwap_top_band_dir == UP AND vwap_bottom_band_dir == UP AND price_dir == UP:
    # VWAP SHIFTING HIGHER
    vwap_shifting_higher = 1

    # PRICE CONFIRMING TREND
    IF price > vwap AND price_velocity > 0:
        price_confirming_trend = 1
    ELSE:
        price_confirming_trend = 0

    # BULLISH CONSENSUS
    IF all([vwap_top_band_dir == UP, vwap_bottom_band_dir == UP, price_dir == UP]):
        bullish_consensus = 1
    ELSE:
        bullish_consensus = 0

    # TREND CONTINUATION
    IF vwap_velocity > trend_threshold:
        trend_continuation = 1
    ELSE:
        trend_continuation = 0

    # SIGNAL
    signal = BULLISH  # 1
    signal_type = 'STRONG_LONG'
    signal_strength = 3  # Low risk, high confidence
```

---

## Scenario 5: Top UP, Bottom UP, Price DOWN

```python
IF vwap_top_band_dir == UP AND vwap_bottom_band_dir == UP AND price_dir == DOWN:
    # VWAP SHIFTING HIGHER
    vwap_shifting_higher = 1

    # PRICE FALLING WHILE BANDS RISE
    IF price < vwap AND vwap_velocity > 0:
        bearish_divergence = 1
    ELSE:
        bearish_divergence = 0

    # MEAN REVERSION SETUP
    IF abs(price - vwap) > mean_reversion_threshold:
        mean_reversion_setup = 1
    ELSE:
        mean_reversion_setup = 0

    # SIGNAL
    signal = BULLISH  # 1 (buy the dip)
    signal_type = 'BUY_DIP'
    signal_strength = 2  # Mean reversion trade
```

---

## Scenario 6: Top UP, Bottom UP, Price FLAT

```python
IF vwap_top_band_dir == UP AND vwap_bottom_band_dir == UP AND price_dir == FLAT:
    # VWAP DRIFTING HIGHER
    vwap_drifting_higher = 1

    # PRICE RESISTING MOVE
    IF abs(price_velocity) < resistance_threshold:
        price_resisting = 1
    ELSE:
        price_resisting = 0

    # LAGGING PRICE ACTION
    IF vwap_velocity > price_velocity:
        lagging_price = 1
    ELSE:
        lagging_price = 0

    # COILING FOR MOVE UP
    coiling_for_move = 1

    # SIGNAL
    signal = BULLISH  # 1 (patient long)
    signal_type = 'PATIENT_LONG'
    signal_strength = 1  # Wait for confirmation
```

---

## Scenario 7: Top DOWN, Bottom DOWN, Price DOWN

```python
IF vwap_top_band_dir == DOWN AND vwap_bottom_band_dir == DOWN AND price_dir == DOWN:
    # VWAP SHIFTING LOWER
    vwap_shifting_lower = 1

    # PRICE CONFIRMING TREND
    IF price < vwap AND price_velocity < 0:
        price_confirming_trend = 1
    ELSE:
        price_confirming_trend = 0

    # BEARISH CONSENSUS
    IF all([vwap_top_band_dir == DOWN, vwap_bottom_band_dir == DOWN, price_dir == DOWN]):
        bearish_consensus = 1
    ELSE:
        bearish_consensus = 0

    # TREND CONTINUATION
    IF vwap_velocity < -trend_threshold:
        trend_continuation = 1
    ELSE:
        trend_continuation = 0

    # SIGNAL
    signal = BEARISH  # 0
    signal_type = 'STRONG_SHORT'
    signal_strength = 3  # Low risk, high confidence
```

---

## Scenario 8: Top DOWN, Bottom DOWN, Price UP

```python
IF vwap_top_band_dir == DOWN AND vwap_bottom_band_dir == DOWN AND price_dir == UP:
    # VWAP SHIFTING LOWER
    vwap_shifting_lower = 1

    # PRICE RISING WHILE BANDS FALL
    IF price > vwap AND vwap_velocity < 0:
        bullish_divergence = 1
    ELSE:
        bullish_divergence = 0

    # MEAN REVERSION SETUP
    IF abs(price - vwap) > mean_reversion_threshold:
        mean_reversion_setup = 1
    ELSE:
        mean_reversion_setup = 0

    # SIGNAL
    signal = BEARISH  # 0 (short the rip)
    signal_type = 'SHORT_RIP'
    signal_strength = 2  # Mean reversion trade
```

---

## Scenario 9: Top DOWN, Bottom DOWN, Price FLAT

```python
IF vwap_top_band_dir == DOWN AND vwap_bottom_band_dir == DOWN AND price_dir == FLAT:
    # VWAP DRIFTING LOWER
    vwap_drifting_lower = 1

    # PRICE RESISTING MOVE
    IF abs(price_velocity) < resistance_threshold:
        price_resisting = 1
    ELSE:
        price_resisting = 0

    # LAGGING PRICE ACTION
    IF abs(vwap_velocity) > abs(price_velocity):
        lagging_price = 1
    ELSE:
        lagging_price = 0

    # COILING FOR MOVE DOWN
    coiling_for_move = 1

    # SIGNAL
    signal = BEARISH  # 0 (patient short)
    signal_type = 'PATIENT_SHORT'
    signal_strength = 1  # Wait for confirmation
```

---

## Scenario 10: Top UP, Bottom FLAT, Price UP

```python
IF vwap_top_band_dir == UP AND vwap_bottom_band_dir == FLAT AND price_dir == UP:
    # UPPER VOLATILITY EXPANDING
    upper_volatility_expanding = 1

    # LOWER SUPPORT HOLDING
    lower_support_stable = 1

    # ASYMMETRIC BULLISH
    asymmetric_bullish = 1

    # STRONG FLOOR ESTABLISHED
    IF vwap_lower_band_velocity == 0:
        strong_floor = 1
    ELSE:
        strong_floor = 0

    # SIGNAL
    signal = BULLISH  # 1
    signal_type = 'AGGRESSIVE_LONG'
    signal_strength = 3  # Strong support + upside momentum
```

---

## Scenario 11: Top UP, Bottom FLAT, Price DOWN

```python
IF vwap_top_band_dir == UP AND vwap_bottom_band_dir == FLAT AND price_dir == DOWN:
    # UPPER VOLATILITY EXPANDING
    upper_volatility_expanding = 1

    # PRICE REJECTING HIGHS
    price_rejecting_highs = 1

    # DIVERGENCE BUILDING
    divergence = 1

    # CONFUSED MARKET
    confused_market = 1

    # SIGNAL
    signal = NEUTRAL  # None
    signal_type = 'NO_TRADE'
    signal_strength = 0  # Conflicting signals
```

---

## Scenario 12: Top UP, Bottom FLAT, Price FLAT

```python
IF vwap_top_band_dir == UP AND vwap_bottom_band_dir == FLAT AND price_dir == FLAT:
    # UPPER BAND PROBING
    upper_probing = 1

    # LOWER SUPPORT SOLID
    lower_support_solid = 1

    # COILING BULLISH
    coiling_bullish = 1

    # ACCUMULATION PHASE
    accumulation = 1

    # SIGNAL
    signal = BULLISH  # 1 (anticipate long)
    signal_type = 'ANTICIPATE_LONG'
    signal_strength = 1  # Wait for trigger
```

---

## Scenario 13: Top DOWN, Bottom FLAT, Price DOWN

```python
IF vwap_top_band_dir == DOWN AND vwap_bottom_band_dir == FLAT AND price_dir == DOWN:
    # UPPER RESISTANCE HARDENING
    upper_resistance = 1

    # LOWER VOLATILITY EXPANDING
    lower_volatility_expanding = 1

    # ASYMMETRIC BEARISH
    asymmetric_bearish = 1

    # BREAKDOWN POTENTIAL
    breakdown_potential = 1

    # SIGNAL
    signal = BEARISH  # 0
    signal_type = 'AGGRESSIVE_SHORT'
    signal_strength = 3  # Strong resistance + downside momentum
```

---

## Scenario 14: Top DOWN, Bottom FLAT, Price UP

```python
IF vwap_top_band_dir == DOWN AND vwap_bottom_band_dir == FLAT AND price_dir == UP:
    # UPPER BAND COMPRESSING
    upper_compressing = 1

    # PRICE RISING INTO CEILING
    rising_into_ceiling = 1

    # REJECTION LIKELY
    rejection_likely = 1

    # TRAPPED BULLS
    trapped_bulls = 1

    # SIGNAL
    signal = BEARISH  # 0 (fade the move)
    signal_type = 'FADE_SHORT'
    signal_strength = 2  # Rejection setup
```

---

## Scenario 15: Top DOWN, Bottom FLAT, Price FLAT

```python
IF vwap_top_band_dir == DOWN AND vwap_bottom_band_dir == FLAT AND price_dir == FLAT:
    # UPPER RESISTANCE FORMING
    upper_resistance = 1

    # LOWER SUPPORT HOLDING
    lower_support = 1

    # RANGE COMPRESSION
    range_compression = 1

    # BEARISH BIAS
    bearish_bias = 1

    # SIGNAL
    signal = BEARISH  # 0 (anticipate short)
    signal_type = 'ANTICIPATE_SHORT'
    signal_strength = 1  # Wait for trigger
```

---

## Scenario 16: Top FLAT, Bottom UP, Price UP

```python
IF vwap_top_band_dir == FLAT AND vwap_bottom_band_dir == UP AND price_dir == UP:
    # UPPER CEILING ESTABLISHED
    upper_ceiling = 1

    # LOWER SUPPORT RISING
    lower_rising = 1

    # PRICE SQUEEZED HIGHER
    squeezed_higher = 1

    # BULLISH SQUEEZE
    bullish_squeeze = 1

    # SIGNAL
    signal = BULLISH  # 1
    signal_type = 'RIDE_SQUEEZE_LONG'
    signal_strength = 3  # Forced buying
```

---

## Scenario 17: Top FLAT, Bottom UP, Price DOWN

```python
IF vwap_top_band_dir == FLAT AND vwap_bottom_band_dir == UP AND price_dir == DOWN:
    # UPPER CEILING HOLDING
    upper_ceiling = 1

    # LOWER FLOOR RISING
    lower_rising = 1

    # PRICE FIGHTING COMPRESSION
    fighting_compression = 1

    # COMPRESSION PARADOX
    paradox = 1

    # SIGNAL
    signal = NEUTRAL  # None
    signal_type = 'WAIT_CONFLICT'
    signal_strength = 0  # Conflicting
```

---

## Scenario 18: Top FLAT, Bottom UP, Price FLAT

```python
IF vwap_top_band_dir == FLAT AND vwap_bottom_band_dir == UP AND price_dir == FLAT:
    # UPPER CEILING FIRM
    upper_firm = 1

    # LOWER FLOOR RISING
    lower_rising = 1

    # NARROWING RANGE
    narrowing_range = 1

    # COILING ENERGY
    coiling = 1

    # SIGNAL
    signal = BULLISH  # 1 (prepare long)
    signal_type = 'PREPARE_LONG'
    signal_strength = 2  # Strong setup
```

---

## Scenario 19: Top FLAT, Bottom DOWN, Price DOWN

```python
IF vwap_top_band_dir == FLAT AND vwap_bottom_band_dir == DOWN AND price_dir == DOWN:
    # UPPER CEILING LOCKED
    upper_locked = 1

    # LOWER SUPPORT COLLAPSING
    lower_collapsing = 1

    # PRICE FOLLOWING FLOOR
    following_floor = 1

    # BEARISH SQUEEZE
    bearish_squeeze = 1

    # SIGNAL
    signal = BEARISH  # 0
    signal_type = 'RIDE_SQUEEZE_SHORT'
    signal_strength = 3  # Forced selling
```

---

## Scenario 20: Top FLAT, Bottom DOWN, Price UP

```python
IF vwap_top_band_dir == FLAT AND vwap_bottom_band_dir == DOWN AND price_dir == UP:
    # UPPER RESISTANCE FIRM
    upper_firm = 1

    # LOWER SUPPORT VANISHING
    lower_vanishing = 1

    # PRICE RISING INTO TRAP
    rising_into_trap = 1

    # DANGEROUS SETUP
    dangerous = 1

    # SIGNAL
    signal = BEARISH  # 0 (avoid / short top)
    signal_type = 'SHORT_TOP'
    signal_strength = 2  # Risky setup
```

---

## Scenario 21: Top FLAT, Bottom DOWN, Price FLAT

```python
IF vwap_top_band_dir == FLAT AND vwap_bottom_band_dir == DOWN AND price_dir == FLAT:
    # UPPER CEILING FIRM
    upper_firm = 1

    # LOWER FLOOR FALLING
    lower_falling = 1

    # NARROWING RANGE
    narrowing_range = 1

    # COILING ENERGY
    coiling = 1

    # SIGNAL
    signal = BEARISH  # 0 (prepare short)
    signal_type = 'PREPARE_SHORT'
    signal_strength = 2  # Strong setup
```

---

## Scenario 22: Top FLAT, Bottom FLAT, Price UP

```python
IF vwap_top_band_dir == FLAT AND vwap_bottom_band_dir == FLAT AND price_dir == UP:
    # RANGE LOCKED
    range_locked = 1

    # PRICE APPROACHING RESISTANCE
    approaching_resistance = 1

    # REJECTION LIKELY
    rejection_likely = 1

    # RANGE BOUND MARKET
    range_bound = 1

    # SIGNAL
    signal = BEARISH  # 0 (fade at resistance)
    signal_type = 'FADE_RESISTANCE'
    signal_strength = 2  # Mean reversion
```

---

## Scenario 23: Top FLAT, Bottom FLAT, Price DOWN

```python
IF vwap_top_band_dir == FLAT AND vwap_bottom_band_dir == FLAT AND price_dir == DOWN:
    # RANGE LOCKED
    range_locked = 1

    # PRICE APPROACHING SUPPORT
    approaching_support = 1

    # BOUNCE LIKELY
    bounce_likely = 1

    # RANGE BOUND MARKET
    range_bound = 1

    # SIGNAL
    signal = BULLISH  # 1 (buy at support)
    signal_type = 'BUY_SUPPORT'
    signal_strength = 2  # Mean reversion
```

---

## Scenario 24: Top FLAT, Bottom FLAT, Price FLAT

```python
IF vwap_top_band_dir == FLAT AND vwap_bottom_band_dir == FLAT AND price_dir == FLAT:
    # COMPLETE STASIS
    complete_stasis = 1

    # EXTREME COMPRESSION
    extreme_compression = 1

    # VOLATILITY COLLAPSE
    volatility_collapse = 1

    # CALM BEFORE STORM
    calm_before_storm = 1

    # SIGNAL
    signal = NEUTRAL  # None
    signal_type = 'WAIT_CATALYST'
    signal_strength = 0  # Massive move coming, direction unknown
```

---

## Composite Feature Vector

```python
vwap_band_features = {
    # Direction inputs (categorical: -1, 0, 1)
    'vwap_top_band_dir': UP,      # 1
    'vwap_bottom_band_dir': DOWN,  # -1
    'price_dir': UP,               # 1

    # Component flags (binary: 0 or 1)
    'explosive_volatility': 1,
    'strong_upward_momentum': 1,
    'market_panic': 0,
    'breakout_in_progress': 1,
    'vwap_shifting_higher': 0,
    'price_confirming_trend': 1,
    'bullish_consensus': 0,
    'bearish_divergence': 0,
    'mean_reversion_setup': 0,
    # ... all condition flags

    # Output signal (categorical: 0, 1, or None)
    'signal': BULLISH,  # 1

    # Signal type (categorical encoded as integer)
    'signal_type': 'MOMENTUM_LONG',  # Or: 0, 1, 2, ...

    # Signal strength (0-3)
    'signal_strength': 2,
}
```

---

## Signal Type Encoding

```python
SIGNAL_TYPE_ENCODING = {
    'MOMENTUM_LONG': 0,
    'MOMENTUM_SHORT': 1,
    'WAIT_BREAKOUT': 2,
    'STRONG_LONG': 3,
    'BUY_DIP': 4,
    'PATIENT_LONG': 5,
    'STRONG_SHORT': 6,
    'SHORT_RIP': 7,
    'PATIENT_SHORT': 8,
    'AGGRESSIVE_LONG': 9,
    'NO_TRADE': 10,
    'ANTICIPATE_LONG': 11,
    'AGGRESSIVE_SHORT': 12,
    'FADE_SHORT': 13,
    'ANTICIPATE_SHORT': 14,
    'RIDE_SQUEEZE_LONG': 15,
    'WAIT_CONFLICT': 16,
    'PREPARE_LONG': 17,
    'RIDE_SQUEEZE_SHORT': 18,
    'SHORT_TOP': 19,
    'PREPARE_SHORT': 20,
    'FADE_RESISTANCE': 21,
    'BUY_SUPPORT': 22,
    'WAIT_CATALYST': 23,
}
```

---

**Last Updated**: 2025-09-30
**Use Case**: Executable VWAP band signal conditions for macro-state encoding
**Total Scenarios**: 24 complete IF-THEN conditions
