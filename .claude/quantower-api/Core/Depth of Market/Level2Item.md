# Level2Item Class

Paste official Quantower documentation here.

**URL**: https://api.quantower.com/docs/TradingPlatform.BusinessLayer.Level2Item.html
Class Level2Item
Represent access to level2 item.

Namespace: TradingPlatform.BusinessLayer
Syntax
public class Level2Item
Properties
Cumulative
Cumulative size

Declaration
public double Cumulative { get; }
Property Value
Type	Description
double	
CumulativeCount
Cumulative orders count

Declaration
public double CumulativeCount { get; }
Property Value
Type	Description
double	
CumulativeMoney
Cumulative money

Declaration
public double CumulativeMoney { get; }
Property Value
Type	Description
double	
DetailedLevels
Represent access to level2 item.

Declaration
public Level2Item[] DetailedLevels { get; set; }
Property Value
Type	Description
Level2Item[]	
Id
Represent access to level2 item.

Declaration
public string Id { get; }
Property Value
Type	Description
string	
ImbalancePercent
Imbalance Percent

Declaration
public double ImbalancePercent { get; }
Property Value
Type	Description
double	
MMID
MMID

Declaration
public string MMID { get; }
Property Value
Type	Description
string	
NumberOrders
Number orders

Declaration
public int NumberOrders { get; }
Property Value
Type	Description
int	
Price
Price

Declaration
public double Price { get; }
Property Value
Type	Description
double	
Priority
Represent access to level2 item.

Declaration
public long Priority { get; }
Property Value
Type	Description
long	
QuoteTime
Time

Declaration
public DateTime QuoteTime { get; }
Property Value
Type	Description
DateTime	
Size
Size

Declaration
public double Size { get; }
Property Value
Type	Description
double	
Methods
ToString()
Returns a string that represents the current object.

Declaration
public override string ToString()
Returns
Type	Description
string	
A string that represents the current object.

Overrides
object.ToString()
