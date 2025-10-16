# SettingItemGroup Class

Paste official Quantower documentation here.

**URL**: https://api.quantower.com/docs/TradingPlatform.BusinessLayer.SettingItemGroup.html
Class SettingItemGroup
Typecasts setting as TabControl item

Namespace: TradingPlatform.BusinessLayer
Syntax
public sealed class SettingItemGroup : SettingItemList
Constructors
SettingItemGroup()
Typecasts setting as TabControl item

Declaration
public SettingItemGroup()
SettingItemGroup(string, IList<SettingItem>, int)
Typecasts setting as TabControl item

Declaration
public SettingItemGroup(string name, IList<SettingItem> items, int sortIndex = 0)
Parameters
Type	Name	Description
string	name	
IList<SettingItem>	items	
int	sortIndex	
Fields
AllowCreateEmptyGroup
Typecasts setting as TabControl item

Declaration
public bool AllowCreateEmptyGroup
Field Value
Type	Description
bool	
Properties
FirstActionInfo
Typecasts setting as TabControl item

Declaration
public GroupActionInfo FirstActionInfo { get; set; }
Property Value
Type	Description
GroupActionInfo	
Items
Typecasts setting as TabControl item

Declaration
protected override List<SettingItem> Items { get; set; }
Property Value
Type	Description
List<SettingItem>	
Overrides
SettingItemList.Items
SecondActionInfo
Typecasts setting as TabControl item

Declaration
public GroupActionInfo SecondActionInfo { get; set; }
Property Value
Type	Description
GroupActionInfo	
Type
Typecasts setting as TabControl item

Declaration
public override SettingItemType Type { get; }
Property Value
Type	Description
SettingItemType	
Overrides
SettingItem.Type
Methods
AddItem(SettingItem)
Typecasts setting as TabControl item

Declaration
public void AddItem(SettingItem item)
Parameters
Type	Name	Description
SettingItem	item	
GetHashCode()
Serves as the default hash function.

Declaration
public override int GetHashCode()
Returns
Type	Description
int	
A hash code for the current object.

Overrides
SettingItem.GetHashCode()
