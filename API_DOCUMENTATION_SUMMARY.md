# CipherFeed Quantower API Documentation - Summary

**Date:** December 2024  
**Status:** ? Complete

## Overview

I've analyzed all Quantower API usage across the CipherFeed codebase and created comprehensive reference documentation. The main deliverable is `QUANTOWER_API_REFERENCES.md` which provides:

- **Exact document paths** to Quantower API documentation
- **Straightforward instructions** for each API component
- **File-by-file breakdown** of API usage
- **Quick reference guides** for the most critical APIs
- **Code examples** and usage patterns

---

## Key Deliverables

### 1. QUANTOWER_API_REFERENCES.md
**Location:** `C:\Users\tyler\source\repos\CipherFeed\QUANTOWER_API_REFERENCES.md`

**Contents:**
- Complete Quantower API reference with URLs
- Document paths in `quantower-api\` repository structure
- 8 major sections covering all API areas
- File-by-file breakdown of API usage per CipherFeed component
- Quick reference guide for top 10 most-used APIs
- Critical data flow diagram showing tick-to-CSV path

**Structure:**
1. Core Strategy Framework
2. Market Data & Symbols  
3. Historical Data & Indicators
4. Enumerations
5. Account & Connection
6. Metrics System
7. Time Management
8. File-by-File API Usage

---

## Quantower API Document Paths

All paths relative to `quantower-api\` base directory:

### Strategies
```
Strategies\Strategy.md
Strategies\InputParameter.md
Strategies\StrategyLoggingLevel.md
```

### Core
```
Core\Core.md
Core\BusinessObjects\Symbol.md
Core\BusinessObjects\Account.md
Core\Quotes\Last.md
Core\Quotes\Quote.md
Core\History\HistoricalData.md
Core\History\Period.md
Core\Requests\HistoryRequestParameters.md
Core\Enums\AggressorFlag.md
Core\Enums\TickDirection.md
Core\Enums\SymbolType.md
Core\Enums\SeekOriginHistory.md
Core\TimeUtils.md
Core\Connections\Connection.md
```

### Indicators
```
Indicators\Indicator.md
Indicators\InputParameter.md
Indicators\UpdateArgs.md
Indicators\IndicatorMethods.md
```

---

## File-by-File API Mapping

### CipherFeed.cs (Main Strategy)
**APIs Used:**
- `Strategy` base class ? lifecycle management (OnRun, OnStop, OnInitializeMetrics)
- `TradingPlatform.BusinessLayer.Core.Instance` ? symbol discovery, time management
- `Symbol` ? GetHistory(), NewLast/NewQuote events
- `Last` / `Quote` ? real-time market data
- `HistoricalData` / `HistoryRequestParameters` ? indicator initialization
- `InputParameter` ? user configuration
- `StrategyLoggingLevel` ? logging categorization

### Core\SymbolSubscriptionManager.cs
**APIs Used:**
- `Core.Instance.Symbols` ? platform-wide symbol collection
- `Symbol.NewLast` / `Symbol.NewQuote` events ? lambda closure pattern
- `SymbolType.Futures` ? futures contract filtering
- `Account.ConnectionId` ? connection scoping

### Models\MarketDataSnapshot.cs
**APIs Used:**
- `Last` ? Price, Size, AggressorFlag, TickDirection
- `Quote` ? Bid, Ask, BidSize, AskSize, timestamps
- `Symbol` ? Volume, Trades, Delta properties
- `AggressorFlag` / `TickDirection` ? orderflow calculation

### Indicators\ (VWAPIndicator.cs, VPOCIndicator.cs, TWAPIndicator.cs)
**APIs Used:**
- `Indicator` base class ? OnInit(), OnUpdate(), OnClear()
- `UpdateArgs` ? bar update context
- `Symbol.TickSize` ? price level bucketing (VPOC)
- Bar access methods ? High(), Low(), Close(), Volume()

### CSVDataExporter.cs
**APIs Used:**
- `Symbol` ? passed reference (no direct access)
- Indicator public methods ? GetVWAP(), GetVPOC(), GetTWAP(), etc.
- Standard .NET I/O ? System.IO.StreamWriter

### Core\MetricsProvider.cs
**APIs Used:**
- `Strategy.OnInitializeMetrics(Meter)` ? metrics registration point
- `System.Diagnostics.Metrics.Meter` ? .NET metrics API
- `Meter.CreateObservableGauge()` ? pull-based metrics

---

## Critical Data Flow (Real-Time Tick Processing)

```
Platform Tick Arrives
  ?
Symbol.NewLast event fires (Quantower API)
  ?
SymbolSubscriptionManager.lastHandler lambda (per-symbol closure)
  ?
CipherFeed.OnNewLast_ForSymbol(symbolRoot, symbol, last)
  ?
SessionManager.CheckBoundary(last.Time) ? detect session transitions
  ?
MarketDataSnapshot.UpdateFromLast(symbol, last)
  ?? Extract: last.Price, last.Size, last.AggressorFlag, last.TickDirection
  ?? Access: symbol.Volume, symbol.Trades, symbol.Delta
  ?? Calculate: delta, cumulative volumes, imbalances
  ?
[If EnableCSVLogging]
  CSVDataExporter.WriteSnapshot(...)
    ?? Get indicator values (VWAP, VPOC, TWAP)
    ?? StreamWriter.WriteLine(csvRow)
```

---

## Top 10 Most Critical APIs

1. **Strategy.OnRun() / OnStop()** - Lifecycle management
2. **Symbol.NewLast** event - Primary data stream
3. **Symbol.NewQuote** event - Order book updates
4. **Last.Price / Size / AggressorFlag** - Trade details
5. **Symbol.Volume / Trades / Delta** - Aggregates
6. **Indicator.OnUpdate()** - Indicator calculation
7. **Strategy.Log()** - Debugging/monitoring
8. **Core.Instance.TimeUtils.DateTimeUtcNow** - Time management
9. **Symbol.GetHistory()** - Historical data
10. **Meter.CreateObservableGauge()** - Metrics

---

## Build Status

? **Build Successful** - All files compile correctly

### Fixed Issues:
- Resolved namespace collision with `Core.Instance`
- Changed references to `TradingPlatform.BusinessLayer.Core.Instance`
- All 3 instances fixed in `CipherFeed.cs` (lines 197, 207, 290)

---

## Usage Guidelines

### For Development:
1. **Open** `QUANTOWER_API_REFERENCES.md` alongside your code
2. **Search** for the API component you're using
3. **Follow** the exact document path to Quantower docs
4. **Reference** code examples in the "Usage in CipherFeed" sections

### For Onboarding:
1. **Start** with Section 1 (Core Strategy Framework)
2. **Progress** through Sections 2-7 sequentially
3. **Use** Section 8 (File-by-File) as a practical reference
4. **Refer** to Quick Reference Guide for common patterns

### For Debugging:
1. **Check** Critical Data Flow diagram for tick processing path
2. **Verify** API usage against documented patterns
3. **Compare** with file-specific API usage examples
4. **Review** Quantower docs via provided URLs

---

## Document Maintenance

### When Adding New Files:
1. Document Quantower APIs used
2. Add to Section 8 (File-by-File API Usage)
3. Update Critical Data Flow if applicable
4. Test build after changes

### When Using New APIs:
1. Add to appropriate section (1-7)
2. Include document path and URL
3. Provide usage example from CipherFeed context
4. Update Quick Reference if critical

---

## Key Architectural Patterns

### 1. Lambda Closure Pattern (SymbolSubscriptionManager)
```csharp
// Each symbol gets dedicated handler with captured symbolRoot
symbol.NewLast += (s, last) => OnNewLast_ForSymbol(symbolRoot, s, last);
```
**Benefit:** Zero dictionary lookups, true parallel processing

### 2. Percentage Space Calculations (Indicators)
```csharp
PricePct = (Price - SessionOpen) / SessionOpen
VWAPPct = ?(PricePct × Volume) / ?(Volume)
```
**Benefit:** Normalized across instruments, ML-ready

### 3. Delegate-Based Metrics (MetricsProvider)
```csharp
Func<string, (Symbol, bool)> getSymbol = (symbolRoot) => { /* lambda */ };
meter.CreateObservableGauge("metric_name", () => getSymbol("MNQ").price);
```
**Benefit:** Late binding, pull-based metrics

### 4. Session-Anchored State (CipherFeed)
```csharp
OnSessionChanged() {
    sessionOpenPrices.Clear();
    latestSnapshots.Clear();
    // Re-initialize indicators from new session start
}
```
**Benefit:** Clean session boundaries, accurate calculations

---

## Additional Resources

### Quantower Official Documentation:
- **Base URL:** https://api.quantower.com/docs/
- **Namespace:** TradingPlatform.BusinessLayer

### .NET Metrics Documentation:
- **Meter Class:** https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.metrics.meter
- **ObservableGauge:** https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.metrics.meter.createobservablegauge

### CipherFeed Project Files:
- **Main Strategy:** `CipherFeed.cs`
- **Indicators:** `Indicators\*.cs`
- **Core Managers:** `Core\*.cs`
- **Models:** `Models\MarketDataSnapshot.cs`
- **CSV Export:** `CSVDataExporter.cs`

---

## Next Steps

1. ? **Documentation Complete** - `QUANTOWER_API_REFERENCES.md` created
2. ? **Build Verified** - All files compile successfully
3. ? **API References Mapped** - All Quantower APIs documented
4. **Ready for Development** - Use docs for continued development
5. **Consider:** Adding inline code comments referencing specific API sections

---

**End of Summary**
