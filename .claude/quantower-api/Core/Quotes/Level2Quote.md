# Level2Quote Class

Paste official Quantower documentation here.

**URL**: https://api.quantower.com/docs/TradingPlatform.BusinessLayer.Level2Quote.html
Class Level2Quote
Represent access to Level2 quote.

Namespace: TradingPlatform.BusinessLayer
Syntax
public class Level2Quote : MessageQuote
Properties
Broker
Broker identifier that send level2 quote

Declaration
public string Broker { get; set; }
Property Value
Type	Description
string	
Closed
Shows, whether Level2 quote is using only for removing from depth

Declaration
public bool Closed { get; set; }
Property Value
Type	Description
bool	
Id
Unique ID of Level2 quote

Declaration
public string Id { get; }
Property Value
Type	Description
string	
ImpliedSize
specifies the implied quantity associated with the price for the quote. Subtracting this amount from the Size yields the outright quantity for the price level. A value of zero indicates that the implied size is not available/defined or that it is actually zero.

Declaration
public double ImpliedSize { get; set; }
Property Value
Type	Description
double	
NumberOrders
Number orders of Level2 quote

Declaration
public int NumberOrders { get; set; }
Property Value
Type	Description
int	
Price
Price of Level2 quote

Declaration
public double Price { get; }
Property Value
Type	Description
double	
PriceType
Price type of Level2 quote: Bid or Ask

Declaration
public QuotePriceType PriceType { get; }
Property Value
Type	Description
QuotePriceType	
Priority
Represent access to Level2 quote.

Declaration
public long Priority { get; set; }
Property Value
Type	Description
long	
Size
Size of Level2 quote

Declaration
public double Size { get; }
Property Value
Type	Description
double	
