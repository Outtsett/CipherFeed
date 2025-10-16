# Symbol Class

Paste official Quantower documentation here.

**URL**: https://api.quantower.com/docs/TradingPlatform.BusinessLayer.Symbol.html
Class Symbol
Represent access to symbol information and properties.

Namespace: TradingPlatform.BusinessLayer
Syntax
public class Symbol : BusinessObject
Constructors
Symbol()
Represent access to symbol information and properties.

Declaration
protected Symbol()
Fields
SPOT_SYMBOL_ID
Represent access to symbol information and properties.

Declaration
public const string SPOT_SYMBOL_ID = "spotSymbolId"
Field Value
Type	Description
string	
TRADING_SYMBOL_ID
Represent access to symbol information and properties.

Declaration
public const string TRADING_SYMBOL_ID = "TradingSymbol"
Field Value
Type	Description
string	
historyMetadata
Represent access to symbol information and properties.

Declaration
protected HistoryMetadata historyMetadata
Field Value
Type	Description
HistoryMetadata	
volumeAnalysisMetadata
Represent access to symbol information and properties.

Declaration
protected VolumeAnalysisMetadata volumeAnalysisMetadata
Field Value
Type	Description
VolumeAnalysisMetadata	
Properties
Ask
Gets Ask price

Declaration
public double Ask { get; }
Property Value
Type	Description
double	
AskSize
Gets Ask size

Declaration
public double AskSize { get; }
Property Value
Type	Description
double	
AvailableFutures
Represent access to symbol information and properties.

Declaration
public AvailableDerivatives AvailableFutures { get; }
Property Value
Type	Description
AvailableDerivatives	
AvailableOptions
Represent access to symbol information and properties.

Declaration
public AvailableDerivatives AvailableOptions { get; }
Property Value
Type	Description
AvailableDerivatives	
AvailableOptionsExchanges
Represent access to symbol information and properties.

Declaration
public Exchange[] AvailableOptionsExchanges { get; }
Property Value
Type	Description
Exchange[]	
AverageTradedPrice
Represent access to symbol information and properties.

Declaration
public double AverageTradedPrice { get; }
Property Value
Type	Description
double	
Bid
Gets Bid price

Declaration
public double Bid { get; }
Property Value
Type	Description
double	
BidSize
Gets Bid size

Declaration
public double BidSize { get; }
Property Value
Type	Description
double	
BottomPriceLimit
Represent access to symbol information and properties.

Declaration
public double BottomPriceLimit { get; }
Property Value
Type	Description
double	
Change
Gets change value between Bid/Last and Close price

Declaration
public double Change { get; }
Property Value
Type	Description
double	
ChangePercentage
Gets Change percentage value

Declaration
public double ChangePercentage { get; }
Property Value
Type	Description
double	
ComplexId
Represent access to symbol information and properties.

Declaration
public SymbolComplexIdentifier ComplexId { get; }
Property Value
Type	Description
SymbolComplexIdentifier	
CurrentSessionsInfo
Represent access to symbol information and properties.

Declaration
public SessionsContainer CurrentSessionsInfo { get; }
Property Value
Type	Description
SessionsContainer	
Delta
Represent access to symbol information and properties.

Declaration
public double Delta { get; }
Property Value
Type	Description
double	
DeltaCalculationType
Represent access to symbol information and properties.

Declaration
public DeltaCalculationType DeltaCalculationType { get; }
Property Value
Type	Description
DeltaCalculationType	
DepthOfMarket
Gets Level2 data

Declaration
public DepthOfMarket DepthOfMarket { get; }
Property Value
Type	Description
DepthOfMarket	
Description
Gets symbol description

Declaration
public string Description { get; }
Property Value
Type	Description
string	
Exchange
Gets Exchange of current symbol

Declaration
public Exchange Exchange { get; protected set; }
Property Value
Type	Description
Exchange	
ExchangeId
Gets Exchange id of current symbol

Declaration
public string ExchangeId { get; }
Property Value
Type	Description
string	
ExpirationDate
Gets derivative expiration date

Declaration
public DateTime ExpirationDate { get; }
Property Value
Type	Description
DateTime	
FundingRate
Represent access to symbol information and properties.

Declaration
public double FundingRate { get; }
Property Value
Type	Description
double	
FundingTime
Represent access to symbol information and properties.

Declaration
public DateTime? FundingTime { get; }
Property Value
Type	Description
DateTime?	
FutureContractType
Represent access to symbol information and properties.

Declaration
public FutureContractType? FutureContractType { get; }
Property Value
Type	Description
FutureContractType?	
Gamma
Represent access to symbol information and properties.

Declaration
public double Gamma { get; }
Property Value
Type	Description
double	
Group
Gets SymbolGroup

Declaration
public SymbolGroup Group { get; protected set; }
Property Value
Type	Description
SymbolGroup	
High
Gets high price

Declaration
public double High { get; }
Property Value
Type	Description
double	
HistoryType
Default history type

Declaration
public HistoryType HistoryType { get; }
Property Value
Type	Description
HistoryType	
IV
Represent access to symbol information and properties.

Declaration
public double IV { get; }
Property Value
Type	Description
double	
Id
Gets symbol Id

Declaration
public string Id { get; protected set; }
Property Value
Type	Description
string	
Last
Gets last price

Declaration
public double Last { get; }
Property Value
Type	Description
double	
LastDateTime
Gets last time

Declaration
public DateTime LastDateTime { get; }
Property Value
Type	Description
DateTime	
LastSize
Gets last size

Declaration
public double LastSize { get; }
Property Value
Type	Description
double	
LastTradingDate
Gets derivative last trading date

Declaration
public DateTime LastTradingDate { get; }
Property Value
Type	Description
DateTime	
LotSize
Amount of base asset Product for one lot.

Declaration
public double LotSize { get; }
Property Value
Type	Description
double	
LotStep
Step of the lot changes

Declaration
public double LotStep { get; }
Property Value
Type	Description
double	
Low
Gets low price

Declaration
public double Low { get; }
Property Value
Type	Description
double	
Mark
Gets mark price

Declaration
public double Mark { get; }
Property Value
Type	Description
double	
MarkSize
Gets mark size

Declaration
public double MarkSize { get; }
Property Value
Type	Description
double	
MaturityDate
Gets derivative maturity date

Declaration
public DateTime MaturityDate { get; }
Property Value
Type	Description
DateTime	
MaxLot
The highest trade allowed

Declaration
public double MaxLot { get; }
Property Value
Type	Description
double	
MinLot
The lowest trade allowed

Declaration
public double MinLot { get; }
Property Value
Type	Description
double	
MinVolumeAnalysisTickSize
Represent access to symbol information and properties.

Declaration
public double MinVolumeAnalysisTickSize { get; }
Property Value
Type	Description
double	
Name
Gets symbol name

Declaration
public string Name { get; protected set; }
Property Value
Type	Description
string	
NettingType
Gets symbol NettingType

Declaration
public NettingType NettingType { get; }
Property Value
Type	Description
NettingType	
NotionalValueStep
Step of the notional value changes

Declaration
public double NotionalValueStep { get; }
Property Value
Type	Description
double	
Open
Gets open price

Declaration
public double Open { get; }
Property Value
Type	Description
double	
OpenInterest
Represent access to symbol information and properties.

Declaration
public double OpenInterest { get; }
Property Value
Type	Description
double	
PrevClose
Gets previous close price

Declaration
public double PrevClose { get; }
Property Value
Type	Description
double	
Product
Gets symbol base Asset

Declaration
public Asset Product { get; protected set; }
Property Value
Type	Description
Asset	
QuoteAssetVolume
Gets quote asset volume value

Declaration
public double QuoteAssetVolume { get; }
Property Value
Type	Description
double	
QuoteDateTime
Gets quote time

Declaration
public DateTime QuoteDateTime { get; }
Property Value
Type	Description
DateTime	
QuoteDelay
Returns delay with which quote come in platform.

Declaration
public TimeSpan QuoteDelay { get; }
Property Value
Type	Description
TimeSpan	
QuotingCurrency
Gets symbol counter Asset

Declaration
public Asset QuotingCurrency { get; protected set; }
Property Value
Type	Description
Asset	
QuotingType
Gets current SymbolQuotingType

Declaration
public SymbolQuotingType QuotingType { get; }
Property Value
Type	Description
SymbolQuotingType	
Rho
Represent access to symbol information and properties.

Declaration
public double Rho { get; }
Property Value
Type	Description
double	
Root
Gets derivative underlier name

Declaration
public string Root { get; }
Property Value
Type	Description
string	
Spread
Gets spread value between Bid and Ask

Declaration
public double Spread { get; }
Property Value
Type	Description
double	
SpreadPercentage
Gets Spread percentage value

Declaration
public double SpreadPercentage { get; }
Property Value
Type	Description
double	
SymbolType
Gets symbol type

Declaration
public SymbolType SymbolType { get; }
Property Value
Type	Description
SymbolType	
Theta
Represent access to symbol information and properties.

Declaration
public double Theta { get; }
Property Value
Type	Description
double	
TickSize
Gets cached tick size if it available, else tries to obtain GetTickSize(double) with Last, Bid, Ask, first element of VariableTick list otherwise - DOUBLE_UNDEFINED

Declaration
public double TickSize { get; }
Property Value
Type	Description
double	
Ticks
Gets ticks amount

Declaration
public long Ticks { get; }
Property Value
Type	Description
long	
TopPriceLimit
Represent access to symbol information and properties.

Declaration
public double TopPriceLimit { get; }
Property Value
Type	Description
double	
TotalBuyQuantity
Represent access to symbol information and properties.

Declaration
public double TotalBuyQuantity { get; }
Property Value
Type	Description
double	
TotalSellQuantity
Represent access to symbol information and properties.

Declaration
public double TotalSellQuantity { get; }
Property Value
Type	Description
double	
Trades
Gets trades amount

Declaration
public long Trades { get; }
Property Value
Type	Description
long	
Underlier
Gets derivative underlier symbol

Declaration
public Symbol Underlier { get; }
Property Value
Type	Description
Symbol	
UnderlierId
Gets derivative underlier symbol id

Declaration
public string UnderlierId { get; }
Property Value
Type	Description
string	
VariableTickList
Stores list of symbol ticksizes

Declaration
public List<VariableTick> VariableTickList { get; }
Property Value
Type	Description
List<VariableTick>	
Vega
Represent access to symbol information and properties.

Declaration
public double Vega { get; }
Property Value
Type	Description
double	
Volume
Gets volume value

Declaration
public double Volume { get; }
Property Value
Type	Description
double	
VolumeType
Gets SymbolVolumeType

Declaration
public SymbolVolumeType VolumeType { get; }
Property Value
Type	Description
SymbolVolumeType	
Methods
CalculatePrice(double, double)
Calculates new price which equal to given price shifted by a number of given ticks

Declaration
public double CalculatePrice(double price, double ticks)
Parameters
Type	Name	Description
double	price	
double	ticks	
Returns
Type	Description
double	
CalculateTicks(double, double)
Calculates ticks between two prices

Declaration
public double CalculateTicks(double price1, double price2)
Parameters
Type	Name	Description
double	price1	
double	price2	
Returns
Type	Description
double	
DetermineTickDirection(double, double, TickDirection)
Represent access to symbol information and properties.

Declaration
public static TickDirection DetermineTickDirection(double previousPrice, double currentPrice, TickDirection prevItemTickDirection)
Parameters
Type	Name	Description
double	previousPrice	
double	currentPrice	
TickDirection	prevItemTickDirection	
Returns
Type	Description
TickDirection	
Equals(object)
Determines whether the specified object is equal to the current object.

Declaration
public override bool Equals(object obj)
Parameters
Type	Name	Description
object	obj	
The object to compare with the current object.

Returns
Type	Description
bool	
true if the specified object is equal to the current object; otherwise, false.

Overrides
object.Equals(object)
Equals(Symbol)
Indicates whether the current object is equal to another object of the same type.

Declaration
public bool Equals(Symbol other)
Parameters
Type	Name	Description
Symbol	other	
An object to compare with this object.

Returns
Type	Description
bool	
true if the current object is equal to the other parameter; otherwise, false.

FindVariableTick(double)
Returns VariableTick if it can be retrived from VariableTick list by price or null

Declaration
public VariableTick FindVariableTick(double price)
Parameters
Type	Name	Description
double	price	
Returns
Type	Description
VariableTick	
FormatOffset(double, string)
Returns string with formatted ticks value

Declaration
public string FormatOffset(double offset, string dimension = "ticks")
Parameters
Type	Name	Description
double	offset	
string	dimension	
Returns
Type	Description
string	
FormatPrice(double)
Formats price value to the appropriative string with a counting on tick precision.

Declaration
public string FormatPrice(double price)
Parameters
Type	Name	Description
double	price	
Returns
Type	Description
string	
FormatPrice(double, VariableTick)
Formats price value to the appropriative string with a counting on tick precision.

Declaration
public string FormatPrice(double price, VariableTick variableTick)
Parameters
Type	Name	Description
double	price	
VariableTick	variableTick	
Returns
Type	Description
string	
FormatPriceWithMaxPrecision(double)
Formats price value to the appropriative string with a counting on max tick precision.

Declaration
public string FormatPriceWithMaxPrecision(double price)
Parameters
Type	Name	Description
double	price	
Returns
Type	Description
string	
FormatQuantity(double, bool, bool)
Represent access to symbol information and properties.

Declaration
public virtual string FormatQuantity(double quantity, bool inLots = true, bool abbreviate = false)
Parameters
Type	Name	Description
double	quantity	
bool	inLots	
bool	abbreviate	
Returns
Type	Description
string	
GetConnectionStateDependency()
Represent access to symbol information and properties.

Declaration
public virtual ConnectionDependency GetConnectionStateDependency()
Returns
Type	Description
ConnectionDependency	
GetHashCode()
Serves as the default hash function.

Declaration
public override int GetHashCode()
Returns
Type	Description
int	
A hash code for the current object.

Overrides
object.GetHashCode()
GetHistory(HistoryAggregation, DateTime, DateTime)
Gets historical data according to aggregation and other parameters

Declaration
public HistoricalData GetHistory(HistoryAggregation aggregation, DateTime fromTime, DateTime toTime = default)
Parameters
Type	Name	Description
HistoryAggregation	aggregation	
DateTime	fromTime	
DateTime	toTime	
Returns
Type	Description
HistoricalData	
GetHistory(HistoryRequestParameters)
Gets historical data according to given history request

Declaration
public HistoricalData GetHistory(HistoryRequestParameters historyRequestParameters)
Parameters
Type	Name	Description
HistoryRequestParameters	historyRequestParameters	
Returns
Type	Description
HistoricalData	
GetHistory(Period, DateTime, DateTime)
Gets historical data according to period and other parameters

Declaration
public HistoricalData GetHistory(Period period, DateTime fromTime, DateTime toTime = default)
Parameters
Type	Name	Description
Period	period	
DateTime	fromTime	
DateTime	toTime	
Returns
Type	Description
HistoricalData	
GetHistory(Period, HistoryType, DateTime, DateTime)
Gets historical data according to period and other parameters

Declaration
public HistoricalData GetHistory(Period period, HistoryType historyType, DateTime fromTime, DateTime toTime = default)
Parameters
Type	Name	Description
Period	period	
HistoryType	historyType	
DateTime	fromTime	
DateTime	toTime	
Returns
Type	Description
HistoricalData	
GetMarginInfo(OrderRequestParameters)
Represent access to symbol information and properties.

Declaration
public MarginInfo GetMarginInfo(OrderRequestParameters orderRequestParameters)
Parameters
Type	Name	Description
OrderRequestParameters	orderRequestParameters	
Returns
Type	Description
MarginInfo	
GetTickCost(double)
Gets symbol tick cost retrived from the VariableTick list by price

Declaration
public double GetTickCost(double price)
Parameters
Type	Name	Description
double	price	
Returns
Type	Description
double	
GetTickHistory(HistoryType, DateTime, DateTime)
Gets historical ticks data according to given parameters

Declaration
public HistoricalData GetTickHistory(HistoryType historyType, DateTime fromTime, DateTime toTime = default)
Parameters
Type	Name	Description
HistoryType	historyType	
DateTime	fromTime	
DateTime	toTime	
Returns
Type	Description
HistoricalData	
GetTickSize(double)
Gets cached symbol tick size or retrives it from the VariableTick list

Declaration
public double GetTickSize(double price)
Parameters
Type	Name	Description
double	price	
Returns
Type	Description
double	
IsTradingAllowed(Account)
Represent access to symbol information and properties.

Declaration
public virtual bool IsTradingAllowed(Account account)
Parameters
Type	Name	Description
Account	account	
Returns
Type	Description
bool	
OnConnectionStateChanged(Connection, ConnectionStateChangedEventArgs)
Represent access to symbol information and properties.

Declaration
public virtual void OnConnectionStateChanged(Connection connection, ConnectionStateChangedEventArgs e)
Parameters
Type	Name	Description
Connection	connection	
ConnectionStateChangedEventArgs	e	
RoundPriceToTickSize(double, double)
Returns rounded to TickSize price

Declaration
public double RoundPriceToTickSize(double price, double tickSize = NaN)
Parameters
Type	Name	Description
double	price	
double	tickSize	
Returns
Type	Description
double	
SubscribeAction(SubscribeQuoteType)
Represent access to symbol information and properties.

Declaration
protected virtual void SubscribeAction(SubscribeQuoteType type)
Parameters
Type	Name	Description
SubscribeQuoteType	type	
UnSubscribeAction(SubscribeQuoteType)
Represent access to symbol information and properties.

Declaration
protected virtual void UnSubscribeAction(SubscribeQuoteType type)
Parameters
Type	Name	Description
SubscribeQuoteType	type	
Events
NewDayBar
Will be triggered when new correctional quote is comming from the vendor.

Declaration
public event DayBarHandler NewDayBar
Event Type
Type	Description
DayBarHandler	
NewLast
Will be triggered when new trade quote is comming

Declaration
public event LastHandler NewLast
Event Type
Type	Description
LastHandler	
NewLevel2
Will be triggered when new Level2 quote is comming

Declaration
public event Level2Handler NewLevel2
Event Type
Type	Description
Level2Handler	
NewMark
Represent access to symbol information and properties.

Declaration
public event MarkHandler NewMark
Event Type
Type	Description
MarkHandler	
NewQuote
Will be triggered when new Level1 quote is comming

Declaration
public event QuoteHandler NewQuote
Event Type
Type	Description
QuoteHandler	
Updated
Will be triggered when symbol updated.

Declaration
public event SymbolUpdateHandler Updated
Event Type
Type	Description
SymbolUpdateHandler