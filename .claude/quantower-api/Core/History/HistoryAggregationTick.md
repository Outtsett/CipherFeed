# HistoryAggregationTick Class

Paste official Quantower documentation here.

**URL**: https://api.quantower.com/docs/TradingPlatform.BusinessLayer.HistoryAggregationTick.html
Class HistoryAggregationTick
Namespace: TradingPlatform.BusinessLayer
Syntax
public sealed class HistoryAggregationTick : HistoryAggregation, IHistoryAggregationHistoryTypeSupport
Constructors
HistoryAggregationTick(HistoryAggregationTick)
Declaration
public HistoryAggregationTick(HistoryAggregationTick aggregation)
Parameters
Type	Name	Description
HistoryAggregationTick	aggregation	
HistoryAggregationTick(HistoryType)
Declaration
public HistoryAggregationTick(HistoryType historyType)
Parameters
Type	Name	Description
HistoryType	historyType	
Properties
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