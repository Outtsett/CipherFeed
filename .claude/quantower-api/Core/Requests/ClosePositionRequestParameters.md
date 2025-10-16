# ClosePositionRequestParameters Class

Paste official Quantower documentation here.

**URL**: https://api.quantower.com/docs/TradingPlatform.BusinessLayer.ClosePositionRequestParameters.html
Class ClosePositionRequestParameters
Namespace: TradingPlatform.BusinessLayer
Syntax
public class ClosePositionRequestParameters : TradingRequestParameters
Constructors
ClosePositionRequestParameters()
Declaration
public ClosePositionRequestParameters()
ClosePositionRequestParameters(ClosePositionRequestParameters)
Declaration
public ClosePositionRequestParameters(ClosePositionRequestParameters origin)
Parameters
Type	Name	Description
ClosePositionRequestParameters	origin	
Properties
CloseQuantity
Declaration
public double CloseQuantity { get; set; }
Property Value
Type	Description
double	
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
Position
Declaration
public Position Position { get; set; }
Property Value
Type	Description
Position	
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