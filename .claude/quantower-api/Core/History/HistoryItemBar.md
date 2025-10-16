# HistoryItemBar Class

Paste official Quantower documentation here.

**URL**: https://api.quantower.com/docs/TradingPlatform.BusinessLayer.HistoryItemBar.html
Class HistoryItemBar
Represents historical data bar item

Namespace: TradingPlatform.BusinessLayer
Syntax
public class HistoryItemBar : HistoryItem
Constructors
HistoryItemBar()
Creates HistoryItemBar instance with default OHLC price = DOUBLE_UNDEFINED

Declaration
public HistoryItemBar()
Properties
Close
Defines Close price

Declaration
public double Close { get; set; }
Property Value
Type	Description
double	
FundingRate
Represents historical data bar item

Declaration
public double FundingRate { get; set; }
Property Value
Type	Description
double	
High
Defines High price

Declaration
public double High { get; set; }
Property Value
Type	Description
double	
this[PriceType]
Gets price by indexing PriceType

Declaration
public override double this[PriceType priceType] { get; }
Parameters
Type	Name	Description
PriceType	priceType	
Property Value
Type	Description
double	
Overrides
HistoryItem.this[PriceType]
Low
Defines Low price

Declaration
public double Low { get; set; }
Property Value
Type	Description
double	
Median
Gets Median (High+Low)/2 price

Declaration
public double Median { get; }
Property Value
Type	Description
double	
Open
Defines Open price

Declaration
public double Open { get; set; }
Property Value
Type	Description
double	
OpenInterest
Represents historical data bar item

Declaration
public double OpenInterest { get; set; }
Property Value
Type	Description
double	
QuoteAssetVolume
Represents historical data bar item

Declaration
public double QuoteAssetVolume { get; set; }
Property Value
Type	Description
double	
Ticks
Defines ticks amount

Declaration
public long Ticks { get; set; }
Property Value
Type	Description
long	
TicksRight
Defines bar's ticks count

Declaration
public override long TicksRight { get; set; }
Property Value
Type	Description
long	
Overrides
HistoryItem.TicksRight
TimeRight
Gets bar's right time border

Declaration
public DateTime TimeRight { get; }
Property Value
Type	Description
DateTime	
Typical
Gets Typical (High+Low+Close)/3 price

Declaration
public double Typical { get; }
Property Value
Type	Description
double	
Volume
Defines volume value

Declaration
public double Volume { get; set; }
Property Value
Type	Description
double	
Weighted
Gets Weighted (High+Low+Close+Close)/4 price

Declaration
public double Weighted { get; }
Property Value
Type	Description
double	
