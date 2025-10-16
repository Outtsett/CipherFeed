# SettingItemAction Class

Paste official Quantower documentation here.

**URL**: https://api.quantower.com/docs/TradingPlatform.BusinessLayer.SettingItemAction.html
Class SettingItemAction
Typecasts setting as Button item

Namespace: TradingPlatform.BusinessLayer
Syntax
public sealed class SettingItemAction : SettingItem
Constructors
SettingItemAction()
Typecasts setting as Button item

Declaration
public SettingItemAction()
SettingItemAction(string, SettingItemActionDelegate, int)
Typecasts setting as Button item

Declaration
public SettingItemAction(string name, SettingItemActionDelegate value, int sortIndex = 0)
Parameters
Type	Name	Description
string	name	
SettingItemActionDelegate	value	
int	sortIndex	
SettingItemAction(SettingItemAction)
Typecasts setting as Button item

Declaration
public SettingItemAction(SettingItemAction settingItem)
Parameters
Type	Name	Description
SettingItemAction	settingItem	
Properties
LabelText
Typecasts setting as Button item

Declaration
public string LabelText { get; set; }
Property Value
Type	Description
string	
Type
Typecasts setting as Button item

Declaration
public override SettingItemType Type { get; }
Property Value
Type	Description
SettingItemType	
Overrides
SettingItem.Type
Methods
ValueFromXElement(XElement, DeserializationInfo)
Typecasts setting as Button item

Declaration
protected override void ValueFromXElement(XElement element, DeserializationInfo deserializationInfo)
Parameters
Type	Name	Description
XElement	element	
DeserializationInfo	deserializationInfo	
Overrides
SettingItem.ValueFromXElement(XElement, DeserializationInfo)