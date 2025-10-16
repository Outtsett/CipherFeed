# TradingRequestParameters Class

Paste official Quantower documentation here.

**URL**: https://api.quantower.com/docs/TradingPlatform.BusinessLayer.TradingRequestParameters.html
Class TradingRequestParameters
Namespace: TradingPlatform.BusinessLayer
Syntax
public abstract class TradingRequestParameters : RequestParameters
Constructors
TradingRequestParameters()
Declaration
public TradingRequestParameters()
TradingRequestParameters(TradingRequestParameters)
Declaration
public TradingRequestParameters(TradingRequestParameters origin)
Parameters
Type	Name	Description
TradingRequestParameters	origin	
Properties
ConnectionId
Declaration
public abstract string ConnectionId { get; }
Property Value
Type	Description
string	
Event
Declaration
public abstract string Event { get; }
Property Value
Type	Description
string	
Message
Declaration
public abstract string Message { get; }
Property Value
Type	Description
string	
ParentOperation
Declaration
public GroupTradingOperation ParentOperation { get; init; }
Property Value
Type	Description
GroupTradingOperation	
Methods
GetAccount()
Declaration
protected abstract Account GetAccount()
Returns
Type	Description
Account	