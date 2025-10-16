# SettingItemString Class

Paste official Quantower documentation here.

**URL**: https://api.quantower.com/docs/TradingPlatform.BusinessLayer.SettingItemString.html
Class SettingItemString
Typecasts setting as TextBox item

Namespace: TradingPlatform.BusinessLayer
Syntax
public sealed class SettingItemString : SettingItem
Constructors
SettingItemString()
Typecasts setting as TextBox item

Declaration
public SettingItemString()
SettingItemString(string, string, int)
Typecasts setting as TextBox item

Declaration
public SettingItemString(string name, string value, int sortIndex = 0)
Parameters
Type	Name	Description
string	name	
string	value	
int	sortIndex	
Properties
ApplyOnEachInput
Typecasts setting as TextBox item

Declaration
public bool ApplyOnEachInput { get; set; }
Property Value
Type	Description
bool	
Type
Typecasts setting as TextBox item

Declaration
public override SettingItemType Type { get; }
Property Value
Type	Description
SettingItemType	
Overrides
SettingItem.Type