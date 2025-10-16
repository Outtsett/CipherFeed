# Orderflow Features Implementation

## Overview
The CipherFeed strategy now captures comprehensive orderflow and market microstructure data from Quantower for all monitored symbols in real-time.

## MarketDataSnapshot Class
A new data structure that stores all orderflow features for a single symbol at a point in time:

### Price and Trade Information
- **Last**: Last traded price
- **LastSize**: Size of the last trade
- **Aggressor**: Buy/Sell side of the trade (AggressorFlag)
- **TickDirection**: Direction of price movement

### Bid/Ask Information
- **BidPrice** / **AskPrice**: Current bid and ask prices
- **BidSize** / **AskSize**: Sizes at bid and ask
- **BidTickDirection** / **AskTickDirection**: Direction of bid/ask movement
- **TimeBid** / **TimeAsk**: Timestamps of last bid/ask updates

### Volume and Delta
- **Volume**: Total volume traded
- **Trades**: Total number of trades
- **Delta**: Net buy/sell pressure (Symbol.Delta property)
- **CumulativeDelta**: Running total of delta from session start

### Buy/Sell Volume
- **BuyVolume** / **SellVolume**: Cumulative buy and sell volumes
- **BuyVolumePercent** / **SellVolumePercent**: Percentage breakdown
- **BuyTrades** / **SellTrades**: Count of buy and sell trades

### Imbalance
- **Imbalance**: Difference between buy and sell volume
- **ImbalancePercent**: Imbalance as percentage of total volume

### Average Sizes
- **AverageSize**: Average trade size
- **AverageBuySize**: Average buy trade size
- **AverageSellSize**: Average sell trade size

### Max Trade Volume
- **MaxOneTradeVolume**: Largest single trade
- **MaxOneTradeVolumePercent**: As percentage of total volume

### Filtered Volume
- **FilteredVolume**: Volume after filtering (if applicable)
- **FilteredVolumePercent**: As percentage
- **FilteredBuyVolume** / **FilteredSellVolume**: Filtered by side
- **FilteredBuyVolumePercent** / **FilteredSellVolumePercent**: As percentages

### VWAP Bid/Ask
- **VWAPBid** / **VWAPAsk**: Volume-weighted average price for bid/ask
- **CumulativeSizeBid** / **CumulativeSizeAsk**: Running totals
- **CumulativeSize**: Total cumulative size
- **AskTradeSize**: Size traded at ask

### Session Reference
- **SessionOpen**: Opening price for percentage calculations
- **Timestamp**: Time of the snapshot

## Data Collection

### Event Handlers
1. **OnNewLast**: Captures trade data when a new trade occurs
   - Updates trade information (price, size, aggressor, direction)
   - Accumulates buy/sell volumes based on aggressor flag
   - Updates cumulative delta
   - Calculates volume percentages and imbalance

2. **OnNewQuote**: Captures bid/ask data when quotes update
   - Updates bid and ask prices and sizes
   - Tracks tick directions for both sides
   - Accumulates cumulative bid/ask sizes

### Cumulative Tracking
The strategy maintains session-scoped cumulative values in dictionaries:
- `cumulativeDelta`: Running delta from session start
- `cumulativeBuyVolume`: Total buy volume
- `cumulativeSellVolume`: Total sell volume
- `cumulativeSizeBid`: Cumulative bid size
- `cumulativeSizeAsk`: Cumulative ask size

All cumulative values are **reset at session boundaries** (RTH ? ETH transitions).

## Accessing the Data

### Public Methods
```csharp
// Get snapshot by Symbol object
MarketDataSnapshot snapshot = strategy.GetMarketDataSnapshot(symbol);

// Get snapshot by symbol root name
MarketDataSnapshot snapshot = strategy.GetMarketDataSnapshot("MNQ");
```

### Logging
Set the **"Log Orderflow Features"** input parameter to `true` to include orderflow data in the periodic logs. This will display:
- Volume and trade counts
- Buy/Sell volume breakdown with percentages
- Delta and cumulative delta
- Imbalance metrics
- Current bid/ask levels
- Last trade details with aggressor flag

## Symbol Coverage
Orderflow data is collected for all configured symbols:
- MNQ (Micro E-mini Nasdaq)
- MES (Micro E-mini S&P 500)
- M2K (Micro E-mini Russell 2000)
- MYM (Micro E-mini Dow)
- ENQ (E-mini Nasdaq)
- EP (E-mini S&P 500)
- RTY (E-mini Russell 2000)
- YM (E-mini Dow)

## Session Handling
- All cumulative metrics reset at session boundaries
- Session open price is captured for each session
- Data is properly cleaned up on session changes
- Separate tracking for RTH and ETH sessions

## Use Cases
This rich orderflow data enables:
1. **Order Flow Analysis**: Track buy/sell pressure in real-time
2. **Market Microstructure Studies**: Analyze bid/ask dynamics
3. **Delta Divergence Detection**: Compare price movement vs. delta
4. **Volume Profile Analysis**: Understand volume distribution
5. **Imbalance Trading**: Identify supply/demand imbalances
6. **ML Feature Engineering**: Use as inputs for trading models
7. **Smart Order Routing**: Assess liquidity and market depth

## Future Enhancements
Potential additions:
- Level 2 DOM aggregation (deeper book analysis)
- Volume analysis calculations (POC, VAH, VAL from live data)
- Trade size distribution histograms
- Time & sales filtering by size thresholds
- Iceberg order detection
- Liquidity heatmaps
- Spread analytics
