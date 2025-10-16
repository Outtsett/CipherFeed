# GetLevel2ItemsParameters Class

Paste official Quantower documentation here.

**URL**: https://api.quantower.com/docs/TradingPlatform.BusinessLayer.GetLevel2ItemsParameters.html
Class GetLevel2ItemsParameters
Represent parameters of request for Leve2Item collection

Namespace: TradingPlatform.BusinessLayer
Syntax
public class GetLevel2ItemsParameters
Properties
AggregateMethod
Aggregation method

Declaration
public AggregateMethod AggregateMethod { get; set; }
Property Value
Type	Description
AggregateMethod	
CalculateCumulative
Calculate cumulative size

Declaration
public bool CalculateCumulative { get; set; }
Property Value
Type	Description
bool	
CustomTickSize
Use custom tick size

Declaration
public double CustomTickSize { get; set; }
Property Value
Type	Description
double	
GetMBOItems
Represent parameters of request for Leve2Item collection

Declaration
public bool GetMBOItems { get; set; }
Property Value
Type	Description
bool	
ImplicitOrderBookType
Represent parameters of request for Leve2Item collection

Declaration
public ImplicitOrderBookType ImplicitOrderBookType { get; set; }
Property Value
Type	Description
ImplicitOrderBookType	
LevelsCount
Required amount of level2

Declaration
public int LevelsCount { get; set; }
Property Value
Type	Description
int	
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
operator ==(GetLevel2ItemsParameters, GetLevel2ItemsParameters)
Represent parameters of request for Leve2Item collection

Declaration
public static bool operator ==(GetLevel2ItemsParameters p1, GetLevel2ItemsParameters p2)
Parameters
Type	Name	Description
GetLevel2ItemsParameters	p1	
GetLevel2ItemsParameters	p2	
Returns
Type	Description
bool	
operator !=(GetLevel2ItemsParameters, GetLevel2ItemsParameters)
Represent parameters of request for Leve2Item collection

Declaration
public static bool operator !=(GetLevel2ItemsParameters p1, GetLevel2ItemsParameters p2)
Parameters
Type	Name	Description
GetLevel2ItemsParameters	p1	
GetLevel2ItemsParameters	p2	
Returns
Type	Description
bool	