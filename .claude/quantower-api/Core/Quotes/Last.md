# Last Class

Paste official Quantower documentation here.

**URL**: https://api.quantower.com/docs/TradingPlatform.BusinessLayer.Last.html
Class Last
Represent access to trade information.

Namespace: TradingPlatform.BusinessLayer
Syntax
public class Last : MessageQuote
Properties
AggressorFlag
Information about operation side of the trade

Declaration
public AggressorFlag AggressorFlag { get; set; }
Property Value
Type	Description
AggressorFlag	
Buyer
Represent access to trade information.

Declaration
public string Buyer { get; set; }
Property Value
Type	Description
string	
OpenInterest
Represent access to trade information.

Declaration
public double OpenInterest { get; set; }
Property Value
Type	Description
double	
Price
Price at which trade occured

Declaration
public double Price { get; }
Property Value
Type	Description
double	
QuoteAssetVolume
Represent access to trade information.

Declaration
public double QuoteAssetVolume { get; set; }
Property Value
Type	Description
double	
Seller
Represent access to trade information.

Declaration
public string Seller { get; set; }
Property Value
Type	Description
string	
Size
Size of the trade

Declaration
public double Size { get; set; }
Property Value
Type	Description
double	
TickDirection
Shows the direction of price movement, comparing to previous value.

Declaration
public TickDirection TickDirection { get; set; }
Property Value
Type	Description
TickDirection	
TradeId
Represent access to trade information.

Declaration
public string TradeId { get; set; }
Property Value
Type	Description
string	