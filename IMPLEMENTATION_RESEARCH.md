# Quantower API Implementation Research
## VWAP, TWAP, VPOC Calculations with Bands and Multi-Symbol Support

**Date**: 2025-10-03
**Purpose**: Document validated API constructs for implementing indicators with historical initialization and multi-symbol support

---

## 1. VWAP (Volume-Weighted Average Price) Calculation

### Formula
```
VWAP = Σ(Typical Price × Volume) / Σ(Volume)
where Typical Price = (High + Low + Close) / 3
```

### Quantower API Constructs

**PriceType Enum Values:**
- `PriceType.High` - Bar high
- `PriceType.Low` - Bar low
- `PriceType.Close` - Bar close
- `PriceType.Typical` - (High + Low + Close) / 3 (built-in!)
- `PriceType.Volume` - Bar volume

**Indicator Base Class Methods:**
```csharp
double GetPrice(PriceType priceType, int offset)  // offset: 0=current, 1=previous
double Volume(int offset)                          // Get volume at offset
void SetValue(double value)                        // Set indicator value
double GetValue(int offset)                        // Get indicator value
int Count                                          // Number of bars loaded
```

### VWAP Implementation Pattern

```csharp
public class VWAPIndicator : Indicator
{
    private double cumulativeTPV;  // Typical Price × Volume
    private double cumulativeVolume;

    protected override void OnUpdate(UpdateArgs args)
    {
        // Use built-in Typical price type!
        double typicalPrice = GetPrice(PriceType.Typical, 0);
        double volume = Volume(0);

        cumulativeTPV += typicalPrice * volume;
        cumulativeVolume += volume;

        double vwap = cumulativeVolume > 0 ? cumulativeTPV / cumulativeVolume : 0;
        SetValue(vwap);
    }
}
```

### VWAP Bands Calculation

**Standard Deviation Formula:**
```
StdDev = √(Σ(Price - VWAP)² / Count)
Upper Band = VWAP + (StdDev × multiplier)
Lower Band = VWAP - (StdDev × multiplier)
```

**Implementation:**
```csharp
public (double upper, double lower) GetVWAPBands(double stdDevMultiplier = 2.0)
{
    double vwap = GetValue();
    double sumSquaredDiff = 0;

    // Calculate variance from VWAP
    for (int i = 0; i < Count; i++)
    {
        double typicalPrice = GetPrice(PriceType.Typical, i);
        double diff = typicalPrice - vwap;
        sumSquaredDiff += diff * diff;
    }

    double stdDev = Math.Sqrt(sumSquaredDiff / Count);
    double upperBand = vwap + (stdDev * stdDevMultiplier);
    double lowerBand = vwap - (stdDev * stdDevMultiplier);

    return (upperBand, lowerBand);
}
```

---

## 2. TWAP (Time-Weighted Average Price) Calculation

### Formula
```
TWAP = Σ(Price) / Count
(Simple average - each price weighted equally by time, volume ignored)
```

### Implementation Pattern

```csharp
public class TWAPIndicator : Indicator
{
    private double cumulativePrice;
    private int priceCount;

    [InputParameter("Price Type", 0)]
    public PriceType PriceType { get; set; } = PriceType.Close;

    protected override void OnUpdate(UpdateArgs args)
    {
        double price = GetPrice(PriceType, 0);

        cumulativePrice += price;
        priceCount++;

        double twap = priceCount > 0 ? cumulativePrice / priceCount : 0;
        SetValue(twap);
    }
}
```

### TWAP Bands Calculation

Same standard deviation formula as VWAP, but calculated against TWAP instead:

```csharp
public (double upper, double lower) GetTWAPBands(double stdDevMultiplier = 2.0)
{
    double twap = GetValue();
    double sumSquaredDiff = 0;

    for (int i = 0; i < Count; i++)
    {
        double price = GetPrice(PriceType, i);
        double diff = price - twap;
        sumSquaredDiff += diff * diff;
    }

    double stdDev = Math.Sqrt(sumSquaredDiff / Count);
    return (twap + stdDev * stdDevMultiplier, twap - stdDev * stdDevMultiplier);
}
```

---

## 3. VPOC (Volume Point of Control) Calculation

### Definition
VPOC = Price level with the highest traded volume

### Quantower API Constructs

**VolumeAnalysisManager (Instantiable Class):**
```csharp
var volumeManager = new VolumeAnalysisManager();
IVolumeAnalysisCalculationTask task = volumeManager.CalculateProfile(symbol, startTime, endTime);
```

**VolumeAnalysisData Structure:**
```csharp
Dictionary<double, VolumeAnalysisItem> PriceLevels  // Key=price, Value=volume data
VolumeAnalysisItem Total                             // Summary volume info
```

**VolumeAnalysisItem Properties:**
- `Volume` - Total volume at price level
- `BuyVolume` - Buy volume
- `SellVolume` - Sell volume
- `Delta` - BuyVolume - SellVolume
- `Trades` - Number of trades
- `AverageSize` - Average trade size

**VolumeAnalysisField Enum:**
- Volume, BuyVolume, SellVolume
- Delta, CumulativeDelta, DeltaPercent
- Trades, BuyTrades, SellTrades
- AverageSize, AverageBuySize, AverageSellSize
- MaxOneTradeVolume

### VPOC Implementation

```csharp
public class VPOCAnalyzer
{
    private VolumeAnalysisManager volumeManager = new VolumeAnalysisManager();
    private IVolumeAnalysisCalculationTask currentTask;

    public IVolumeAnalysisCalculationTask CalculateProfile(Symbol symbol, DateTime start, DateTime end)
    {
        currentTask = volumeManager.CalculateProfile(symbol, start, end);
        return currentTask;
    }

    public double GetVPOC()
    {
        if (currentTask?.Progress?.State != VolumeAnalysisCalculationState.Finished)
            return 0;

        VolumeAnalysisData data = currentTask.Result;
        if (data?.PriceLevels == null || data.PriceLevels.Count == 0)
            return 0;

        // Find price with maximum volume
        var vpocLevel = data.PriceLevels
            .OrderByDescending(kvp => kvp.Value.Volume)
            .FirstOrDefault();

        return vpocLevel.Key;
    }
}
```

### Value Area Calculation (70% Volume Range)

```csharp
public (double high, double low, double vpoc) GetValueArea()
{
    if (currentTask?.Progress?.State != VolumeAnalysisCalculationState.Finished)
        return (0, 0, 0);

    VolumeAnalysisData data = currentTask.Result;
    double totalVolume = data.Total?.Volume ?? 0;
    if (totalVolume == 0) return (0, 0, 0);

    double vpoc = GetVPOC();

    // Sort by volume descending
    var sortedLevels = data.PriceLevels
        .OrderByDescending(kvp => kvp.Value.Volume)
        .ToList();

    // Accumulate 70% of total volume
    double targetVolume = totalVolume * 0.70;
    double accumulated = 0;
    double valueAreaHigh = double.MinValue;
    double valueAreaLow = double.MaxValue;

    foreach (var level in sortedLevels)
    {
        accumulated += level.Value.Volume;

        if (level.Key > valueAreaHigh) valueAreaHigh = level.Key;
        if (level.Key < valueAreaLow) valueAreaLow = level.Key;

        if (accumulated >= targetVolume) break;
    }

    return (valueAreaHigh, valueAreaLow, vpoc);
}
```

---

## 4. Historical Data Initialization

### Problem
Indicators must start with current market values, not zero. Need to process all historical bars on initialization.

### Solution: OnUpdate with UpdateReason

**UpdateReason Enum:**
- `UpdateReason.HistoricalBar` - Processing historical data
- `UpdateReason.NewTick` - Real-time tick update
- `UpdateReason.NewBar` - New bar started

**Pattern for Historical Initialization:**

```csharp
protected override void OnUpdate(UpdateArgs args)
{
    // Process ALL bars including historical
    // Cumulative calculations naturally include history

    double price = GetPrice(PriceType, 0);
    cumulativePrice += price;
    priceCount++;

    double average = cumulativePrice / priceCount;
    SetValue(average);

    // Optional: Different logic for historical vs real-time
    if (args.Reason == UpdateReason.HistoricalBar)
    {
        // Historical processing
    }
    else if (args.Reason == UpdateReason.NewTick)
    {
        // Real-time processing
    }
}
```

### Alternative: VolumeAnalysisData_Loaded Callback

For volume-dependent indicators, implement `IVolumeAnalysisIndicator`:

```csharp
public class MyIndicator : Indicator, IVolumeAnalysisIndicator
{
    public void VolumeAnalysisData_Loaded()
    {
        // Called when all volume data finishes loading

        // Initialize all historical values at once
        for (int i = 0; i < Count; i++)
        {
            // Calculate for each historical bar
            double value = CalculateValue(i);
            SetValue(value, 0, i);  // lineIndex=0, offset=i
        }
    }

    protected override void OnUpdate(UpdateArgs args)
    {
        // Only handle real-time updates
        if (args.Reason != UpdateReason.HistoricalBar)
        {
            // Real-time calculation
        }
    }
}
```

---

## 5. Multi-Symbol Support

### Accessing Multiple Symbols

**Core.Instance.Symbols:**
```csharp
// Get all available symbols
Symbol[] allSymbols = Core.Instance.Symbols;

// Filter specific symbols
var cmeSymbols = allSymbols.Where(s =>
    s.Exchange?.ExchangeName?.Contains("CME") == true);
```

### Pattern for Multi-Symbol Strategy

```csharp
public class MultiSymbolStrategy : Strategy
{
    // Dictionary to store per-symbol indicators
    private Dictionary<Symbol, VWAPIndicator> vwapIndicators;
    private Dictionary<Symbol, TWAPIndicator> twapIndicators;
    private Dictionary<Symbol, VPOCAnalyzer> vpocAnalyzers;

    protected override void OnInit()
    {
        vwapIndicators = new Dictionary<Symbol, VWAPIndicator>();
        twapIndicators = new Dictionary<Symbol, TWAPIndicator>();
        vpocAnalyzers = new Dictionary<Symbol, VPOCAnalyzer>();

        // Get target symbols
        var symbols = GetTargetSymbols();

        foreach (var symbol in symbols)
        {
            // Create VWAP for this symbol
            var vwap = new VWAPIndicator();

            // Request historical data for symbol
            var history = symbol.GetHistory(
                Period.MIN1,  // 1-minute bars
                DateTime.Now.AddDays(-1),
                DateTime.Now
            );

            // Add indicator to history
            history.AddIndicator(vwap);

            // Store in dictionary
            vwapIndicators[symbol] = vwap;

            // Subscribe to Level 2 for volume profile
            symbol.Subscribe(SubscribeQuoteType.Level2);

            // Create VPOC analyzer for this symbol
            var vpoc = new VPOCAnalyzer();
            vpoc.CalculateProfile(symbol, DateTime.Now.AddHours(-4), DateTime.Now);
            vpocAnalyzers[symbol] = vpoc;
        }
    }

    protected override void OnRun()
    {
        // Access indicator values per symbol
        foreach (var kvp in vwapIndicators)
        {
            Symbol symbol = kvp.Key;
            VWAPIndicator vwap = kvp.Value;

            double currentVWAP = vwap.GetVWAP();
            var (upperBand, lowerBand) = vwap.GetVWAPBands(2.0);

            Log($"{symbol.Name} - VWAP: {currentVWAP}, Bands: [{lowerBand}, {upperBand}]");
        }
    }

    private Symbol[] GetTargetSymbols()
    {
        var targetNames = new[] { "ES", "NQ", "RTY", "YM", "MES", "MNQ", "M2K", "MYM" };
        var allSymbols = Core.Instance.Symbols;

        return allSymbols.Where(s =>
            targetNames.Any(name =>
                s.Name.StartsWith(name, StringComparison.OrdinalIgnoreCase)) &&
            s.Exchange?.ExchangeName?.Contains("CME") == true
        ).ToArray();
    }
}
```

### Getting Historical Data for Symbols

**Symbol.GetHistory() Overloads:**
```csharp
// Method 1: Simple period-based
HistoricalData history = symbol.GetHistory(
    Period.MIN1,           // 1-minute period
    DateTime.Now.AddDays(-7),
    DateTime.Now
);

// Method 2: Advanced with parameters
var parameters = new HistoryRequestParameters
{
    Symbol = symbol,
    Period = Period.MIN5,
    HistoryType = HistoryType.Last,
    FromTime = DateTime.Now.AddDays(-30),
    ToTime = DateTime.Now
};
HistoricalData history = symbol.GetHistory(parameters);

// Method 3: Aggregation-based
var aggregation = new HistoryAggregationTime(Period.DAY1);
HistoricalData history = symbol.GetHistory(
    aggregation,
    DateTime.Now.AddMonths(-6),
    DateTime.Now
);
```

### HistoricalData Access

**Bar Properties:**
```csharp
foreach (var bar in historicalData)
{
    HistoryItemBar barData = (HistoryItemBar)bar;

    double open = barData.Open;
    double high = barData.High;
    double low = barData.Low;
    double close = barData.Close;
    double volume = barData.Volume;
    double typical = barData.Typical;  // (H+L+C)/3
    double median = barData.Median;    // (H+L)/2
    double weighted = barData.Weighted; // (H+L+C+C)/4

    DateTime time = barData.TimeRight;
    int ticks = barData.Ticks;
}

// Index access
HistoryItemBar currentBar = (HistoryItemBar)historicalData[0, SeekOriginHistory.End];
HistoryItemBar previousBar = (HistoryItemBar)historicalData[1, SeekOriginHistory.End];
```

---

## 6. Complete Indicator Validation Checklist

### VWAP Indicator Requirements
- ✅ Use `PriceType.Typical` for (H+L+C)/3
- ✅ Access volume via `Volume(offset)`
- ✅ Maintain cumulative TPV and cumulative volume
- ✅ Calculate bands using standard deviation from VWAP
- ✅ Process all historical bars in OnUpdate
- ✅ Reset cumulative values in OnClear()

### TWAP Indicator Requirements
- ✅ Use `GetPrice(priceType, offset)` with configurable price type
- ✅ Maintain cumulative price and count
- ✅ Calculate simple average (volume-agnostic)
- ✅ Calculate bands using standard deviation from TWAP
- ✅ Process all historical bars in OnUpdate
- ✅ Reset cumulative values in OnClear()

### VPOC Analyzer Requirements
- ✅ Instantiate `VolumeAnalysisManager` directly
- ✅ Call `CalculateProfile(symbol, start, end)` - returns `IVolumeAnalysisCalculationTask`
- ✅ Check `task.Progress.State == VolumeAnalysisCalculationState.Finished`
- ✅ Access `task.Result.PriceLevels` dictionary
- ✅ Find max volume price level for VPOC
- ✅ Calculate 70% volume range for value area
- ✅ Subscribe to `SubscribeQuoteType.Level2` for DOM data

### Multi-Symbol Strategy Requirements
- ✅ Access symbols via `Core.Instance.Symbols`
- ✅ Use `Dictionary<Symbol, Indicator>` to store per-symbol indicators
- ✅ Request historical data: `symbol.GetHistory(period, from, to)`
- ✅ Add indicator to history: `history.AddIndicator(indicator)`
- ✅ Subscribe each symbol: `symbol.Subscribe(SubscribeQuoteType.Level2)`
- ✅ Create separate VPOC analyzer per symbol
- ✅ Access indicator values via dictionary lookup

---

## 7. Key API Patterns Summary

### Platform-Provided Objects (Cannot Instantiate)
- `Symbol` - Get from `Core.Instance.Symbols`
- `Account` - Get from `Core.Instance.Accounts`
- `HistoricalData` - Get from `Symbol.GetHistory()`
- `Order`, `Position` - Created by platform

### Instantiable Classes
- `VolumeAnalysisManager` - Create with `new`
- `VolumeAnalysisData` - Create with `new`
- `HistoryRequestParameters` - Create with `new`
- Request parameter classes - Create with `new`

### Interfaces to Implement
- `IVolumeAnalysisIndicator` - For volume data callbacks
  - Implement `VolumeAnalysisData_Loaded()` method

### Interfaces Received as Return Types
- `IVolumeAnalysisCalculationTask` - From `CalculateProfile()`
- `IVolumeAnalysisCalculationProgress` - From `task.Progress`

---

## 8. Implementation Priority

1. **VWAP Indicator** - Use Typical price type, cumulative calculation
2. **TWAP Indicator** - Use configurable price type, cumulative calculation
3. **VPOC Analyzer** - Use VolumeAnalysisManager, async pattern
4. **Band Calculations** - Standard deviation from average
5. **Multi-Symbol Support** - Dictionary pattern with per-symbol instances
6. **Historical Initialization** - OnUpdate processes all bars naturally

---

**Validated Against:**
- TradingPlatform.BusinessLayer API Documentation
- Quantower Help Documentation
- Real implementation examples

**Next Steps:**
1. Implement indicators following these exact patterns
2. Test with CME futures symbols
3. Validate calculations against Quantower built-in indicators
4. Implement signal generation using indicator values
