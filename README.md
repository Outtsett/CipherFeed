# CipherFeed

Real-time orderflow analytics for CME micro futures with session-anchored indicators and ML-ready feature engineering.

## ?? Development Guidelines

### **CRITICAL: No .md Summary Files**
- ? **DO NOT** create separate `.md` summary or documentation files for code changes
- ? **DO** update file header comments directly in the source code
- ? **DO** keep notes concise, technical, and to-the-point in header blocks
- Purpose: Avoid documentation sprawl and keep information at the source

### **Using Public Class Files Properly**
To avoid duplicated logic across files:

1. **Check for existing implementations FIRST**
   - Use `code_search` to find existing logic before creating new methods
   - Review `Models\`, `Core\`, and `Indicators\` folders for reusable components

2. **Proper namespace imports**
   ```csharp
   using CipherFeed.Core;        // SessionManager, SymbolLogger, etc.
   using CipherFeed.Models;      // MarketDataSnapshot
   using CipherFeed.Indicators;  // SessionAnchoredVWAP, VPOCIndicator, TWAPIndicator
   ```

3. **Reference existing classes instead of duplicating**
   - ? DON'T: Copy-paste logic from another file
   - ? DO: Import the namespace and use the existing class
   - ? DO: Extend functionality in the original file if modifications are needed

4. **Singleton pattern for managers**
   - `SessionManager`, `SymbolSubscriptionManager`, `MetricsProvider` are created once in `OnCreated()`
   - Pass references via constructors or method parameters, not via static globals

### **File Header Comments**
When updating a file, modify the header block to include:
```csharp
/*
 * FILE: FileName.cs
 * PURPOSE: One-line description
 * KEY DEPENDENCIES: List important imports
 * LAST MODIFIED: Brief change description (date optional)
 */
```

Keep it **technical** and **concise** - no storytelling.

---

## ??? Project Structure

```
CipherFeed/
?
??? CipherFeed.cs                    # Main strategy orchestrator
??? CSVDataExporter.cs               # Real-time CSV export (67 columns)
?
??? Core/                            # Core managers and utilities
?   ??? SessionManager.cs            # RTH/ETH session detection & boundaries
?   ??? SymbolSubscriptionManager.cs # Parallel per-symbol event handlers
?   ??? SymbolLogger.cs              # Formatted console logging
?   ??? MetricsProvider.cs           # Prometheus/OpenTelemetry metrics
?
??? Models/                          # Data models
?   ??? MarketDataSnapshot.cs        # 67 orderflow features per tick
?
??? Indicators/                      # Custom indicators
?   ??? VWAPIndicator.cs             # Session-anchored VWAP with ±2? and MPD bands
?   ??? VPOCIndicator.cs             # Volume Profile (VPOC, VAH, VAL)
?   ??? TWAPIndicator.cs             # Time-weighted average price with ±2? bands
?
??? README.md                        # This file
```

### **File Responsibilities**

#### **CipherFeed.cs (Main Strategy)**
- Strategy lifecycle (`OnRun`, `OnStop`, `OnInitializeMetrics`)
- Symbol subscription orchestration
- Event routing to managers
- High-level session change coordination
- **DOES NOT contain**: Session detection logic, snapshot creation, indicator math

#### **Core\SessionManager.cs**
- `TradingSession` enum (RTH, ETH)
- Session boundary detection (`CheckBoundary`)
- Session time calculations (`GetSession`, `GetSessionStart`)
- UTC time constants (RTH_START, RTH_END, ETH_START, ETH_END)
- `SessionChanged` event firing

#### **Models\MarketDataSnapshot.cs**
- 67 orderflow features with cumulative state management
- `UpdateFromLast()` - Process trade ticks
- `UpdateFromQuote()` - Process bid/ask updates
- `ResetCumulativeState()` - Called on session change

#### **Core\SymbolSubscriptionManager.cs**
- Lambda closure pattern for per-symbol event handlers
- Zero-dictionary-lookup tick processing
- Thread-safe parallel execution
- Symbol discovery and front-month contract selection

#### **Indicators\***
- Custom indicator logic (VWAP, VPOC, TWAP)
- Percentage-space calculations for ML normalization
- Session-anchored indicator state management

---

## ?? Quick Start

### Prerequisites
- .NET 8 SDK
- Quantower platform with active CME data connection
- C# 14.0 compatible IDE

### Building
```bash
dotnet build CipherFeed.csproj
```

### Configuration
Edit `CipherFeed.cs` strategy parameters:
- **Account**: Select trading account
- **Log Interval**: Console logging frequency (default: 5 seconds)
- **CSV Logging**: Enable/disable real-time CSV export
- **CSV Directory**: Output path (default: `C:\CipherFeed\Logs`)

---

## ?? Features

### Real-Time Market Data
- **8 CME Micro Futures**: MNQ, MES, M2K, MYM, ENQ, EP, RTY, YM
- **Parallel Processing**: Dedicated event handlers per symbol (8,000 ticks/sec capacity)
- **Sub-millisecond Latency**: Lambda closures with zero dictionary lookups

### Session Management
- **Automatic RTH/ETH Detection**: PST to UTC conversion
- **Boundary Handling**: Seamless transitions at 12:00 UTC and 23:15 UTC
- **State Reset**: Cumulative volumes, deltas, and indicators reset per session

### Indicators (Percentage Space)
- **Session-Anchored VWAP**: ±2? bands + MPD (Mean Price Deviation) bands
- **Volume Profile (VPOC)**: VAH, VAL, 70% value area
- **TWAP**: Time-weighted average with ±2? bands
- **Normalized Values**: `(Price - SessionOpen) / SessionOpen` for ML compatibility

### Orderflow Analytics (67 Features)
- **Volume**: Total, Buy, Sell, Filtered, percentages
- **Delta**: Tick-by-tick, cumulative, imbalance
- **Liquidity**: Bid/ask sizes, changes, trade counts
- **Aggressor Flags**: Buy-side vs sell-side initiated trades
- **Tick Direction**: Up/Down/Undefined price movement

### CSV Export
- **Real-Time Logging**: Every tick written immediately
- **Per-Symbol Files**: `{Symbol}_{Session}_{Timestamp}.csv`
- **67 Columns**: Full orderflow + indicator values
- **Thread-Safe**: Per-file locks prevent data corruption

---

## ?? Architecture Patterns

### 1. Lambda Closure Pattern (Zero-Lookup Tick Processing)
```csharp
// Each symbol gets dedicated handler with captured symbolRoot
symbol.NewLast += (s, last) => OnNewLast_ForSymbol(symbolRoot, s, last);
```
**Benefit**: No dictionary lookups ? <1ms latency per tick

### 2. Session-Anchored State Management
```csharp
SessionManager.SessionChanged += (old, new, start) => {
    // Reset all cumulative state
    snapshots.Clear();
    indicators.RemoveAll();
    indicators.InitializeFrom(newSessionStart);
};
```
**Benefit**: Clean session boundaries, accurate calculations

### 3. Percentage-Space Normalization
```csharp
double pricePct = (price - sessionOpen) / sessionOpen;
double vwap = ?(pricePct × volume) / ?(volume);
```
**Benefit**: ML-ready features, cross-instrument comparison

---

## ??? Development Workflow

1. **Make Changes**: Edit source files directly
2. **Update Headers**: Modify file header comments (no separate docs)
3. **Build**: `dotnet build`
4. **Test**: Run strategy in Quantower with replay/sim data
5. **Commit**: Push changes with descriptive commit message

### Adding New Indicators
1. Create `Indicators\NewIndicator.cs` inheriting from `Indicator`
2. Implement `OnInit()`, `OnUpdate()`, `OnClear()`
3. Add to `CipherFeed.cs::InitializeIndicatorsForSymbol()`
4. Update `CSVDataExporter.cs` header and row builder

### Adding New Features to Snapshot
1. Add property to `Models\MarketDataSnapshot.cs`
2. Update `UpdateFromLast()` or `UpdateFromQuote()` logic
3. Increment feature count in header comment
4. Add column to `CSVDataExporter.cs::GetCSVHeader()`
5. Add value to `CSVDataExporter.cs::BuildCSVRow()`

---

## ?? Additional Documentation

- **API References**: `QUANTOWER_API_REFERENCES.md` - Complete Quantower API mapping
- **API Summary**: `API_DOCUMENTATION_SUMMARY.md` - Quick reference and patterns

---

## ?? Important Notes

- **UTC Only**: All timestamps and session logic use UTC (avoid PST/timezone conversions)
- **Session Anchored**: All indicators and cumulative state reset at session boundaries
- **Thread Safety**: CSV writes use per-file locks; dictionaries accessed on single thread
- **Memory Management**: Dictionaries cleared on session change; indicators properly disposed

---

## ?? License

[Add your license here]

---

**Last Updated**: December 2024