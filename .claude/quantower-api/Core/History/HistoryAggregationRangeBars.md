# HistoryAggregationRangeBars Class

Paste official Quantower documentation here.

**URL**: https://api.quantower.com/docs/TradingPlatform.BusinessLayer.HistoryAggregationRangeBars.html
Class HistoryAggregationRangeBars
Namespace: TradingPlatform.BusinessLayer
Syntax
public sealed class HistoryAggregationRangeBars : HistoryAggregation, IHistoryAggregationHistoryTypeSupport
Constructors
HistoryAggregationRangeBars(int, HistoryType)
Declaration
public HistoryAggregationRangeBars(int rangeBars, HistoryType historyType)
Parameters
Type	Name	Description
int	rangeBars	
HistoryType	historyType	
Fields
SETTINGS_AGGREGATION_RANGE_BARS
Declaration
public const string SETTINGS_AGGREGATION_RANGE_BARS = "Range bars"
Field Value
Type	Description
string	
Properties
GetPeriod
Declaration
public override Period GetPeriod { get; }
Property Value
Type	Description
Period	
Overrides
HistoryAggregation.GetPeriod
HistoryType
Declaration
public HistoryType HistoryType { get; set; }
Property Value
Type	Description
HistoryType	
Name
Declaration
public override string Name { get; }
Property Value
Type	Description
string	
Overrides
HistoryAggregation.Name
RangeBars
Declaration
public int RangeBars { get; }
Property Value
Type	Description
int	
Settings
Declaration
public override IList<SettingItem> Settings { get; set; }
Property Value
Type	Description
IList<SettingItem>	
Overrides
HistoryAggregation.Settings
Methods
GetAggregationToDirectDownload(HistoryMetadata, ISessionsContainer)
Declaration
public override HistoryAggregation GetAggregationToDirectDownload(HistoryMetadata metadata, ISessionsContainer sessionsContainer)
Parameters
Type	Name	Description
HistoryMetadata	metadata	
ISessionsContainer	sessionsContainer	
Returns
Type	Description
HistoryAggregation	
Overrides
HistoryAggregation.GetAggregationToDirectDownload(HistoryMetadata, ISessionsContainer)
Implements
IHistoryAggregationHistoryTypeSupport