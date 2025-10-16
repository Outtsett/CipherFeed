# HistoryItemLast Class

Paste official Quantower documentation here.

**URL**: https://api.quantower.com/docs/TradingPlatform.BusinessLayer.HistoryItemLast.html
Class HistoryItemLast
Represents historical data trade item

Namespace: TradingPlatform.BusinessLayer
Syntax
public sealed class HistoryItemLast : HistoryItem
Constructors
HistoryItemLast()
Creates HistoryItemLast instance

Declaration
public HistoryItemLast()
HistoryItemLast(Last)
Represents historical data trade item

Declaration
public HistoryItemLast(Last last)
Parameters
Type	Name	Description
Last	last	
Properties
AggressorFlag
Defines trade operation side as aggressor flag

Declaration
public AggressorFlag AggressorFlag { get; set; }
Property Value
Type	Description
AggressorFlag	
Buyer
Represents historical data trade item

Declaration
public string Buyer { get; set; }
Property Value
Type	Description
string	
FundingRate
Represents historical data trade item

Declaration
public double FundingRate { get; set; }
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
OpenInterest
Represents historical data trade item

Declaration
public double OpenInterest { get; set; }
Property Value
Type	Description
double	
Price
Defines price value

Declaration
public double Price { get; set; }
Property Value
Type	Description
double	
QuoteAssetVolume
Represents historical data trade item

Declaration
public double QuoteAssetVolume { get; set; }
Property Value
Type	Description
double	
Seller
Represents historical data trade item

Declaration
public string Seller { get; set; }
Property Value
Type	Description
string	
TickDirection
Represents historical data trade item

Declaration
public TickDirection TickDirection { get; set; }
Property Value
Type	Description
TickDirection	
Volume
Defines volume value

Declaration
public double Volume { get; set; }
Property Value
Type	Description
double