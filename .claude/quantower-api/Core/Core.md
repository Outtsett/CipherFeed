# Core Class

Paste official Quantower documentation here.

**URL**: https://api.quantower.com/docs/TradingPlatform.BusinessLayer.Core.html
Namespace TradingPlatform.BusinessLayer
Classes
Account
Contains all user's account information

Asset
Defines asset entity

BuiltInIndicators
CancelOrderRequestParameters
ClosePositionRequestParameters
Connection
Represents information about connection and provides an access to the current trading information(Symbols, Orders, Position, Accounts etc.).

ConnectionInfo
Represents all needed parameters for the connection constructing process.

Core
The main entry point in the API. Core keeps access to all business logic entities and their properties: connections, accounts, symbols, positions, orders, etc. Some of them can be reached through using managers or directly via specified collections. You can always access the Core object via static Core.Instance property.

CorporateAction
Represents information about corporate action.

CryptoAccount
CryptoAssetBalances
DOMQuote
Represent access to DOM2 quote, which contains Bids and Asks.

DayBar
Represent access to DayBar quote, which contains summary information about instrument prices.

DepthOfMarket
Represent access to level2 data.

DepthOfMarketAggregatedCollections
Leve2 data. Contains Bids and Ask collections

Exchange
Contains all information which belong to the given exchange

GetDepthOfMarketParameters
Represent parameters of DepthOfMarket

GetLevel2ItemsParameters
Represent parameters of request for Leve2Item collection

GetSymbolRequestParameters
HistoricalData
Represent access to historical data information and indicators control.

HistoryAggregation
HistoryAggregationHeikenAshi
HistoryAggregationKagi
HistoryAggregationLineBreak
HistoryAggregationPointsAndFigures
HistoryAggregationRangeBars
HistoryAggregationRenko
HistoryAggregationTick
HistoryAggregationTime
HistoryItemBar
Represents historical data bar item

HistoryItemLast
Represents historical data trade item

HistoryItemTick
Represents historical data tick item

HistoryRequestParameters
Resolves a history request parameters per symbol

Indicator
Base class for all indicators.

InputParameterAttribute
Use this attribute to mark input parameters of your script. You will see them in the settings screen on adding

Last
Represent access to trade information.

Level2Item
Represent access to level2 item.

Level2Quote
Represent access to Level2 quote.

Mark
ModifyOrderRequestParameters
Order
Represents trading information about pending order

OrderRequestParameters
OrdersHistoryRequestParameters
PaintChartEventArgs
PlaceOrderRequestParameters
PnLRequestParameters
Position
Represents trading information about related position

Quote
Represent access to quote information.

ReportRequestParameters
RequestParameters
SettingItemAction
Typecasts setting as Button item

SettingItemBoolean
Typecasts setting as CheckBox item

SettingItemColor
Typecasts setting as Color item

SettingItemDouble
Typecasts setting as NumericUpDown item

SettingItemDoubleRange
SettingItemGroup
Typecasts setting as TabControl item

SettingItemInteger
Typecasts setting as NumericUpDown item

SettingItemIntegerRange
SettingItemLong
SettingItemPeriod
Typecasts setting as Period item

SettingItemSelector
Typecasts setting as ComboBox item

SettingItemSeparatorGroup
Typecasts setting as GroupBox item

SettingItemString
Typecasts setting as TextBox item

Strategy
The base class for strategies

StrategyMetric
Symbol
Represent access to symbol information and properties.

SymbolGroup
Provides possibility to group and sort symbols for each connection

Trade
Represents information about trade.

TradesHistoryRequestParameters
TradingRequestParameters
VolumeAnalysisCalculationParameters
Provides VA calculation parameters

VolumeAnalysisCalculationRequest
Provides VA calculation request per Symbol

VolumeAnalysisData
VolumeAnalysisDataEventArgs
VolumeAnalysisItem
Represent item with Volume Analysis calculation results

VolumeAnalysisManager
Volume Analysis calculations

Structs
Period
Represents mechanism for supporting predefined and custom periods

Interfaces
IHistoryAggregationHistoryTypeSupport
IVolumeAnalysisCalculationProgress
IVolumeAnalysisCalculationTask
Enums
AggregateMethod
Aggregation method

BasePeriod
Period that can be used as a basis for history aggregations

LineStyle
Specifies the style of indicator line.

MaMode
Moving average mode

PointsAndFiguresStyle
PriceType
RenkoStyle
StrategyLoggingLevel
StrategyState
VolumeAnalysisCalculationState
VolumeAnalysisField
