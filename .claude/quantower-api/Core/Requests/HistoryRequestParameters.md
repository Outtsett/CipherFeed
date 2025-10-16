# HistoryRequestParameters Class

Paste official Quantower documentation here.

**URL**: https://api.quantower.com/docs/TradingPlatform.BusinessLayer.HistoryRequestParameters.html
Class HistoryRequestParameters
Resolves a history request parameters per symbol

Namespace: TradingPlatform.BusinessLayer
Syntax
public sealed class HistoryRequestParameters : RequestParameters
Constructors
HistoryRequestParameters()
Resolves a history request parameters per symbol

Declaration
public HistoryRequestParameters()
HistoryRequestParameters(HistoryRequestParameters)
Resolves a history request parameters per symbol

Declaration
public HistoryRequestParameters(HistoryRequestParameters original)
Parameters
Type	Name	Description
HistoryRequestParameters	original	
Properties
Aggregation
Resolves a history request parameters per symbol

Declaration
public HistoryAggregation Aggregation { get; set; }
Property Value
Type	Description
HistoryAggregation	
Copy
Resolves a history request parameters per symbol

Declaration
public HistoryRequestParameters Copy { get; }
Property Value
Type	Description
HistoryRequestParameters	
ExcludeOutOfSession
Resolves a history request parameters per symbol

Declaration
public bool ExcludeOutOfSession { get; set; }
Property Value
Type	Description
bool	
ForceReload
Resolves a history request parameters per symbol

Declaration
public bool ForceReload { get; set; }
Property Value
Type	Description
bool	
FromTime
Resolves a history request parameters per symbol

Declaration
public DateTime FromTime { get; set; }
Property Value
Type	Description
DateTime	
HistoryRequestType
Resolves a history request parameters per symbol

Declaration
public HistoryRequestType HistoryRequestType { get; init; }
Property Value
Type	Description
HistoryRequestType	
Interval
Resolves a history request parameters per symbol

Declaration
public Interval<DateTime> Interval { get; set; }
Property Value
Type	Description
Interval<DateTime>	
IsResetOnSessionBoundaryEnabled
Resolves a history request parameters per symbol

Declaration
public bool IsResetOnSessionBoundaryEnabled { get; set; }
Property Value
Type	Description
bool	
ProgressInfo
Resolves a history request parameters per symbol

Declaration
public IProgress<float> ProgressInfo { get; set; }
Property Value
Type	Description
IProgress<float>	
SessionsContainer
Resolves a history request parameters per symbol

Declaration
public ISessionsContainer SessionsContainer { get; set; }
Property Value
Type	Description
ISessionsContainer	
Symbol
Resolves a history request parameters per symbol

Declaration
public Symbol Symbol { get; set; }
Property Value
Type	Description
Symbol	
SymbolId
Resolves a history request parameters per symbol

Declaration
public string SymbolId { get; set; }
Property Value
Type	Description
string	
ToTime
Resolves a history request parameters per symbol

Declaration
public DateTime ToTime { get; set; }
Property Value
Type	Description
DateTime	
Type
Resolves a history request parameters per symbol

Declaration
public override RequestType Type { get; }
Property Value
Type	Description
RequestType	
Overrides
RequestParameters.Type
Methods
ToDescription()
Resolves a history request parameters per symbol

Declaration
public HistoryDescription ToDescription()
Returns
Type	Description
HistoryDescription