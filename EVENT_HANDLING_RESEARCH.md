# Quantower Event Handling Architecture Research
## Strategy Event System for Signal Detection and Trade Execution

**Date**: 2025-10-03
**Purpose**: Document validated event handling patterns for CipherFeed multi-indicator strategy

---

## Executive Summary

Quantower strategies use an **event-driven architecture** with multiple event types for different data updates. Key findings:

- **Indicators use OnUpdate()** with UpdateReason to distinguish historical/real-time
- **Strategies use Symbol events** (NewQuote, NewLast, NewLevel2) for tick-level processing
- **HistoricalData.NewHistoryItem** for bar-level processing
- **Performance trade-off**: Tick events (high frequency) vs Bar events (lower frequency)
- **Multi-symbol**: Subscribe each symbol's events independently

---

## 1. Indicator Event Model: OnUpdate()

### OnUpdate Method Signature
```csharp
protected override void OnUpdate(UpdateArgs args)
{
    // args.Reason determines update type
}
```

### UpdateReason Enum Values
```csharp
public enum UpdateReason
{
    HistoricalBar,  // Processing historical data during load/backtest
    NewTick,        // Real-time tick update
    NewBar          // New bar started
}
```

### Usage Pattern
```csharp
protected override void OnUpdate(UpdateArgs args)
{
    // Option 1: Skip historical processing
    if (args.Reason == UpdateReason.HistoricalBar)
        return;

    // Option 2: Different logic for different updates
    if (args.Reason == UpdateReason.HistoricalBar)
    {
        // Historical calculation
    }
    else if (args.Reason == UpdateReason.NewTick)
    {
        // Real-time tick processing (highest frequency)
    }
    else if (args.Reason == UpdateReason.NewBar)
    {
        // New bar processing (lower frequency)
    }
}
```

### When OnUpdate Fires
- **During History Load**: Called for each historical bar with `UpdateReason.HistoricalBar`
- **Real-Time Ticks**: Called on every tick with `UpdateReason.NewTick`
- **Bar Open**: Called when new bar starts with `UpdateReason.NewBar`

### Best Practices
✅ **Do**: Use cumulative calculations that work across historical and real-time
✅ **Do**: Check UpdateReason when different logic is needed
✅ **Do**: Keep OnUpdate lightweight for performance
❌ **Don't**: Place orders during historical processing
❌ **Don't**: Perform heavy calculations on every tick

---

## 2. Strategy Event Model: Symbol Events

### Available Symbol Events

| Event | Trigger | Frequency | Use Case |
|-------|---------|-----------|----------|
| `NewQuote` | New Level 1 quote (bid/ask) | Very High | Price updates, spread monitoring |
| `NewLast` | New trade execution | High | Trade flow analysis, momentum |
| `NewLevel2` | Depth of market update | High | Order book analysis, VPOC |
| `NewDayBar` | Correctional quote from vendor | Low | Daily adjustments |
| `Updated` | Symbol metadata update | Very Low | Symbol changes |

### Event Subscription Pattern

```csharp
public class CipherFeedStrategy : Strategy
{
    private Symbol symbol;

    protected override void OnRun()
    {
        // Get symbol instance
        this.symbol = Core.GetSymbol(this.Symbol?.CreateInfo());

        if (this.symbol != null)
        {
            // Subscribe to events
            this.symbol.NewQuote += SymbolOnNewQuote;
            this.symbol.NewLast += SymbolOnNewLast;
            this.symbol.NewLevel2 += SymbolOnNewLevel2;

            Log("Subscribed to symbol events", StrategyLoggingLevel.Trading);
        }
    }

    protected override void OnStop()
    {
        if (this.symbol != null)
        {
            // Unsubscribe from events
            this.symbol.NewQuote -= SymbolOnNewQuote;
            this.symbol.NewLast -= SymbolOnNewLast;
            this.symbol.NewLevel2 -= SymbolOnNewLevel2;

            Log("Unsubscribed from symbol events", StrategyLoggingLevel.Trading);
        }
    }

    private void SymbolOnNewQuote(Symbol symbol, Quote quote)
    {
        // Handle quote update
        double bid = quote.Bid;
        double ask = quote.Ask;
        double spread = ask - bid;

        // Check indicators and signals here
    }

    private void SymbolOnNewLast(Symbol symbol, Last last)
    {
        // Handle trade update
        double price = last.Price;
        double volume = last.Volume;

        // Analyze trade flow
    }

    private void SymbolOnNewLevel2(Symbol symbol, Level2Quote level2)
    {
        // Handle depth of market update
        // Access order book for VPOC calculations
    }
}
```

### Event Handler Signatures
```csharp
// Quote event handler
private void SymbolOnNewQuote(Symbol symbol, Quote quote)

// Last trade event handler
private void SymbolOnNewLast(Symbol symbol, Last last)

// Level 2 event handler
private void SymbolOnNewLevel2(Symbol symbol, Level2Quote level2)

// Day bar event handler
private void SymbolOnNewDayBar(Symbol symbol, DayBar dayBar)

// Symbol update event handler
private void SymbolOnUpdated(Symbol symbol, SymbolEventArgs args)
```

---

## 3. Bar-Level Processing: HistoricalData Events

### NewHistoryItem Event

**Trigger**: When new bar is created
**Frequency**: Depends on period (1-min bar = 1x per minute)
**Use Case**: Bar close analysis, indicator recalculation

### Pattern for Bar-Level Strategy

```csharp
public class CipherFeedStrategy : Strategy
{
    private Symbol symbol;
    private HistoricalData m1BarHistory;  // 1-minute bars
    private HistoricalData m5BarHistory;  // 5-minute bars

    protected override void OnRun()
    {
        this.symbol = Core.GetSymbol(this.Symbol?.CreateInfo());

        if (this.symbol != null)
        {
            // Get historical data for different timeframes
            this.m1BarHistory = this.symbol.GetHistory(
                Period.MIN1,
                Core.TimeUtils.DateTimeUtcNow
            );

            this.m5BarHistory = this.symbol.GetHistory(
                Period.MIN5,
                Core.TimeUtils.DateTimeUtcNow
            );

            // Subscribe to new bar events
            this.m1BarHistory.NewHistoryItem += M1HistoryBarUpdate;
            this.m5BarHistory.NewHistoryItem += M5HistoryBarUpdate;

            Log("Subscribed to bar update events", StrategyLoggingLevel.Trading);
        }
    }

    protected override void OnStop()
    {
        if (this.m1BarHistory != null)
        {
            this.m1BarHistory.NewHistoryItem -= M1HistoryBarUpdate;
            this.m1BarHistory.Dispose();
        }

        if (this.m5BarHistory != null)
        {
            this.m5BarHistory.NewHistoryItem -= M5HistoryBarUpdate;
            this.m5BarHistory.Dispose();
        }
    }

    private void M1HistoryBarUpdate(object sender, HistoryEventArgs e)
    {
        // New 1-minute bar created
        HistoryItemBar bar = (HistoryItemBar)e.HistoryItem;

        double open = bar.Open;
        double high = bar.High;
        double low = bar.Low;
        double close = bar.Close;
        double volume = bar.Volume;

        // Check signals on bar close
        CheckSignals();
    }

    private void M5HistoryBarUpdate(object sender, HistoryEventArgs e)
    {
        // New 5-minute bar created
        // Higher timeframe confirmation
    }

    private void CheckSignals()
    {
        // Analyze all indicators
        // Generate trading signals
        // Execute trades if conditions met
    }
}
```

---

## 4. Multi-Symbol Event Handling

### Pattern for Multiple Symbols

```csharp
public class CipherFeedStrategy : Strategy
{
    private Dictionary<Symbol, SymbolEventHandlers> symbolHandlers;

    private class SymbolEventHandlers
    {
        public Action<Symbol, Quote> QuoteHandler;
        public Action<Symbol, Last> LastHandler;
        public Action<object, HistoryEventArgs> BarHandler;
        public HistoricalData HistoryData;
    }

    protected override void OnRun()
    {
        symbolHandlers = new Dictionary<Symbol, SymbolEventHandlers>();

        // Get all target symbols
        var targetSymbols = GetTargetCMESymbols();

        foreach (var symbol in targetSymbols)
        {
            // Create handlers for this symbol
            var handlers = new SymbolEventHandlers
            {
                QuoteHandler = (s, q) => OnSymbolQuote(symbol, q),
                LastHandler = (s, l) => OnSymbolLast(symbol, l),
                BarHandler = (sender, e) => OnSymbolBar(symbol, e)
            };

            // Subscribe to events
            symbol.NewQuote += handlers.QuoteHandler;
            symbol.NewLast += handlers.LastHandler;

            // Get historical data
            handlers.HistoryData = symbol.GetHistory(Period.MIN1, Core.TimeUtils.DateTimeUtcNow);
            handlers.HistoryData.NewHistoryItem += handlers.BarHandler;

            // Store handlers
            symbolHandlers[symbol] = handlers;

            Log($"Subscribed events for {symbol.Name}", StrategyLoggingLevel.Trading);
        }
    }

    protected override void OnStop()
    {
        foreach (var kvp in symbolHandlers)
        {
            var symbol = kvp.Key;
            var handlers = kvp.Value;

            // Unsubscribe from events
            symbol.NewQuote -= handlers.QuoteHandler;
            symbol.NewLast -= handlers.LastHandler;

            if (handlers.HistoryData != null)
            {
                handlers.HistoryData.NewHistoryItem -= handlers.BarHandler;
                handlers.HistoryData.Dispose();
            }
        }

        symbolHandlers.Clear();
    }

    private void OnSymbolQuote(Symbol symbol, Quote quote)
    {
        // Handle quote for specific symbol
        Log($"{symbol.Name} Quote: Bid={quote.Bid}, Ask={quote.Ask}");
    }

    private void OnSymbolLast(Symbol symbol, Last last)
    {
        // Handle trade for specific symbol
        Log($"{symbol.Name} Trade: Price={last.Price}, Vol={last.Volume}");
    }

    private void OnSymbolBar(Symbol symbol, HistoryEventArgs e)
    {
        // Handle new bar for specific symbol
        HistoryItemBar bar = (HistoryItemBar)e.HistoryItem;
        Log($"{symbol.Name} New Bar: Close={bar.Close}");

        // Check signals for this symbol
        CheckSignalsForSymbol(symbol);
    }

    private void CheckSignalsForSymbol(Symbol symbol)
    {
        // Symbol-specific signal analysis
    }
}
```

---

## 5. Performance Considerations

### Event Frequency Impact

| Event Type | Frequency | CPU Load | Best For |
|------------|-----------|----------|----------|
| NewTick (OnUpdate) | 100-1000+ /sec | Very High | HFT, scalping |
| NewQuote | 50-500 /sec | High | Quote monitoring |
| NewLast | 10-100 /sec | Medium | Trade flow |
| NewBar (1-min) | 60 /hour | Low | Bar-close signals |
| NewBar (5-min) | 12 /hour | Very Low | Higher TF confirmation |

### Performance Best Practices

✅ **Use Bar Events for Signal Generation**
```csharp
// Good: Check signals on bar close
m1BarHistory.NewHistoryItem += (s, e) => {
    CheckSignals();  // Once per minute
};
```

❌ **Avoid Heavy Processing on Ticks**
```csharp
// Bad: Complex calculations on every tick
symbol.NewQuote += (s, q) => {
    RecalculateAllIndicators();  // Too frequent!
};
```

✅ **Throttle Tick Processing**
```csharp
private DateTime lastSignalCheck = DateTime.MinValue;
private TimeSpan signalCheckInterval = TimeSpan.FromSeconds(5);

private void SymbolOnNewQuote(Symbol symbol, Quote quote)
{
    // Throttle to once every 5 seconds
    if (DateTime.Now - lastSignalCheck >= signalCheckInterval)
    {
        CheckSignals();
        lastSignalCheck = DateTime.Now;
    }
}
```

✅ **Use Appropriate Event for Task**
- **Price Updates**: NewQuote
- **Trade Analysis**: NewLast
- **Signal Generation**: NewHistoryItem (bar close)
- **Order Book**: NewLevel2

### Backtest Performance Warning

⚠️ **From Quantower Docs**:
> "Ticks aggregation may lead to high internet traffic consumption and computer processor load while backtesting if used with a long historical period"

**Solution**: Use bar-based processing for backtesting long periods.

---

## 6. CipherFeed Strategy Event Architecture

### Recommended Event Strategy

**For CipherFeed with VWAP/TWAP/VPOC/Momentum:**

```csharp
public class CipherFeedStrategy : Strategy, IVolumeAnalysisIndicator
{
    // Event processing strategy:
    // 1. Use OnInit() - Initialize indicators (happens once)
    // 2. Use OnRun() - Subscribe to bar events (not tick events)
    // 3. Use NewHistoryItem - Check signals on bar close
    // 4. Use NewLevel2 - Update VPOC on DOM changes
    // 5. Use VolumeAnalysisData_Loaded - Initialize volume data

    protected override void OnInit()
    {
        // Initialize all indicators
        // Subscribe to Level 2 for VPOC
        if (this.Symbol != null)
        {
            this.Symbol.Subscribe(SubscribeQuoteType.Level2);
        }
    }

    protected override void OnRun()
    {
        // Get symbols
        var symbols = GetTargetCMESymbols();

        foreach (var symbol in symbols)
        {
            // Get 1-minute bar history
            var history = symbol.GetHistory(Period.MIN1, Core.TimeUtils.DateTimeUtcNow);

            // Subscribe to bar close events (NOT tick events)
            history.NewHistoryItem += (s, e) => OnBarClose(symbol, e);

            // Subscribe to Level 2 for DOM/VPOC updates
            symbol.NewLevel2 += (s, l2) => OnLevel2Update(symbol, l2);
        }
    }

    private void OnBarClose(Symbol symbol, HistoryEventArgs e)
    {
        // Bar closed - safe to check all indicators
        // All indicators have updated values

        // 1. Get indicator values
        double vwap = vwapIndicators[symbol].GetVWAP();
        double twap = twapIndicators[symbol].GetTWAP();
        var (vwapUpper, vwapLower) = vwapIndicators[symbol].GetVWAPBands();

        // 2. Analyze bands
        var bandAnalysis = bandAnalyzer.AnalyzeBands(symbol);

        // 3. Check momentum
        var momentumSignal = momentumIndicators[symbol].GetSignal();

        // 4. Generate combined signal
        var signal = signalGenerator.GenerateSignal(
            vwap, twap, bandAnalysis, momentumSignal
        );

        // 5. Execute trade if conditions met
        if (signal.Confidence >= 80 && ValidateRisk())
        {
            ExecuteTrade(symbol, signal);
        }
    }

    private void OnLevel2Update(Symbol symbol, Level2Quote level2)
    {
        // Update VPOC calculation
        // This happens more frequently but only updates volume data
        vpocAnalyzers[symbol].UpdateWithLevel2(level2);
    }

    public void VolumeAnalysisData_Loaded()
    {
        // Volume data finished loading
        // Initialize VPOC values across all symbols
    }
}
```

### Event Decision Tree

```
Strategy Start (OnRun)
    ├─> Subscribe to Bar Events (NewHistoryItem) - PRIMARY SIGNAL CHECK
    ├─> Subscribe to Level2 Events - VPOC UPDATES ONLY
    └─> NO tick events (NewQuote/NewLast) - Too frequent

Bar Event Fires (Every 1 minute)
    ├─> Read all indicator values (already calculated by OnUpdate)
    ├─> Analyze bands
    ├─> Check momentum
    ├─> Generate signal
    ├─> Validate with risk rules
    └─> Execute trade if confirmed

Level2 Event Fires (High frequency)
    └─> Update VPOC calculation only (lightweight)

Volume Data Loaded (Once)
    └─> Initialize historical VPOC values
```

---

## 7. Signal Checking Best Practices

### Prevent Duplicate Signals

```csharp
private Dictionary<Symbol, SignalState> lastSignals = new Dictionary<Symbol, SignalState>();

private void OnBarClose(Symbol symbol, HistoryEventArgs e)
{
    var currentSignal = GenerateSignal(symbol);

    // Check if signal changed
    if (lastSignals.TryGetValue(symbol, out var lastSignal))
    {
        if (currentSignal.Type == lastSignal.Type)
        {
            // Same signal - don't re-trigger
            return;
        }
    }

    // New signal - execute
    lastSignals[symbol] = currentSignal;
    ExecuteTrade(symbol, currentSignal);
}
```

### Signal State Tracking

```csharp
private class SignalState
{
    public SignalType Type { get; set; }
    public DateTime Timestamp { get; set; }
    public double Confidence { get; set; }
    public int BarIndex { get; set; }
}
```

---

## 8. Complete Event Lifecycle Example

```csharp
public class CipherFeedStrategy : Strategy, IVolumeAnalysisIndicator
{
    // Indicators (initialized in OnInit)
    private Dictionary<Symbol, VWAPIndicator> vwapIndicators;
    private Dictionary<Symbol, TWAPIndicator> twapIndicators;
    private Dictionary<Symbol, MomentumIndicator> momentumIndicators;

    // Event tracking
    private Dictionary<Symbol, HistoricalData> barHistories;
    private Dictionary<Symbol, SignalState> signalStates;

    // === LIFECYCLE ===

    protected override void OnInit()
    {
        // 1. Initialize indicator dictionaries
        vwapIndicators = new Dictionary<Symbol, VWAPIndicator>();
        twapIndicators = new Dictionary<Symbol, TWAPIndicator>();
        momentumIndicators = new Dictionary<Symbol, MomentumIndicator>();

        // 2. Subscribe to Level 2 for current symbol
        if (this.Symbol != null)
        {
            this.Symbol.Subscribe(SubscribeQuoteType.Level2);
        }
    }

    protected override void OnRun()
    {
        barHistories = new Dictionary<Symbol, HistoricalData>();
        signalStates = new Dictionary<Symbol, SignalState>();

        var symbols = GetTargetCMESymbols();

        foreach (var symbol in symbols)
        {
            // Create indicators for this symbol
            var vwap = new VWAPIndicator();
            var twap = new TWAPIndicator();
            var momentum = new MomentumIndicator();

            // Get historical data
            var history = symbol.GetHistory(Period.MIN1, Core.TimeUtils.DateTimeUtcNow);

            // Add indicators to history (they auto-calculate)
            history.AddIndicator(vwap);
            history.AddIndicator(twap);
            history.AddIndicator(momentum);

            // Subscribe to bar close event
            history.NewHistoryItem += (s, e) => OnBarClose(symbol, e);

            // Store references
            vwapIndicators[symbol] = vwap;
            twapIndicators[symbol] = twap;
            momentumIndicators[symbol] = momentum;
            barHistories[symbol] = history;

            Log($"Setup complete for {symbol.Name}", StrategyLoggingLevel.Trading);
        }
    }

    protected override void OnStop()
    {
        // Unsubscribe and cleanup
        foreach (var kvp in barHistories)
        {
            kvp.Value.NewHistoryItem -= OnBarClose;
            kvp.Value.Dispose();
        }

        barHistories.Clear();
        signalStates.Clear();
    }

    protected override void OnRemove()
    {
        // Final cleanup
        vwapIndicators?.Clear();
        twapIndicators?.Clear();
        momentumIndicators?.Clear();
    }

    // === EVENT HANDLERS ===

    private void OnBarClose(Symbol symbol, HistoryEventArgs e)
    {
        // Primary signal checking point
        CheckAndExecuteSignals(symbol);
    }

    public void VolumeAnalysisData_Loaded()
    {
        // Volume data ready
        Log("Volume analysis data loaded for all symbols", StrategyLoggingLevel.Trading);
    }

    // === SIGNAL LOGIC ===

    private void CheckAndExecuteSignals(Symbol symbol)
    {
        // Get indicator values (already calculated)
        double vwap = vwapIndicators[symbol].GetVWAP();
        double twap = twapIndicators[symbol].GetTWAP();
        var momentumSignal = momentumIndicators[symbol].GetSignal();

        // Generate combined signal
        var signal = GenerateCombinedSignal(vwap, twap, momentumSignal);

        // Check if signal changed
        if (IsNewSignal(symbol, signal))
        {
            ExecuteTrade(symbol, signal);
            signalStates[symbol] = signal;
        }
    }
}
```

---

## 9. Validation Checklist

### Event Setup
- ✅ Subscribe to events in OnRun()
- ✅ Unsubscribe in OnStop()
- ✅ Dispose HistoricalData objects
- ✅ Use appropriate event for task

### Performance
- ✅ Use bar events for signal generation
- ✅ Avoid heavy processing on ticks
- ✅ Throttle high-frequency events if needed
- ✅ Consider backtest performance

### Multi-Symbol
- ✅ Track events per symbol separately
- ✅ Use dictionaries for symbol-specific data
- ✅ Independent event handlers per symbol
- ✅ Clean up all symbol subscriptions

### Signal Detection
- ✅ Check signals on bar close
- ✅ Prevent duplicate signal triggers
- ✅ Track signal state per symbol
- ✅ Validate before execution

---

## 10. Key Takeaways

1. **Indicators use OnUpdate()** - Called automatically, check UpdateReason for context
2. **Strategies use Symbol events** - Subscribe in OnRun(), unsubscribe in OnStop()
3. **Bar events are optimal** - NewHistoryItem for signal generation (not ticks)
4. **Level 2 for VPOC** - High frequency but lightweight DOM updates
5. **Multi-symbol pattern** - Dictionaries with per-symbol event handlers
6. **Performance matters** - Choose event frequency based on strategy needs
7. **Signal state tracking** - Prevent duplicate triggers, track changes

**Recommended for CipherFeed**:
- Primary: NewHistoryItem (1-min bar close) for signal checking
- Secondary: NewLevel2 for VPOC updates
- Avoid: NewQuote/NewLast (too frequent for multi-indicator strategy)

---

**Next Research Phase**: Band Analysis & Detection Patterns
