# HistoryAggregationTime Class

Paste official Quantower documentation here.

**URL**: https://api.quantower.com/docs/TradingPlatform.BusinessLayer.HistoryAggregationTime.html
Class HistoryAggregationTime
Namespace: TradingPlatform.BusinessLayer
Syntax
public class HistoryAggregationTime : HistoryAggregation, IHistoryAggregationHistoryTypeSupport
Constructors
HistoryAggregationTime(HistoryAggregationTime)
Declaration
protected HistoryAggregationTime(HistoryAggregationTime aggregation)
Parameters
Type	Name	Description
HistoryAggregationTime	aggregation	
HistoryAggregationTime(Period, HistoryType)
Declaration
public HistoryAggregationTime(Period period, HistoryType historyType)
Parameters
Type	Name	Description
Period	period	
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
Period
Declaration
public Period Period { get; set; }
Property Value
Type	Description
Period	
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
