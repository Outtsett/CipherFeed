# Trade Class

Paste official Quantower documentation here.

**URL**: https://api.quantower.com/docs/TradingPlatform.BusinessLayer.Trade.html
Class Trade
Represents information about trade.

Namespace: TradingPlatform.BusinessLayer
Syntax
public class Trade : TradingObject
Constructors
Trade(string)
Represents information about trade.

Declaration
public Trade(string connectionId)
Parameters
Type	Name	Description
string	connectionId	
Properties
DateTime
Get the date and time when trade was executed

Declaration
public DateTime DateTime { get; }
Property Value
Type	Description
DateTime	
Fee
Get the fee value that was charged for this trade

Declaration
public PnLItem Fee { get; }
Property Value
Type	Description
PnLItem	
GrossPnl
Get the trade Gross P&L

Declaration
public PnLItem GrossPnl { get; }
Property Value
Type	Description
PnLItem	
NetPnl
Get the trade Net P&L

Declaration
public PnLItem NetPnl { get; }
Property Value
Type	Description
PnLItem	
OrderId
Gets the unique identifier of the order initiating the trade.

Declaration
public string OrderId { get; }
Property Value
Type	Description
string	
OrderTypeId
Get the trade order type

Declaration
public string OrderTypeId { get; }
Property Value
Type	Description
string	
PositionId
Gets a unique identifier of the position, which is related to this trade.

Declaration
public string PositionId { get; }
Property Value
Type	Description
string	
PositionImpactType
Represents information about trade.

Declaration
public PositionImpactType PositionImpactType { get; }
Property Value
Type	Description
PositionImpactType	
Price
Get the price where trade was executed

Declaration
public double Price { get; }
Property Value
Type	Description
double	
Quantity
Get the trade quantity

Declaration
public double Quantity { get; }
Property Value
Type	Description
double	
Methods
BuildMessage()
Represents information about trade.

Declaration
public MessageTrade BuildMessage()
Returns
Type	Description
MessageTrade	
ToString()
Returns a string that represents the current object.

Declaration
public override string ToString()
Returns
Type	Description
string	
A string that represents the current object.

Overrides
object.ToString()
Events
Updated
Will be triggered on trade updating

Declaration
public event Action Updated
Event Type
Type	Description
Action	
