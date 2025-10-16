# Quantower API Reference Library

Modular API documentation for CipherFeed development. Each file contains the official Quantower documentation for a specific class/interface/enum.

## How to Use

1. **You paste** the official documentation from https://api.quantower.com/docs/ into each file
2. **I reference** only the specific class files I need when coding
3. **Token efficient** - Load ~50-200 lines instead of 2,000+ lines

## Core Classes

| File | Description | URL |
|------|-------------|-----|
| [Core.md](Core.md) | Main API entry point | [docs](https://api.quantower.com/docs/TradingPlatform.BusinessLayer.Core.html) |
| [Strategy.md](Strategy.md) | Base strategy class | [docs](https://api.quantower.com/docs/TradingPlatform.BusinessLayer.Strategy.html) |
| [Account.md](Account.md) | Trading account | [docs](https://api.quantower.com/docs/TradingPlatform.BusinessLayer.Account.html) |
| [Asset.md](Asset.md) | Asset/currency | [docs](https://api.quantower.com/docs/TradingPlatform.BusinessLayer.Asset.html) |
| [Exchange.md](Exchange.md) | Exchange info | [docs](https://api.quantower.com/docs/TradingPlatform.BusinessLayer.Exchange.html) |
| [Symbol.md](Symbol.md) | Symbol/instrument | [docs](https://api.quantower.com/docs/TradingPlatform.BusinessLayer.Symbol.html) |
| [SymbolGroup.md](SymbolGroup.md) | Symbol grouping | [docs](https://api.quantower.com/docs/TradingPlatform.BusinessLayer.SymbolGroup.html) |

## Trading Classes

| File | Description | URL |
|------|-------------|-----|
| [Order.md](Order.md) | Pending order | [docs](https://api.quantower.com/docs/TradingPlatform.BusinessLayer.Order.html) |
| [Position.md](Position.md) | Open position | [docs](https://api.quantower.com/docs/TradingPlatform.BusinessLayer.Position.html) |
| [Trade.md](Trade.md) | Executed trade | [docs](https://api.quantower.com/docs/TradingPlatform.BusinessLayer.Trade.html) |

## Market Data - Quotes

| File | Description | URL |
|------|-------------|-----|
| [Quote.md](Quote.md) | Level 1 quote (bid/ask) | [docs](https://api.quantower.com/docs/TradingPlatform.BusinessLayer.Quote.html) |
| [Last.md](Last.md) | Trade tick | [docs](https://api.quantower.com/docs/TradingPlatform.BusinessLayer.Last.html) |
| [Level2Quote.md](Level2Quote.md) | Level 2 quote | [docs](https://api.quantower.com/docs/TradingPlatform.BusinessLayer.Level2Quote.html) |
| [DOMQuote.md](DOMQuote.md) | Depth of market | [docs](https://api.quantower.com/docs/TradingPlatform.BusinessLayer.DOMQuote.html) |
| [DayBar.md](DayBar.md) | Daily bar summary | [docs](https://api.quantower.com/docs/TradingPlatform.BusinessLayer.DayBar.html) |

## Market Data - Depth of Market

| File | Description | URL |
|------|-------------|-----|
| [DepthOfMarket.md](DepthOfMarket.md) | DOM access | [docs](https://api.quantower.com/docs/TradingPlatform.BusinessLayer.DepthOfMarket.html) |
| [DepthOfMarketAggregatedCollections.md](DepthOfMarketAggregatedCollections.md) | Aggregated DOM | [docs](https://api.quantower.com/docs/TradingPlatform.BusinessLayer.DepthOfMarketAggregatedCollections.html) |
| [GetDepthOfMarketParameters.md](GetDepthOfMarketParameters.md) | DOM request params | [docs](https://api.quantower.com/docs/TradingPlatform.BusinessLayer.GetDepthOfMarketParameters.html) |
| [GetLevel2ItemsParameters.md](GetLevel2ItemsParameters.md) | Level2 request params | [docs](https://api.quantower.com/docs/TradingPlatform.BusinessLayer.GetLevel2ItemsParameters.html) |
| [Level2Item.md](Level2Item.md) | Level 2 item | [docs](https://api.quantower.com/docs/TradingPlatform.BusinessLayer.Level2Item.html) |
| [AggregateMethod.md](AggregateMethod.md) | Aggregation methods | [docs](https://api.quantower.com/docs/TradingPlatform.BusinessLayer.AggregateMethod.html) |

## Historical Data

| File | Description | URL |
|------|-------------|-----|
| [HistoricalData.md](HistoricalData.md) | Historical data access | [docs](https://api.quantower.com/docs/TradingPlatform.BusinessLayer.HistoricalData.html) |
| [HistoryItemBar.md](HistoryItemBar.md) | OHLCV bar | [docs](https://api.quantower.com/docs/TradingPlatform.BusinessLayer.HistoryItemBar.html) |
| [HistoryItemLast.md](HistoryItemLast.md) | Historical trade | [docs](https://api.quantower.com/docs/TradingPlatform.BusinessLayer.HistoryItemLast.html) |
| [HistoryItemTick.md](HistoryItemTick.md) | Historical tick | [docs](https://api.quantower.com/docs/TradingPlatform.BusinessLayer.HistoryItemTick.html) |
| [HistoryRequestParameters.md](HistoryRequestParameters.md) | History request params | [docs](https://api.quantower.com/docs/TradingPlatform.BusinessLayer.HistoryRequestParameters.html) |

## History Aggregations

| File | Description | URL |
|------|-------------|-----|
| [HistoryAggregation.md](HistoryAggregation.md) | Base aggregation | [docs](https://api.quantower.com/docs/TradingPlatform.BusinessLayer.HistoryAggregation.html) |
| [HistoryAggregationTime.md](HistoryAggregationTime.md) | Time-based bars | [docs](https://api.quantower.com/docs/TradingPlatform.BusinessLayer.HistoryAggregationTime.html) |
| [HistoryAggregationRangeBars.md](HistoryAggregationRangeBars.md) | Range bars | [docs](https://api.quantower.com/docs/TradingPlatform.BusinessLayer.HistoryAggregationRangeBars.html) |
| [HistoryAggregationTick.md](HistoryAggregationTick.md) | Tick bars | [docs](https://api.quantower.com/docs/TradingPlatform.BusinessLayer.HistoryAggregationTick.html) |
| [HistoryAggregationRenko.md](HistoryAggregationRenko.md) | Renko charts | [docs](https://api.quantower.com/docs/TradingPlatform.BusinessLayer.HistoryAggregationRenko.html) |
| [HistoryAggregationPointsAndFigures.md](HistoryAggregationPointsAndFigures.md) | P&F charts | [docs](https://api.quantower.com/docs/TradingPlatform.BusinessLayer.HistoryAggregationPointsAndFigures.html) |
| [RenkoStyle.md](RenkoStyle.md) | Renko styles | [docs](https://api.quantower.com/docs/TradingPlatform.BusinessLayer.RenkoStyle.html) |

## Period & Time

| File | Description | URL |
|------|-------------|-----|
| [Period.md](Period.md) | Time period | [docs](https://api.quantower.com/docs/TradingPlatform.BusinessLayer.Period.html) |
| [BasePeriod.md](BasePeriod.md) | Base period enum | [docs](https://api.quantower.com/docs/TradingPlatform.BusinessLayer.BasePeriod.html) |

## Order Request Parameters

| File | Description | URL |
|------|-------------|-----|
| [OrderRequestParameters.md](OrderRequestParameters.md) | Base order params | [docs](https://api.quantower.com/docs/TradingPlatform.BusinessLayer.OrderRequestParameters.html) |
| [PlaceOrderRequestParameters.md](PlaceOrderRequestParameters.md) | Place order params | [docs](https://api.quantower.com/docs/TradingPlatform.BusinessLayer.PlaceOrderRequestParameters.html) |
| [ModifyOrderRequestParameters.md](ModifyOrderRequestParameters.md) | Modify order params | [docs](https://api.quantower.com/docs/TradingPlatform.BusinessLayer.ModifyOrderRequestParameters.html) |
| [CancelOrderRequestParameters.md](CancelOrderRequestParameters.md) | Cancel order params | [docs](https://api.quantower.com/docs/TradingPlatform.BusinessLayer.CancelOrderRequestParameters.html) |
| [ClosePositionRequestParameters.md](ClosePositionRequestParameters.md) | Close position params | [docs](https://api.quantower.com/docs/TradingPlatform.BusinessLayer.ClosePositionRequestParameters.html) |

## Other Request Parameters

| File | Description | URL |
|------|-------------|-----|
| [TradingRequestParameters.md](TradingRequestParameters.md) | Base trading params | [docs](https://api.quantower.com/docs/TradingPlatform.BusinessLayer.TradingRequestParameters.html) |
| [RequestParameters.md](RequestParameters.md) | Base request params | [docs](https://api.quantower.com/docs/TradingPlatform.BusinessLayer.RequestParameters.html) |
| [PnLRequestParameters.md](PnLRequestParameters.md) | P&L request params | [docs](https://api.quantower.com/docs/TradingPlatform.BusinessLayer.PnLRequestParameters.html) |
| [ReportRequestParameters.md](ReportRequestParameters.md) | Report request params | [docs](https://api.quantower.com/docs/TradingPlatform.BusinessLayer.ReportRequestParameters.html) |
| [GetSymbolRequestParameters.md](GetSymbolRequestParameters.md) | Symbol request params | [docs](https://api.quantower.com/docs/TradingPlatform.BusinessLayer.GetSymbolRequestParameters.html) |

## Connection

| File | Description | URL |
|------|-------------|-----|
| [Connection.md](Connection.md) | Connection class | [docs](https://api.quantower.com/docs/TradingPlatform.BusinessLayer.Connection.html) |
| [ConnectionInfo.md](ConnectionInfo.md) | Connection info | [docs](https://api.quantower.com/docs/TradingPlatform.BusinessLayer.ConnectionInfo.html) |

## Settings Items

| File | Description | URL |
|------|-------------|-----|
| [SettingItemAction.md](SettingItemAction.md) | Button setting | [docs](https://api.quantower.com/docs/TradingPlatform.BusinessLayer.SettingItemAction.html) |
| [SettingItemBoolean.md](SettingItemBoolean.md) | Checkbox setting | [docs](https://api.quantower.com/docs/TradingPlatform.BusinessLayer.SettingItemBoolean.html) |
| [SettingItemColor.md](SettingItemColor.md) | Color picker | [docs](https://api.quantower.com/docs/TradingPlatform.BusinessLayer.SettingItemColor.html) |
| [SettingItemDouble.md](SettingItemDouble.md) | Double numeric | [docs](https://api.quantower.com/docs/TradingPlatform.BusinessLayer.SettingItemDouble.html) |
| [SettingItemInteger.md](SettingItemInteger.md) | Integer numeric | [docs](https://api.quantower.com/docs/TradingPlatform.BusinessLayer.SettingItemInteger.html) |
| [SettingItemGroup.md](SettingItemGroup.md) | Tab group | [docs](https://api.quantower.com/docs/TradingPlatform.BusinessLayer.SettingItemGroup.html) |
| [SettingItemPeriod.md](SettingItemPeriod.md) | Period selector | [docs](https://api.quantower.com/docs/TradingPlatform.BusinessLayer.SettingItemPeriod.html) |
| [SettingItemSelector.md](SettingItemSelector.md) | Dropdown selector | [docs](https://api.quantower.com/docs/TradingPlatform.BusinessLayer.SettingItemSelector.html) |
| [SettingItemSeparatorGroup.md](SettingItemSeparatorGroup.md) | GroupBox | [docs](https://api.quantower.com/docs/TradingPlatform.BusinessLayer.SettingItemSeparatorGroup.html) |
| [SettingItemString.md](SettingItemString.md) | Text input | [docs](https://api.quantower.com/docs/TradingPlatform.BusinessLayer.SettingItemString.html) |

## Volume Analysis

| File | Description | URL |
|------|-------------|-----|
| [VolumeAnalysisManager.md](VolumeAnalysisManager.md) | VA manager | [docs](https://api.quantower.com/docs/TradingPlatform.BusinessLayer.VolumeAnalysisManager.html) |
| [VolumeAnalysisData.md](VolumeAnalysisData.md) | VA data container | [docs](https://api.quantower.com/docs/TradingPlatform.BusinessLayer.VolumeAnalysisData.html) |
| [VolumeAnalysisItem.md](VolumeAnalysisItem.md) | VA calculation item | [docs](https://api.quantower.com/docs/TradingPlatform.BusinessLayer.VolumeAnalysisItem.html) |
| [IVolumeAnalysisCalculationTask.md](IVolumeAnalysisCalculationTask.md) | VA calculation task | [docs](https://api.quantower.com/docs/TradingPlatform.BusinessLayer.IVolumeAnalysisCalculationTask.html) |
| [IVolumeAnalysisCalculationProgress.md](IVolumeAnalysisCalculationProgress.md) | VA progress tracking | [docs](https://api.quantower.com/docs/TradingPlatform.BusinessLayer.IVolumeAnalysisCalculationProgress.html) |
| [VolumeAnalysisCalculationRequest.md](VolumeAnalysisCalculationRequest.md) | VA request | [docs](https://api.quantower.com/docs/TradingPlatform.BusinessLayer.VolumeAnalysisCalculationRequest.html) |
| [VolumeAnalysisCalculationParameters.md](VolumeAnalysisCalculationParameters.md) | VA params | [docs](https://api.quantower.com/docs/TradingPlatform.BusinessLayer.VolumeAnalysisCalculationParameters.html) |
| [VolumeAnalysisDataEventArgs.md](VolumeAnalysisDataEventArgs.md) | VA event args | [docs](https://api.quantower.com/docs/TradingPlatform.BusinessLayer.VolumeAnalysisDataEventArgs.html) |
| [VolumeAnalysisCalculationState.md](VolumeAnalysisCalculationState.md) | VA state enum | [docs](https://api.quantower.com/docs/TradingPlatform.BusinessLayer.VolumeAnalysisCalculationState.html) |
| [VolumeAnalysisField.md](VolumeAnalysisField.md) | VA field enum | [docs](https://api.quantower.com/docs/TradingPlatform.BusinessLayer.VolumeAnalysisField.html) |

## Enums

| File | Description | URL |
|------|-------------|-----|
| [PriceType.md](PriceType.md) | Price type enum | [docs](https://api.quantower.com/docs/TradingPlatform.BusinessLayer.PriceType.html) |
| [BasePeriod.md](BasePeriod.md) | Base period enum | [docs](https://api.quantower.com/docs/TradingPlatform.BusinessLayer.BasePeriod.html) |
| [AggregateMethod.md](AggregateMethod.md) | Aggregation method enum | [docs](https://api.quantower.com/docs/TradingPlatform.BusinessLayer.AggregateMethod.html) |
| [VolumeAnalysisCalculationState.md](VolumeAnalysisCalculationState.md) | VA state enum | [docs](https://api.quantower.com/docs/TradingPlatform.BusinessLayer.VolumeAnalysisCalculationState.html) |
| [VolumeAnalysisField.md](VolumeAnalysisField.md) | VA field enum | [docs](https://api.quantower.com/docs/TradingPlatform.BusinessLayer.VolumeAnalysisField.html) |

---

## Total Files: 68

All files are empty placeholders. Paste the official documentation from https://api.quantower.com/docs/ into each file as needed.
