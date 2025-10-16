# SettingItemPeriod Class

Paste official Quantower documentation here.

**URL**: https://api.quantower.com/docs/TradingPlatform.BusinessLayer.SettingItemPeriod.html
Class SettingItemPeriod
Typecasts setting as Period item

Namespace: TradingPlatform.BusinessLayer
Syntax
public sealed class SettingItemPeriod : SettingItem
Constructors
SettingItemPeriod()
Typecasts setting as Period item

Declaration
public SettingItemPeriod()
SettingItemPeriod(string, Period, int)
Typecasts setting as Period item

Declaration
public SettingItemPeriod(string name, Period value, int sortIndex = 0)
Parameters
Type	Name	Description
string	name	
Period	value	
int	sortIndex	
Properties
ExcludedPeriods
Typecasts setting as Period item

Declaration
public BasePeriod[] ExcludedPeriods { get; set; }
Property Value
Type	Description
BasePeriod[]	
MultiplierMaximum
Typecasts setting as Period item

Declaration
public int MultiplierMaximum { get; set; }
Property Value
Type	Description
int	
MultiplierMinimum
Typecasts setting as Period item

Declaration
public int MultiplierMinimum { get; set; }
Property Value
Type	Description
int	
Type
Typecasts setting as Period item

Declaration
public override SettingItemType Type { get; }
Property Value
Type	Description
SettingItemType	
Overrides
SettingItem.Type
