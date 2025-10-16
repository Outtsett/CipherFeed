# SettingItemSelector Class

Paste official Quantower documentation here.

**URL**: https://api.quantower.com/docs/TradingPlatform.BusinessLayer.SettingItemSelector.html
Class SettingItemSelector
Typecasts setting as ComboBox item

Namespace: TradingPlatform.BusinessLayer
Syntax
public sealed class SettingItemSelector : SettingItem
Constructors
SettingItemSelector()
Typecasts setting as ComboBox item

Declaration
public SettingItemSelector()
SettingItemSelector(string, string, IEnumerable<string>, int)
Typecasts setting as ComboBox item

Declaration
public SettingItemSelector(string name, string value, IEnumerable<string> items, int sortIndex = 0)
Parameters
Type	Name	Description
string	name	
string	value	
IEnumerable<string>	items	
int	sortIndex	
Properties
Items
Typecasts setting as ComboBox item

Declaration
[Bindable("items")]
public IEnumerable<string> Items { get; set; }
Property Value
Type	Description
IEnumerable<string>	
Type
Typecasts setting as ComboBox item

Declaration
public override SettingItemType Type { get; }
Property Value
Type	Description
SettingItemType	
Overrides
SettingItem.Type
