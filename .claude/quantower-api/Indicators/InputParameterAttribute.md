# InputParameterAttribute Class

Paste official Quantower documentation here.

**URL**: https://api.quantower.com/docs/TradingPlatform.BusinessLayer.InputParameterAttribute.html
Class InputParameterAttribute
Use this attribute to mark input parameters of your script. You will see them in the settings screen on adding

Namespace: TradingPlatform.BusinessLayer
Syntax
public class InputParameterAttribute : Attribute
Constructors
InputParameterAttribute(string, int, double, double, double, int, object[])
Use this attribute to mark input parameters of your script. You will see them in the settings screen on adding

Declaration
public InputParameterAttribute(string name = "", int sortIndex = 0, double minimum = -2147483648, double maximum = 2147483647, double increment = 0.01, int decimalPlaces = 2, object[] variants = null)
Parameters
Type	Name	Description
string	name	
int	sortIndex	
double	minimum	
double	maximum	
double	increment	
int	decimalPlaces	
object[]	variants	
Properties
DecimalPlaces
Decimal palces for numeric input parameters

Declaration
public int DecimalPlaces { get; }
Property Value
Type	Description
int	
Increment
Increment value for numeric input parameters

Declaration
public double Increment { get; }
Property Value
Type	Description
double	
Maximum
Maximal value for numeric input parameters

Declaration
public double Maximum { get; }
Property Value
Type	Description
double	
Minimum
Minimal value for numeric input parameters

Declaration
public double Minimum { get; }
Property Value
Type	Description
double	
Name
Displayed name of input parameter

Declaration
public string Name { get; }
Property Value
Type	Description
string	
SortIndex
Sort index for input paramter

Declaration
public int SortIndex { get; }
Property Value
Type	Description
int	
Variants
List of predefined values

Declaration
public IComparable[] Variants { get; }
Property Value
Type	Description
IComparable[]	