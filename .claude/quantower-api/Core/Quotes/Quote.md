# Quote Class

Paste official Quantower documentation here.

**URL**: https://api.quantower.com/docs/TradingPlatform.BusinessLayer.Quote.html
Class Quote
Represent access to quote information.

Namespace: TradingPlatform.BusinessLayer
Syntax
public class Quote : MessageQuote
Properties
Ask
Ask price

Declaration
public double Ask { get; set; }
Property Value
Type	Description
double	
AskSize
Ask size

Declaration
public double AskSize { get; set; }
Property Value
Type	Description
double	
AskTickDirection
Shows the direction of ask price movement, comparing to previous value.

Declaration
public TickDirection AskTickDirection { get; }
Property Value
Type	Description
TickDirection	
Bid
Bid price

Declaration
public double Bid { get; set; }
Property Value
Type	Description
double	
BidSize
Bid size

Declaration
public double BidSize { get; set; }
Property Value
Type	Description
double	
BidTickDirection
Shows the direction of bid price movement, comparing to previous value.

Declaration
public TickDirection BidTickDirection { get; }
Property Value
Type	Description
TickDirection	
