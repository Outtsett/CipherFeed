# SettingItemDouble Class

Paste official Quantower documentation here.

**URL**: https://api.quantower.com/docs/TradingPlatform.BusinessLayer.SettingItemDouble.html
Class SettingItemDouble
Typecasts setting as NumericUpDown item

Namespace: TradingPlatform.BusinessLayer
Syntax
public class SettingItemDouble : SettingItemNumber<double>
Constructors
SettingItemDouble()
Typecasts setting as NumericUpDown item

Declaration
public SettingItemDouble()
SettingItemDouble(string, double, int)
Typecasts setting as NumericUpDown item

Declaration
public SettingItemDouble(string name, double value, int sortIndex = 0)
Parameters
Type	Name	Description
string	name	
double	value	
int	sortIndex	
Properties
DecimalPlaces
Typecasts setting as NumericUpDown item

Declaration
[Bindable("decimalPlaces")]
public int DecimalPlaces { get; set; }
Property Value
Type	Description
int	
Type
Typecasts setting as NumericUpDown item

Declaration
public override SettingItemType Type { get; }
Property Value
Type	Description
SettingItemType	
Overrides
SettingItem.Type
Methods
Equals(SettingItem)
Indicates whether the current object is equal to another object of the same type.

Declaration
public override bool Equals(SettingItem other)
Parameters
Type	Name	Description
SettingItem	other	
An object to compare with this object.

Returns
Type	Description
bool	
true if the current object is equal to the other parameter; otherwise, false.

Overrides
SettingItemNumber<double>.Equals(SettingItem)
GetHashCode()
Serves as the default hash function.

Declaration
public override int GetHashCode()
Returns
Type	Description
int	
A hash code for the current object.

Overrides
SettingItemNumber<double>.GetHashCode()