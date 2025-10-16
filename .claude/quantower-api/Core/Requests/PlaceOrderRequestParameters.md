# PlaceOrderRequestParameters Class

Paste official Quantower documentation here.

**URL**: https://api.quantower.com/docs/TradingPlatform.BusinessLayer.PlaceOrderRequestParameters.html
Class PlaceOrderRequestParameters
Namespace: TradingPlatform.BusinessLayer
Syntax
public class PlaceOrderRequestParameters : OrderRequestParameters
Constructors
PlaceOrderRequestParameters()
Declaration
public PlaceOrderRequestParameters()
PlaceOrderRequestParameters(IOrder)
Declaration
public PlaceOrderRequestParameters(IOrder order)
Parameters
Type	Name	Description
IOrder	order	
PlaceOrderRequestParameters(OrderRequestParameters)
Declaration
public PlaceOrderRequestParameters(OrderRequestParameters original)
Parameters
Type	Name	Description
OrderRequestParameters	original	
Properties
Event
Declaration
public override string Event { get; }
Property Value
Type	Description
string	
Overrides
TradingRequestParameters.Event
Type
Declaration
public override RequestType Type { get; }
Property Value
Type	Description
RequestType	
Overrides
RequestParameters.Type
Methods
Clone()
Creates a new object that is a copy of the current instance.

Declaration
public override object Clone()
Returns
Type	Description
object	
A new object that is a copy of this instance.

Overrides
OrderRequestParameters.Clone()