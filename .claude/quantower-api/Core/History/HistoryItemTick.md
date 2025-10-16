# HistoryItemTick Class

Paste official Quantower documentation here.

**URL**: https://api.quantower.com/docs/TradingPlatform.BusinessLayer.HistoryItemTick.html
Class HistoryItemTick
Represents historical data tick item

Namespace: TradingPlatform.BusinessLayer
Syntax
public class HistoryItemTick : HistoryItem
Constructors
HistoryItemTick()
Creates HistoryItemBar instance with default Ask/AskSize/Bid/BidSize = DOUBLE_UNDEFINED

Declaration
public HistoryItemTick()
Properties
Ask
Defines Ask price

Declaration
public double Ask { get; set; }
Property Value
Type	Description
double	
AskSize
Defines Ask size

Declaration
public double AskSize { get; set; }
Property Value
Type	Description
double	
AskTickDirection
Represents historical data tick item

Declaration
public TickDirection AskTickDirection { get; set; }
Property Value
Type	Description
TickDirection	
Bid
Defines Bid price

Declaration
public double Bid { get; set; }
Property Value
Type	Description
double	
BidSize
Defines Bid size

Declaration
public double BidSize { get; set; }
Property Value
Type	Description
double	
BidTickDirection
Represents historical data tick item

Declaration
public TickDirection BidTickDirection { get; set; }
Property Value
Type	Description
TickDirection	
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
