# Quantower API References for CipherFeed Strategy

**Last Updated:** December 2024  
**Purpose:** Comprehensive mapping of all Quantower API usage across the CipherFeed codebase with exact document paths for development reference.

---

## Table of Contents

1. [Core Strategy Framework](#1-core-strategy-framework)
2. [Market Data & Symbols](#2-market-data--symbols)
3. [Historical Data & Indicators](#3-historical-data--indicators)
4. [Enumerations](#4-enumerations)
5. [Account & Connection](#5-account--connection)
6. [Metrics System](#6-metrics-system)
7. [Time Management](#7-time-management)
8. [File-by-File API Usage](#8-file-by-file-api-usage)

---

## 1. Core Strategy Framework

### Strategy Base Class
- **Document:** `quantower-api\Strategies\Strategy.md`
- **URL:** https://api.quantower.com/docs/TradingPlatform.BusinessLayer.Strategy.html
- **Namespace:** `TradingPlatform.BusinessLayer`

#### Key Methods:
- `OnCreated()` - Called once when strategy is instantiated (initialization)
- `OnRun()` - Called when strategy starts (subscribe to data, setup state)
- `OnStop()` - Called when strategy stops (cleanup, unsubscribe)
- `OnInitializeMetrics(Meter meter)` - Register custom metrics for Metrics panel
- `Log(string message, StrategyLoggingLevel level)` - Write to Strategy Log panel

#### Key Properties:
- `Name` - Strategy display name
- `CurrentAccount` - Selected trading account (set via [InputParameter])

#### Usage in CipherFeed:
- **File:** `CipherFeed.cs`
- **Class:** `CipherFeed : Strategy`
- **Lifecycle:**
  - `OnCreated()` ? Create managers (one-time)
  - `OnRun()` ? Subscribe to symbols, initialize indicators
  - `OnStop()` ? Unsubscribe, cleanup
  - `OnInitializeMetrics()` ? Delegate to MetricsProvider

---

### InputParameter Attribute
- **Document:** `quantower-api\Strategies\InputParameter.md`
- **URL:** https://api.quantower.com/docs/TradingPlatform.BusinessLayer.InputParameter.html
- **Namespace:** `TradingPlatform.BusinessLayer`

#### Usage Pattern:
```csharp
[InputParameter("Display Name", order)]
public Type PropertyName { get; set; } = defaultValue;
```

#### Usage in CipherFeed:
- **File:** `CipherFeed.cs`
- **Parameters:**
  ```csharp
  [InputParameter("Account", 1)]
  public Account CurrentAccount { get; set; }
  
  [InputParameter("Enable Real-Time Symbol Logging", 2)]
  public bool EnableRealtimeLogging { get; set; } = false;
  
  [InputParameter("Log Orderflow Features", 3)]
  public bool LogOrderflowFeatures { get; set; } = false;
  
  [InputParameter("Enable CSV Logging", 4)]
  public bool EnableCSVLogging { get; set; } = true;
  
  [InputParameter("CSV Log Directory", 5)]
  public string CSVLogDirectory { get; set; } = @"C:\CipherFeed\Logs";
  ```

---

### StrategyLoggingLevel
- **Document:** `quantower-api\Strategies\StrategyLoggingLevel.md`
- **URL:** https://api.quantower.com/docs/TradingPlatform.BusinessLayer.StrategyLoggingLevel.html
- **Namespace:** `TradingPlatform.BusinessLayer`

#### Values:
- `Trading` - Normal operational messages
- `Error` - Error messages (red in log)
- `Warning` - Warning messages (yellow in log)
- `Info` - Informational messages
- `Debug` - Debug messages

#### Usage in CipherFeed:
- **Files:** `CipherFeed.cs`, `Core\SymbolLogger.cs`
- **Pattern:** `Log(message, StrategyLoggingLevel.Trading)`

---

## 2. Market Data & Symbols

### Symbol Class
- **Document:** `quantower-api\Core\BusinessObjects\Symbol.md`
- **URL:** https://api.quantower.com/docs/TradingPlatform.BusinessLayer.Symbol.html
- **Namespace:** `TradingPlatform.BusinessLayer`

#### Key Properties:
- `Name` (string) - Full symbol name (e.g., "MNQH24")
- `SymbolType` (SymbolType) - Futures, Forex, Stocks, etc.
- `ConnectionId` (string) - Data connection identifier
- `ExpirationDate` (DateTime) - Contract expiration date
- `TickSize` (double) - Minimum price movement
- `Volume` (double) - Session total volume
- `Trades` (long) - Session total trade count
- `Delta` (double) - Session cumulative orderflow delta
- `HistoryType` (HistoryType) - For historical data requests

#### Key Events:
- `NewLast` - Fires on every trade execution (time & sales)
- `NewQuote` - Fires on bid/ask update (order book)

#### Key Methods:
- `GetHistory(HistoryRequestParameters params)` - Retrieve historical bars
- `Subscribe()` / `Unsubscribe()` - Implicit via event subscription

#### Usage in CipherFeed:
- **Primary File:** `Core\SymbolSubscriptionManager.cs`
  - Subscribe via: `symbol.NewLast += lastHandler`
  - Subscribe via: `symbol.NewQuote += quoteHandler`
- **Used in:** `CipherFeed.cs`, `Models\MarketDataSnapshot.cs`, `Indicators\VPOCIndicator.cs`

---

### Last (Trade Tick)
- **Document:** `quantower-api\Core\Quotes\Last.md`
- **URL:** https://api.quantower.com/docs/TradingPlatform.BusinessLayer.Last.html
- **Namespace:** `TradingPlatform.BusinessLayer`

#### Properties:
- `Price` (double) - Trade execution price
- `Size` (double) - Trade size/quantity
- `Time` (DateTime) - Trade timestamp (UTC)
- `AggressorFlag` (AggressorFlag) - Buy/Sell/None
- `TickDirection` (TickDirection) - Up/Down/Undefined

#### Usage in CipherFeed:
- **Primary File:** `Models\MarketDataSnapshot.cs::UpdateFromLast()`
- **Event Handler:** `CipherFeed.cs::OnNewLast_ForSymbol()`
- **Features Extracted:**
  - Price movement (Last, Size)
  - Orderflow direction (AggressorFlag)
  - Delta calculation (Buy/Sell volume)
  - Cumulative tracking

---

### Quote (Bid/Ask)
- **Document:** `quantower-api\Core\Quotes\Quote.md`
- **URL:** https://api.quantower.com/docs/TradingPlatform.BusinessLayer.Quote.html
- **Namespace:** `TradingPlatform.BusinessLayer`

#### Properties:
- `Bid` (double) - Best bid price
- `BidSize` (double) - Best bid size
- `Ask` (double) - Best ask price
- `AskSize` (double) - Best ask size
- `Time` (DateTime) - Quote timestamp (UTC)
- `BidTickDirection` (TickDirection) - Bid movement direction
- `AskTickDirection` (TickDirection) - Ask movement direction

#### Usage in CipherFeed:
- **Primary File:** `Models\MarketDataSnapshot.cs::UpdateFromQuote()`
- **Event Handler:** `CipherFeed.cs::OnNewQuote_ForSymbol()`
- **Features Extracted:**
  - Spread (Ask - Bid)
  - Liquidity (BidSize, AskSize)
  - Cumulative bid/ask sizes

---

## 3. Historical Data & Indicators

### HistoricalData Class
- **Document:** `quantower-api\Core\History\HistoricalData.md`
- **URL:** https://api.quantower.com/docs/TradingPlatform.BusinessLayer.HistoricalData.html
- **Namespace:** `TradingPlatform.BusinessLayer`

#### Key Methods:
- `AddIndicator(Indicator indicator)` - Attach indicator to historical data
- `RemoveIndicator(Indicator indicator)` - Detach indicator
- `Count` (int) - Number of bars
- `[int index, SeekOriginHistory origin]` - Indexer to access bars

#### Usage in CipherFeed:
- **File:** `CipherFeed.cs`
- **Method:** `GetHistoricalData()` - Retrieve bars for indicator initialization
- **Pattern:**
  ```csharp
  HistoricalData history = symbol.GetHistory(params);
  history.AddIndicator(vwapIndicator);
  history.AddIndicator(vpocIndicator);
  history.AddIndicator(twapIndicator);
  ```

---

### HistoryRequestParameters
- **Document:** `quantower-api\Core\Requests\HistoryRequestParameters.md`
- **URL:** https://api.quantower.com/docs/TradingPlatform.BusinessLayer.HistoryRequestParameters.html
- **Namespace:** `TradingPlatform.BusinessLayer`

#### Properties:
- `Symbol` (Symbol) - Symbol to retrieve history for
- `FromTime` (DateTime) - Start time (UTC)
- `ToTime` (DateTime) - End time (UTC)
- `Aggregation` (HistoryAggregation) - Bar period/type

#### Usage in CipherFeed:
- **File:** `CipherFeed.cs::GetHistoricalData()`
- **Example:**
  ```csharp
  HistoryRequestParameters historyParams = new()
  {
      Symbol = symbol,
      FromTime = sessionStartTime,
      ToTime = currentTime,
      Aggregation = new HistoryAggregationTime(Period.MIN1, symbol.HistoryType)
  };
  ```

---

### Period
- **Document:** `quantower-api\Core\History\Period.md`
- **URL:** https://api.quantower.com/docs/TradingPlatform.BusinessLayer.Period.html
- **Namespace:** `TradingPlatform.BusinessLayer`

#### Common Values:
- `Period.TICK1` - 1 tick
- `Period.SEC1` - 1 second
- `Period.MIN1` - 1 minute (used in CipherFeed)
- `Period.HOUR1` - 1 hour
- `Period.DAY1` - 1 day

#### Usage in CipherFeed:
- **File:** `CipherFeed.cs::GetHistoricalData()`
- **Purpose:** 1-minute bars for indicator initialization

---

### Indicator Base Class
- **Document:** `quantower-api\Indicators\Indicator.md`
- **URL:** https://api.quantower.com/docs/TradingPlatform.BusinessLayer.Indicator.html
- **Namespace:** `TradingPlatform.BusinessLayer`

#### Key Methods:
- `OnInit()` - Initialize indicator state (called once)
- `OnUpdate(UpdateArgs args)` - Calculate indicator on new bar
- `OnClear()` - Reset indicator state
- `High()`, `Low()`, `Close()`, `Volume()` - Access bar OHLCV data

#### Key Properties:
- `Name` (string) - Indicator display name
- `Description` (string) - Indicator description
- `SeparateWindow` (bool) - Display in separate panel (false = overlay)
- `Count` (int) - Number of bars processed
- `Symbol` (Symbol) - Symbol indicator is attached to

#### Usage in CipherFeed:
- **Files:** 
  - `Indicators\VWAPIndicator.cs` (SessionAnchoredVWAP)
  - `Indicators\VPOCIndicator.cs` (VPOCIndicator)
  - `Indicators\TWAPIndicator.cs` (TWAPIndicator)

---

### UpdateArgs
- **Document:** `quantower-api\Indicators\UpdateArgs.md`
- **URL:** https://api.quantower.com/docs/TradingPlatform.BusinessLayer.UpdateArgs.html
- **Namespace:** `TradingPlatform.BusinessLayer`

#### Properties:
- `Reason` (UpdateReason) - Why update was triggered (NewBar, HistoricalBar, etc.)

#### Usage in CipherFeed:
- **Files:** All indicator files
- **Method:** `OnUpdate(UpdateArgs args)`

---

### HistoryItemBar
- **Document:** `quantower-api\Core\History\HistoryItemBar.md`
- **URL:** https://api.quantower.com/docs/TradingPlatform.BusinessLayer.HistoryItemBar.html
- **Namespace:** `TradingPlatform.BusinessLayer`

#### Properties:
- `Open` (double) - Bar open price
- `High` (double) - Bar high price
- `Low` (double) - Bar low price
- `Close` (double) - Bar close price
- `Volume` (double) - Bar volume
- `TimeLeft` (DateTime) - Bar start time (UTC)

#### Usage in CipherFeed:
- **File:** `CipherFeed.cs::FindSessionOpenBar()`
- **Purpose:** Find first bar of session to initialize session open price

---

## 4. Enumerations

### AggressorFlag
- **Document:** `quantower-api\Core\Enums\AggressorFlag.md`
- **URL:** https://api.quantower.com/docs/TradingPlatform.BusinessLayer.AggressorFlag.html
- **Namespace:** `TradingPlatform.BusinessLayer`

#### Values:
- `Buy` (0) - Trade initiated by market buy (taker bought)
- `Sell` (1) - Trade initiated by market sell (taker sold)
- `None` (2) - Unknown or non-applicable

#### Usage in CipherFeed:
- **File:** `Models\MarketDataSnapshot.cs`
- **Purpose:** Orderflow delta calculation (buy volume vs sell volume)
- **Logic:**
  ```csharp
  if (last.AggressorFlag == AggressorFlag.Buy) {
      tradeDelta = last.Size; // Positive delta
      _cumulativeBuyVolume += last.Size;
  } else if (last.AggressorFlag == AggressorFlag.Sell) {
      tradeDelta = -last.Size; // Negative delta
      _cumulativeSellVolume += last.Size;
  }
  ```

---

### TickDirection
- **Document:** `quantower-api\Core\Enums\TickDirection.md`
- **URL:** https://api.quantower.com/docs/TradingPlatform.BusinessLayer.TickDirection.html
- **Namespace:** `TradingPlatform.BusinessLayer`

#### Values:
- `Up` (0) - Price increased
- `Down` (1) - Price decreased
- `Undefined` (2) - No price change or unknown

#### Usage in CipherFeed:
- **File:** `Models\MarketDataSnapshot.cs`
- **Properties:** `TickDirection`, `BidTickDirection`, `AskTickDirection`
- **Purpose:** Track price momentum and liquidity flow direction

---

### SymbolType
- **Document:** `quantower-api\Core\Enums\SymbolType.md`
- **URL:** https://api.quantower.com/docs/TradingPlatform.BusinessLayer.SymbolType.html
- **Namespace:** `TradingPlatform.BusinessLayer`

#### Common Values:
- `Futures` - Futures contracts (used in CipherFeed)
- `Forex` - Currency pairs
- `Stocks` - Equities
- `Options` - Option contracts

#### Usage in CipherFeed:
- **File:** `Core\SymbolSubscriptionManager.cs::Subscribe()`
- **Purpose:** Filter symbol discovery to futures contracts only
- **LINQ Query:**
  ```csharp
  List<Symbol> candidates = Core.Instance.Symbols
      .Where(s => s.ConnectionId == account.ConnectionId &&
                  s.Name.StartsWith(symbolRoot) &&
                  s.SymbolType == SymbolType.Futures &&
                  s.ExpirationDate > referenceTime)
      .OrderBy(s => s.ExpirationDate)
      .ToList();
  ```

---

### SeekOriginHistory
- **Document:** `quantower-api\Core\Enums\SeekOriginHistory.md`
- **URL:** https://api.quantower.com/docs/TradingPlatform.BusinessLayer.SeekOriginHistory.html
- **Namespace:** `TradingPlatform.BusinessLayer`

#### Values:
- `Begin` (0) - Index from start of historical data
- `End` (1) - Index from end of historical data

#### Usage in CipherFeed:
- **File:** `CipherFeed.cs::FindSessionOpenBar()`
- **Pattern:** `history[i, SeekOriginHistory.Begin]`

---

## 5. Account & Connection

### Account
- **Document:** `quantower-api\Core\BusinessObjects\Account.md`
- **URL:** https://api.quantower.com/docs/TradingPlatform.BusinessLayer.Account.html
- **Namespace:** `TradingPlatform.BusinessLayer`

#### Key Properties:
- `ConnectionId` (string) - Associated connection identifier
- `Name` (string) - Account name
- `Id` (string) - Unique account identifier

#### Usage in CipherFeed:
- **File:** `CipherFeed.cs`
- **InputParameter:** Selected by user in strategy settings
- **Purpose:** Scopes symbol lookup to specific broker/exchange connection

---

### Connection
- **Document:** `quantower-api\Core\Connections\Connection.md`
- **URL:** https://api.quantower.com/docs/TradingPlatform.BusinessLayer.Connection.html
- **Namespace:** `TradingPlatform.BusinessLayer`

#### Key Properties:
- `Id` (string) - Connection identifier (matches Account.ConnectionId)
- `Name` (string) - Connection display name

#### Usage in CipherFeed:
- **Indirect:** Via `Account.ConnectionId` for symbol filtering
- **Not directly accessed** in current codebase

---

### Core Instance (Singleton)
- **Document:** `quantower-api\Core\Core.md`
- **URL:** https://api.quantower.com/docs/TradingPlatform.BusinessLayer.Core.html
- **Namespace:** `TradingPlatform.BusinessLayer`

#### Key Properties:
- `Instance` (static) - Singleton accessor
- `Symbols` (IEnumerable<Symbol>) - All available symbols platform-wide
- `TimeUtils` (TimeUtils) - Time utilities (UTC time)

#### Usage in CipherFeed:
- **File:** `Core\SymbolSubscriptionManager.cs`
- **Purpose:** Symbol discovery via LINQ queries
- **Pattern:**
  ```csharp
  Core.Instance.Symbols.Where(s => s.SymbolType == SymbolType.Futures)
  ```

---

## 6. Metrics System

### System.Diagnostics.Metrics (Standard .NET)

#### Meter Class
- **Microsoft Docs:** https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.metrics.meter
- **Namespace:** `System.Diagnostics.Metrics`
- **Purpose:** Container for instrument instances (gauges, counters, histograms)

#### Usage in CipherFeed:
- **File:** `Core\MetricsProvider.cs`
- **Method:** `InitializeMetrics(Meter meter)`
- **Pattern:**
  ```csharp
  meter.CreateObservableGauge(
      name: "MNQ_session_open",
      observeValue: () => { /* lambda returning value */ },
      unit: "price",
      description: "Session open price for MNQ"
  );
  ```

---

#### ObservableGauge<T>
- **Microsoft Docs:** https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.metrics.meter.createobservablegauge
- **Namespace:** `System.Diagnostics.Metrics`
- **Purpose:** Metric that reports value when observed (pull-based)

#### Metrics Exposed in CipherFeed:
All defined in `Core\MetricsProvider.cs`:

**Nasdaq-100 Futures:**
- `MNQ_session_open` - Micro Nasdaq-100 session open
- `ENQ_session_open` - E-mini Nasdaq-100 session open

**S&P 500 Futures:**
- `MES_session_open` - Micro E-mini S&P 500 session open
- `EP_session_open` - E-mini S&P 500 session open

**Russell 2000 Futures:**
- `M2K_session_open` - Micro E-mini Russell 2000 session open
- `RTY_session_open` - E-mini Russell 2000 session open

**Dow Jones Futures:**
- `MYM_session_open` - Micro E-mini Dow session open
- `YM_session_open` - E-mini Dow session open

#### Quantower Integration:
- **Display:** Quantower UI ? Strategy ? Metrics panel
- **Refresh:** Poll-based (Quantower controls frequency)
- **Export:** Compatible with Prometheus/Grafana/OpenTelemetry

---

## 7. Time Management

### TimeUtils
- **Document:** `quantower-api\Core\TimeUtils.md`
- **URL:** https://api.quantower.com/docs/TradingPlatform.BusinessLayer.Core.html
- **Namespace:** `TradingPlatform.BusinessLayer`

#### Key Property:
- `DateTimeUtcNow` (DateTime) - Current platform UTC time

#### Usage in CipherFeed:
- **File:** `CipherFeed.cs::OnRun()`
- **Pattern:**
  ```csharp
  DateTime now = Core.Instance.TimeUtils.DateTimeUtcNow;
  sessionManager.Initialize(now);
  ```

#### Important Note:
**Always use UTC** for session logic and timestamps. CipherFeed converts PST session times to UTC:
- RTH: 4:00 AM - 1:45 PM PST = **12:00 - 21:45 UTC**
- ETH: 3:15 PM - 4:00 AM PST = **23:15 UTC - 12:00 UTC (next day)**

---

## 8. File-by-File API Usage

### CipherFeed.cs (Main Strategy)

#### Quantower APIs Used:
1. **Strategy Base Class**
   - `Strategy.OnCreated()` - Manager initialization
   - `Strategy.OnRun()` - Symbol subscription & indicator setup
   - `Strategy.OnStop()` - Cleanup
   - `Strategy.OnInitializeMetrics(Meter)` - Metrics registration
   - `Strategy.Log()` - Logging to Strategy Log panel

2. **Symbol & Market Data**
   - `Symbol` - 8 futures symbols (MNQ, MES, M2K, MYM, ENQ, EP, RTY, YM)
   - `Symbol.GetHistory()` - Historical data for indicators
   - `Symbol.NewLast` event - Via SymbolSubscriptionManager
   - `Symbol.NewQuote` event - Via SymbolSubscriptionManager

3. **Historical Data**
   - `HistoricalData` - Container for 1-minute bars
   - `HistoricalData.AddIndicator()` - Attach VWAP/VPOC/TWAP
   - `HistoricalData.RemoveIndicator()` - Cleanup on session change
   - `HistoryRequestParameters` - Configure history requests
   - `Period.MIN1` - 1-minute aggregation

4. **Core Instance**
   - `Core.Instance.TimeUtils.DateTimeUtcNow` - Platform UTC time

5. **Input Parameters**
   - `[InputParameter]` attribute for user configuration

6. **Enumerations**
   - `StrategyLoggingLevel` - Log message categorization

---

### Core\SymbolSubscriptionManager.cs

#### Quantower APIs Used:
1. **Symbol Discovery**
   - `Core.Instance.Symbols` - Platform-wide symbol collection
   - LINQ `.Where()` filtering
   - `SymbolType.Futures` - Filter to futures contracts
   - `Symbol.ExpirationDate` - Front-month selection
   - `Symbol.ConnectionId` - Connection scoping

2. **Event Subscription**
   - `Symbol.NewLast` event - Trade ticks
   - `Symbol.NewQuote` event - Bid/ask updates

3. **Market Data Types**
   - `Last` - Trade execution data
   - `Quote` - Order book top-of-book

4. **Account**
   - `Account.ConnectionId` - Scope symbol lookup

#### Design Pattern:
**Lambda Closures for Parallel Processing**
- Each symbol gets dedicated event handler
- `symbolRoot` captured in closure (zero dictionary lookups)
- True parallel execution (no sequential bottlenecks)

---

### Models\MarketDataSnapshot.cs

#### Quantower APIs Used:
1. **Trade Data**
   - `Last.Price` - Execution price
   - `Last.Size` - Execution size
   - `Last.Time` - Execution timestamp (UTC)
   - `Last.AggressorFlag` - Buy/Sell/None
   - `Last.TickDirection` - Up/Down/Undefined

2. **Quote Data**
   - `Quote.Bid` / `Quote.Ask` - Best prices
   - `Quote.BidSize` / `Quote.AskSize` - Best sizes
   - `Quote.Time` - Quote timestamp (UTC)
   - `Quote.BidTickDirection` / `Quote.AskTickDirection`

3. **Symbol Aggregates**
   - `Symbol.Volume` - Session total volume
   - `Symbol.Trades` - Session total trades
   - `Symbol.Delta` - Session cumulative delta

4. **Enumerations**
   - `AggressorFlag` - For delta calculation
   - `TickDirection` - For momentum tracking

#### State Management:
**Internal Cumulative Fields:**
- `_cumulativeDelta` - Running buy/sell delta
- `_cumulativeBuyVolume` / `_cumulativeSellVolume`
- `_cumulativeSizeBid` / `_cumulativeSizeAsk`

**Reset Pattern:**
- `ResetCumulativeState()` called on session change
- Ensures session-anchored calculations

---

### Indicators\VWAPIndicator.cs (SessionAnchoredVWAP)

#### Quantower APIs Used:
1. **Indicator Base Class**
   - `Indicator.OnInit()` - Initialize cumulative state
   - `Indicator.OnUpdate(UpdateArgs)` - Calculate on new bar
   - `Indicator.OnClear()` - Reset state
   - `Indicator.High()` / `Low()` / `Close()` / `Volume()` - Bar data access

2. **Input Parameters**
   - `[InputParameter]` attribute for user configuration
   - `UseTypicalPrice`, `ShowStdDevBands`, `ShowMPDBands`

3. **Properties**
   - `Name` - Indicator display name
   - `Description` - Indicator description
   - `SeparateWindow` = false (overlay on chart)
   - `Count` - Number of bars processed

#### Calculation:
**Percentage Space Formula:**
```
PricePct = (Price - SessionOpen) / SessionOpen
VWAPPct = ?(PricePct × Volume) / ?(Volume)
```

---

### Indicators\VPOCIndicator.cs

#### Quantower APIs Used:
1. **Indicator Base Class**
   - Same as VWAPIndicator (OnInit, OnUpdate, OnClear)
   - Bar data access (High, Low, Close, Volume)

2. **Symbol Properties**
   - `Symbol.TickSize` - Minimum price movement
   - Used for volume bucketing into discrete price levels

#### Calculation:
**Volume Profile Construction:**
1. Distribute volume across tick-sized price levels
2. Find VPOC (price with max volume)
3. Calculate Value Area (70% of total volume)
4. VAH = max(valueAreaPrices), VAL = min(valueAreaPrices)

---

### Indicators\TWAPIndicator.cs

#### Quantower APIs Used:
1. **Indicator Base Class**
   - Same as VWAPIndicator (OnInit, OnUpdate, OnClear)
   - Bar data access (High, Low, Close)

2. **Input Parameters**
   - `UseTypicalPrice`, `ShowStdDevBands`

#### Calculation:
**Time-Weighted Average (Equal Weight per Bar):**
```
PricePct = (Price - SessionOpen) / SessionOpen
TWAPPct = ?(PricePct) / N
```

---

### Core\SessionManager.cs

#### Quantower APIs Used:
1. **Time Management**
   - `Core.Instance.TimeUtils.DateTimeUtcNow` - Platform UTC time
   - `Last.Time` - Tick timestamp for session boundary detection

2. **Session Logic**
   - No direct Quantower API usage
   - Pure business logic for RTH/ETH detection

#### Session Boundaries (UTC):
```
00:00 ??? ETH (overnight)
12:00 ??? RTH starts ??? Session Transition #1
21:45 ??  RTH ends
23:15 ??? ETH starts ??? Session Transition #2
24:00 ??? ETH continues
```

---

### Core\SymbolLogger.cs

#### Quantower APIs Used:
1. **Strategy Logging**
   - `Strategy.Log(string, StrategyLoggingLevel)` - Via delegate
   - `StrategyLoggingLevel.Trading` - Primary log level

2. **Data Display**
   - Formats indicator values (VWAP, VPOC, TWAP)
   - Formats orderflow features (delta, volume, imbalance)
   - No direct API calls (receives data via method parameters)

---

### CSVDataExporter.cs

#### Quantower APIs Used:
1. **Symbol Reference**
   - `Symbol` - Passed to WriteSnapshot for context
   - No direct property access

2. **Indicator Values**
   - Calls public methods on indicators:
     - `SessionAnchoredVWAP.GetVWAP()` / `GetUpperStdDev()` / etc.
     - `VPOCIndicator.GetVPOC()` / `GetVAH()` / `GetVAL()`
     - `TWAPIndicator.GetTWAP()` / `GetUpperStdDev()` / `GetLowerStdDev()`

3. **No Direct Quantower API Calls**
   - Pure data serialization layer
   - Uses standard .NET I/O (`System.IO.StreamWriter`)

---

### Core\MetricsProvider.cs

#### Quantower APIs Used:
1. **Strategy Integration**
   - Called from `Strategy.OnInitializeMetrics(Meter meter)`

2. **.NET Metrics API**
   - `System.Diagnostics.Metrics.Meter`
   - `Meter.CreateObservableGauge<T>()`

3. **Symbol Access (via Delegates)**
   - Receives delegates from CipherFeed.cs
   - `getSymbol(string symbolRoot)` ? access symbolManager
   - `getSessionOpen(Symbol symbol)` ? access sessionOpenPrices

#### Pattern:
**Pull-Based Metrics with Lazy Evaluation**
- Observable lambdas capture delegates
- Quantower polls gauges at refresh interval
- Gauges execute delegates to get current values

---

## Summary: Critical API Path for Real-Time Data Flow

```
Platform Tick Arrives
    ?
Symbol.NewLast event fires (Quantower)
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
[If EnableRealtimeLogging]
    SymbolLogger.LogSymbolUpdate(...)
        ?? Strategy.Log(message, StrategyLoggingLevel.Trading)
    ?
[If EnableCSVLogging]
    CSVDataExporter.WriteSnapshot(...)
        ?? Get indicator values (VWAP, VPOC, TWAP)
        ?? StreamWriter.WriteLine(csvRow)
```

---

## Quick Reference: Most Used APIs

### Top 10 Most Critical APIs:
1. `Strategy.OnRun()` / `OnStop()` - Lifecycle management
2. `Symbol.NewLast` event - Primary data stream
3. `Symbol.NewQuote` event - Order book updates
4. `Last.Price` / `Size` / `AggressorFlag` - Trade details
5. `Symbol.Volume` / `Trades` / `Delta` - Aggregates
6. `Indicator.OnUpdate()` - Indicator calculation
7. `Strategy.Log()` - Debugging/monitoring
8. `Core.Instance.TimeUtils.DateTimeUtcNow` - Time management
9. `Symbol.GetHistory()` - Historical data
10. `Meter.CreateObservableGauge()` - Metrics

---

## Document Paths for Quantower API Repository

**Base Path:** `quantower-api\`

### Strategies:
- `Strategies\Strategy.md`
- `Strategies\InputParameter.md`
- `Strategies\StrategyLoggingLevel.md`

### Core:
- `Core\Core.md`
- `Core\BusinessObjects\Symbol.md`
- `Core\BusinessObjects\Account.md`
- `Core\Quotes\Last.md`
- `Core\Quotes\Quote.md`
- `Core\History\HistoricalData.md`
- `Core\History\Period.md`
- `Core\Requests\HistoryRequestParameters.md`
- `Core\Enums\AggressorFlag.md`
- `Core\Enums\TickDirection.md`
- `Core\Enums\SymbolType.md`
- `Core\Enums\SeekOriginHistory.md`
- `Core\TimeUtils.md`
- `Core\Connections\Connection.md`

### Indicators:
- `Indicators\Indicator.md`
- `Indicators\InputParameter.md`
- `Indicators\UpdateArgs.md`
- `Indicators\IndicatorMethods.md`

---

**End of Document**
