# SettingItemColor Class

Paste official Quantower documentation here.

**URL**: https://api.quantower.com/docs/TradingPlatform.BusinessLayer.SettingItemColor.html
Class SettingItemColor
Typecasts setting as Color item

Namespace: TradingPlatform.BusinessLayer
Syntax
public sealed class SettingItemColor : SettingItem
Constructors
SettingItemColor()
Typecasts setting as Color item

Declaration
public SettingItemColor()
SettingItemColor(string, Color, int)
Typecasts setting as Color item

Declaration
public SettingItemColor(string name, Color value, int sortIndex = 0)
Parameters
Type	Name	Description
string	name	
Color	value	
int	sortIndex	
Properties
AllowDisableColor
Typecasts setting as Color item

Declaration
public bool AllowDisableColor { get; set; }
Property Value
Type	Description
bool	
Checked
Typecasts setting as Color item

Declaration
public bool Checked { get; set; }
Property Value
Type	Description
bool	
ColorText
Typecasts setting as Color item

Declaration
public string ColorText { get; set; }
Property Value
Type	Description
string	
Type
Typecasts setting as Color item

Declaration
public override SettingItemType Type { get; }
Property Value
Type	Description
SettingItemType	
Overrides
SettingItem.Type
WithCheckBox
Typecasts setting as Color item

Declaration
public bool WithCheckBox { get; set; }
Property Value
Type	Description
bool