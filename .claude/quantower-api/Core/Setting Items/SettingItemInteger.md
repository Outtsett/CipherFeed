# SettingItemInteger Class

Paste official Quantower documentation here.

**URL**: https://api.quantower.com/docs/TradingPlatform.BusinessLayer.SettingItemInteger.html
Class SettingItemInteger
Typecasts setting as NumericUpDown item

Namespace: TradingPlatform.BusinessLayer
Syntax
public sealed class SettingItemInteger : SettingItemNumber<int>
Constructors
SettingItemInteger()
Typecasts setting as NumericUpDown item

Declaration
public SettingItemInteger()
SettingItemInteger(string, int, int)
Typecasts setting as NumericUpDown item

Declaration
public SettingItemInteger(string name, int value, int sortIndex = 0)
Parameters
Type	Name	Description
string	name	
int	value	
int	sortIndex	
Properties
Type
Typecasts setting as NumericUpDown item

Declaration
public override SettingItemType Type { get; }
Property Value
Type	Description
SettingItemType	
Overrides
SettingItem.Type
