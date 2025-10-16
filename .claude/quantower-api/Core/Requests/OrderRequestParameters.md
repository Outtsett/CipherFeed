# OrderRequestParameters Class

Paste official Quantower documentation here.

**URL**: https://api.quantower.com/docs/TradingPlatform.BusinessLayer.OrderRequestParameters.html
Class OrderRequestParameters
Namespace: TradingPlatform.BusinessLayer
Syntax
public abstract class OrderRequestParameters : TradingRequestParameters
Constructors
OrderRequestParameters()
Declaration
protected OrderRequestParameters()
OrderRequestParameters(IOrder)
Declaration
protected OrderRequestParameters(IOrder order)
Parameters
Type	Name	Description
IOrder	order	
OrderRequestParameters(OrderRequestParameters)
Declaration
protected OrderRequestParameters(OrderRequestParameters origin)
Parameters
Type	Name	Description
OrderRequestParameters	origin	
Properties
Account
Declaration
public Account Account { get; set; }
Property Value
Type	Description
Account	
AccountId
Declaration
public string AccountId { get; set; }
Property Value
Type	Description
string	
AdditionalParameters
Declaration
public IList<SettingItem> AdditionalParameters { get; set; }
Property Value
Type	Description
IList<SettingItem>	
Comment
Declaration
public string Comment { get; set; }
Property Value
Type	Description
string	
ConnectionId
Declaration
public override string ConnectionId { get; }
Property Value
Type	Description
string	
Overrides
TradingRequestParameters.ConnectionId
ExpirationTime
Declaration
public DateTime ExpirationTime { get; set; }
Property Value
Type	Description
DateTime	
GroupId
Declaration
public string GroupId { get; set; }
Property Value
Type	Description
string	
Message
Declaration
public override string Message { get; }
Property Value
Type	Description
string	
Overrides
TradingRequestParameters.Message
OrderType
Declaration
public OrderType OrderType { get; }
Property Value
Type	Description
OrderType	
OrderTypeId
Declaration
public string OrderTypeId { get; set; }
Property Value
Type	Description
string	
PositionId
Declaration
public string PositionId { get; set; }
Property Value
Type	Description
string	
Price
Declaration
public double Price { get; set; }
Property Value
Type	Description
double	
Quantity
Declaration
public double Quantity { get; set; }
Property Value
Type	Description
double	
QuantityDefinitionSettingName
Declaration
public string QuantityDefinitionSettingName { get; set; }
Property Value
Type	Description
string	
Side
Declaration
public Side Side { get; set; }
Property Value
Type	Description
Side	
Slippage
Declaration
public int Slippage { get; set; }
Property Value
Type	Description
int	
StopLoss
Declaration
public SlTpHolder StopLoss { get; set; }
Property Value
Type	Description
SlTpHolder	
StopLossItems
Declaration
public List<SlTpHolder> StopLossItems { get; }
Property Value
Type	Description
List<SlTpHolder>	
Symbol
Declaration
public Symbol Symbol { get; set; }
Property Value
Type	Description
Symbol	
SymbolId
Declaration
public string SymbolId { get; set; }
Property Value
Type	Description
string	
TakeProfit
Declaration
public SlTpHolder TakeProfit { get; set; }
Property Value
Type	Description
SlTpHolder	
TakeProfitItems
Declaration
public List<SlTpHolder> TakeProfitItems { get; }
Property Value
Type	Description
List<SlTpHolder>	
TimeInForce
Declaration
public TimeInForce TimeInForce { get; set; }
Property Value
Type	Description
TimeInForce	
Total
Declaration
public double Total { get; set; }
Property Value
Type	Description
double	
TrailOffset
Declaration
public double TrailOffset { get; set; }
Property Value
Type	Description
double	
TriggerPrice
Declaration
public double TriggerPrice { get; set; }
Property Value
Type	Description
double	
Methods
ApplyValuesFrom(OrderRequestParameters)
Declaration
public void ApplyValuesFrom(OrderRequestParameters other)
Parameters
Type	Name	Description
OrderRequestParameters	other	
Clone()
Creates a new object that is a copy of the current instance.

Declaration
public abstract object Clone()
Returns
Type	Description
object	
A new object that is a copy of this instance.

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
RequestParameters.Equals(object)
Equals(OrderRequestParameters)
Indicates whether the current object is equal to another object of the same type.

Declaration
public bool Equals(OrderRequestParameters other)
Parameters
Type	Name	Description
OrderRequestParameters	other	
An object to compare with this object.

Returns
Type	Description
bool	
true if the current object is equal to the other parameter; otherwise, false.

FromXElement(XElement, DeserializationInfo)
Declaration
public void FromXElement(XElement element, DeserializationInfo deserializationInfo)
Parameters
Type	Name	Description
XElement	element	
DeserializationInfo	deserializationInfo	
GetAccount()
Declaration
protected override Account GetAccount()
Returns
Type	Description
Account	
Overrides
TradingRequestParameters.GetAccount()
GetHashCode()
Serves as the default hash function.

Declaration
public override int GetHashCode()
Returns
Type	Description
int	
A hash code for the current object.

Overrides
RequestParameters.GetHashCode()
ToString()
Returns a string that represents the current object.

Declaration
public override string ToString()
Returns
Type	Description
string	
A string that represents the current object.

Overrides
RequestParameters.ToString()
ToXElement()
Declaration
public XElement ToXElement()
Returns
Type	Description
XElement	
UpdateFrom(OrderRequestParameters)
Declaration
public void UpdateFrom(OrderRequestParameters origin)
Parameters
Type	Name	Description
OrderRequestParameters	origin