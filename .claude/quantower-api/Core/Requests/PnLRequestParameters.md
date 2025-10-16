# PnLRequestParameters Class

Paste official Quantower documentation here.

**URL**: https://api.quantower.com/docs/TradingPlatform.BusinessLayer.PnLRequestParameters.html
Class PnLRequestParameters
Namespace: TradingPlatform.BusinessLayer
Syntax
public class PnLRequestParameters : RequestParameters
Constructors
PnLRequestParameters()
Declaration
public PnLRequestParameters()
PnLRequestParameters(PnLRequestParameters)
Declaration
public PnLRequestParameters(PnLRequestParameters original)
Parameters
Type	Name	Description
PnLRequestParameters	original	
Properties
Account
Declaration
public Account Account { get; set; }
Property Value
Type	Description
Account	
ClosePrice
Declaration
public double ClosePrice { get; set; }
Property Value
Type	Description
double	
OpenPrice
Declaration
public double OpenPrice { get; set; }
Property Value
Type	Description
double	
PositionId
Declaration
public string PositionId { get; set; }
Property Value
Type	Description
string	
Quantity
Declaration
public double Quantity { get; set; }
Property Value
Type	Description
double	
Side
Declaration
public Side Side { get; set; }
Property Value
Type	Description
Side	
Symbol
Declaration
public Symbol Symbol { get; set; }
Property Value
Type	Description
Symbol	
Type
Declaration
public override RequestType Type { get; }
Property Value
Type	Description
RequestType	
Overrides
RequestParameters.Type