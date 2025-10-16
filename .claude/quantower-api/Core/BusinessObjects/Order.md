# Order Class

Paste official Quantower documentation here.

**URL**: https://api.quantower.com/docs/TradingPlatform.BusinessLayer.Order.html
Class Order
Represents trading information about pending order

Namespace: TradingPlatform.BusinessLayer
Syntax
public class Order : TradingObject
Properties
AverageFillPrice
Represents trading information about pending order

Declaration
public double AverageFillPrice { get; }
Property Value
Type	Description
double	
ExpirationTime
Gets orders expiration time

Declaration
public DateTime ExpirationTime { get; }
Property Value
Type	Description
DateTime	
FilledQuantity
Filled quantity of the order

Declaration
public double FilledQuantity { get; }
Property Value
Type	Description
double	
GroupId
The ID of the order group. This group created when trades done by the MAM account.

Declaration
public string GroupId { get; }
Property Value
Type	Description
string	
LastUpdateTime
Gets orders last update time

Declaration
public DateTime LastUpdateTime { get; }
Property Value
Type	Description
DateTime	
OrderType
Gets OrderType

Declaration
public OrderType OrderType { get; }
Property Value
Type	Description
OrderType	
OrderTypeId
Orders Type Id. It is used for the orders type comparing.

Declaration
public string OrderTypeId { get; }
Property Value
Type	Description
string	
OriginalStatus
Gets open order original status

Declaration
public string OriginalStatus { get; }
Property Value
Type	Description
string	
PositionId
Gets Position Id.

Declaration
public string PositionId { get; }
Property Value
Type	Description
string	
Price
Gets order price value

Declaration
public double Price { get; }
Property Value
Type	Description
double	
RemainingQuantity
Remaining quantity of the order

Declaration
public double RemainingQuantity { get; }
Property Value
Type	Description
double	
Status
Gets orders current status

Declaration
public OrderStatus Status { get; }
Property Value
Type	Description
OrderStatus	
StopLoss
Gets StopLoss holder for given order

Declaration
public SlTpHolder StopLoss { get; }
Property Value
Type	Description
SlTpHolder	
StopLossItems
Represents trading information about pending order

Declaration
public SlTpHolder[] StopLossItems { get; }
Property Value
Type	Description
SlTpHolder[]	
TakeProfit
Gets TakeProfit holder for given order

Declaration
public SlTpHolder TakeProfit { get; }
Property Value
Type	Description
SlTpHolder	
TakeProfitItems
Represents trading information about pending order

Declaration
public SlTpHolder[] TakeProfitItems { get; }
Property Value
Type	Description
SlTpHolder[]	
TimeInForce
Gets order TIF(Time-In-Force) type

Declaration
public TimeInForce TimeInForce { get; }
Property Value
Type	Description
TimeInForce	
TotalQuantity
Total quantity of the order

Declaration
public double TotalQuantity { get; }
Property Value
Type	Description
double	
TrailOffset
Gets order trailing offset value

Declaration
public double TrailOffset { get; }
Property Value
Type	Description
double	
TriggerPrice
Gets order trigger price value

Declaration
public double TriggerPrice { get; }
Property Value
Type	Description
double	
Methods
BuildMessage()
Represents trading information about pending order

Declaration
public MessageOpenOrder BuildMessage()
Returns
Type	Description
MessageOpenOrder	
Cancel(string)
Cancels pending order

Declaration
public TradingOperationResult Cancel(string sendingSource = null)
Parameters
Type	Name	Description
string	sendingSource	
Returns
Type	Description
TradingOperationResult	
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
Equals(Order)
Indicates whether the current object is equal to another object of the same type.

Declaration
public bool Equals(Order other)
Parameters
Type	Name	Description
Order	other	
An object to compare with this object.

Returns
Type	Description
bool	
true if the current object is equal to the other parameter; otherwise, false.

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
Will be triggered on each UpdateByMessage(MessageOpenOrder) invocation

Declaration
public event Action<IOrder> Updated
Event Type
Type	Description
Action<IOrder>