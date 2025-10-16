# ModifyOrderRequestParameters Class

Paste official Quantower documentation here.

**URL**: https://api.quantower.com/docs/TradingPlatform.BusinessLayer.ModifyOrderRequestParameters.html
Class ModifyOrderRequestParameters
Namespace: TradingPlatform.BusinessLayer
Syntax
public class ModifyOrderRequestParameters : OrderRequestParameters
Constructors
ModifyOrderRequestParameters(IOrder)
Declaration
public ModifyOrderRequestParameters(IOrder order)
Parameters
Type	Name	Description
IOrder	order	
ModifyOrderRequestParameters(ModifyOrderRequestParameters)
Declaration
public ModifyOrderRequestParameters(ModifyOrderRequestParameters original)
Parameters
Type	Name	Description
ModifyOrderRequestParameters	original	
Properties
Event
Declaration
public override string Event { get; }
Property Value
Type	Description
string	
Overrides
TradingRequestParameters.Event
OrderId
Id of the order

Declaration
public string OrderId { get; set; }
Property Value
Type	Description
string	
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
ToString()
Returns a string that represents the current object.

Declaration
public override string ToString()
Returns
Type	Description
string	
A string that represents the current object.

Overrides
OrderRequestParameters.ToString()