# CancelOrderRequestParameters Class

Paste official Quantower documentation here.

**URL**: https://api.quantower.com/docs/TradingPlatform.BusinessLayer.CancelOrderRequestParameters.html
Class CancelOrderRequestParameters
Namespace: TradingPlatform.BusinessLayer
Syntax
public sealed class CancelOrderRequestParameters : TradingRequestParameters
Constructors
CancelOrderRequestParameters()
Declaration
public CancelOrderRequestParameters()
CancelOrderRequestParameters(CancelOrderRequestParameters)
Declaration
public CancelOrderRequestParameters(CancelOrderRequestParameters original)
Parameters
Type	Name	Description
CancelOrderRequestParameters	original	
Properties
ConnectionId
Declaration
public override string ConnectionId { get; }
Property Value
Type	Description
string	
Overrides
TradingRequestParameters.ConnectionId
Event
Declaration
public override string Event { get; }
Property Value
Type	Description
string	
Overrides
TradingRequestParameters.Event
Message
Declaration
public override string Message { get; }
Property Value
Type	Description
string	
Overrides
TradingRequestParameters.Message
Order
Declaration
public IOrder Order { get; set; }
Property Value
Type	Description
IOrder	
OrderId
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
GetAccount()
Declaration
protected override Account GetAccount()
Returns
Type	Description
Account	
Overrides
TradingRequestParameters.GetAccount()
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