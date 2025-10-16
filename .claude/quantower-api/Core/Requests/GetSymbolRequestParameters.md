# GetSymbolRequestParameters Class

Paste official Quantower documentation here.

**URL**: https://api.quantower.com/docs/TradingPlatform.BusinessLayer.GetSymbolRequestParameters.html
Class GetSymbolRequestParameters
Namespace: TradingPlatform.BusinessLayer
Syntax
public class GetSymbolRequestParameters : CachedRequestParameters
Constructors
GetSymbolRequestParameters()
Declaration
public GetSymbolRequestParameters()
GetSymbolRequestParameters(GetSymbolRequestParameters)
Declaration
public GetSymbolRequestParameters(GetSymbolRequestParameters origin)
Parameters
Type	Name	Description
GetSymbolRequestParameters	origin	
Properties
SymbolId
Declaration
public string SymbolId { get; set; }
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
GetCacheKey()
Declaration
public override int GetCacheKey()
Returns
Type	Description
int	
Overrides
CachedRequestParameters.GetCacheKey()