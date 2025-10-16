# SettingItemBoolean Class

Paste official Quantower documentation here.

**URL**: https://api.quantower.com/docs/TradingPlatform.BusinessLayer.SettingItemBoolean.html
Class SettingItemBoolean
Typecasts setting as CheckBox item

Namespace: TradingPlatform.BusinessLayer
Syntax
public sealed class SettingItemBoolean : SettingItem
Constructors
SettingItemBoolean()
Typecasts setting as CheckBox item

Declaration
public SettingItemBoolean()
SettingItemBoolean(string, bool, int)
Typecasts setting as CheckBox item

Declaration
public SettingItemBoolean(string name, bool value, int sortIndex = 0)
Parameters
Type	Name	Description
string	name	
bool	value	
int	sortIndex	
Properties
Type
Typecasts setting as CheckBox item

Declaration
public override SettingItemType Type { get; }
Property Value
Type	Description
SettingItemType	
Overrides
SettingItem.Type