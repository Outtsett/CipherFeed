# GetDepthOfMarketParameters Class

Paste official Quantower documentation here.

**URL**: https://api.quantower.com/docs/TradingPlatform.BusinessLayer.GetDepthOfMarketParameters.html
Class GetDepthOfMarketParameters
Represent parameters of DepthOfMarket

Namespace: TradingPlatform.BusinessLayer
Syntax
public class GetDepthOfMarketParameters
Constructors
GetDepthOfMarketParameters()
Represent parameters of DepthOfMarket

Declaration
public GetDepthOfMarketParameters()
Properties
CalculateImbalancePercent
Represent parameters of DepthOfMarket

Declaration
public bool CalculateImbalancePercent { get; set; }
Property Value
Type	Description
bool	
GetLevel2ItemsParameters
Represent parameters of DepthOfMarket

Declaration
public GetLevel2ItemsParameters GetLevel2ItemsParameters { get; set; }
Property Value
Type	Description
GetLevel2ItemsParameters	
Methods
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
Operators
operator ==(GetDepthOfMarketParameters, GetDepthOfMarketParameters)
Represent parameters of DepthOfMarket

Declaration
public static bool operator ==(GetDepthOfMarketParameters p1, GetDepthOfMarketParameters p2)
Parameters
Type	Name	Description
GetDepthOfMarketParameters	p1	
GetDepthOfMarketParameters	p2	
Returns
Type	Description
bool	
operator !=(GetDepthOfMarketParameters, GetDepthOfMarketParameters)
Represent parameters of DepthOfMarket

Declaration
public static bool operator !=(GetDepthOfMarketParameters p1, GetDepthOfMarketParameters p2)
Parameters
Type	Name	Description
GetDepthOfMarketParameters	p1	
GetDepthOfMarketParameters	p2	
Returns
Type	Description
bool	