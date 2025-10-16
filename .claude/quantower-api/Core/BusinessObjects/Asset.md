# Asset Class

Paste official Quantower documentation here.

**URL**: https://api.quantower.com/docs/TradingPlatform.BusinessLayer.Asset.html
Class Asset
Defines asset entity

Namespace: TradingPlatform.BusinessLayer
Syntax
public class Asset : BusinessObject
Properties
Description
Asset description

Declaration
public string Description { get; }
Property Value
Type	Description
string	
Id
Asset id bearer

Declaration
public string Id { get; }
Property Value
Type	Description
string	
IsoCode
Gets asset ISO 4217 code

Declaration
public string IsoCode { get; }
Property Value
Type	Description
string	
MinimumChange
Defines a number precision of the change value

Declaration
public double MinimumChange { get; set; }
Property Value
Type	Description
double	
Name
Asset name bearer

Declaration
public string Name { get; }
Property Value
Type	Description
string	
Precision
Gets precision value

Declaration
public int Precision { get; }
Property Value
Type	Description
int	
Methods
FormatPrice(double)
Formats price into precision normalized string

Declaration
public string FormatPrice(double price)
Parameters
Type	Name	Description
double	price	
Returns
Type	Description
string	
FormatPriceWithCurrency(double, bool)
Formats price into concatenated string which contains the precision normalized value and Asset's name (optional)

Declaration
public string FormatPriceWithCurrency(double price, bool withAssetName = true)
Parameters
Type	Name	Description
double	price	
bool	withAssetName	
Returns
Type	Description
string	
FormatWithCurrency(double)
Defines asset entity

Declaration
public string FormatWithCurrency(double value)
Parameters
Type	Name	Description
double	value	
Returns
Type	Description
string	