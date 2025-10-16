# Strategy Class

Paste official Quantower documentation here.

**URL**: https://api.quantower.com/docs/TradingPlatform.BusinessLayer.Strategy.html
Class Strategy
The base class for strategies

Namespace: TradingPlatform.BusinessLayer
Syntax
public abstract class Strategy : ExecutionEntity
Constructors
Strategy()
The base class for strategies

Declaration
protected Strategy()
Properties
Id
Unique ID of the strategy

Declaration
public string Id { get; }
Property Value
Type	Description
string	
InstanceName
The base class for strategies

Declaration
public string InstanceName { get; set; }
Property Value
Type	Description
string	
MonitoringConnectionsIds
The base class for strategies

Declaration
public virtual string[] MonitoringConnectionsIds { get; }
Property Value
Type	Description
string[]	
NewVersionAvailable
The base class for strategies

Declaration
public bool NewVersionAvailable { get; }
Property Value
Type	Description
bool	
Settings
The base class for strategies

Declaration
public override IList<SettingItem> Settings { get; set; }
Property Value
Type	Description
IList<SettingItem>	
Overrides
ExecutionEntity.Settings
State
The current state of the strategy

Declaration
public StrategyState State { get; }
Property Value
Type	Description
StrategyState	
Methods
GetConnectionStateDependency()
The base class for strategies

Declaration
public ConnectionDependency GetConnectionStateDependency()
Returns
Type	Description
ConnectionDependency	
GetLogs(DateTime, DateTime)
Get logs from the strategy for specified date range

Declaration
public LoggerEvent[] GetLogs(DateTime from, DateTime to)
Parameters
Type	Name	Description
DateTime	from	
DateTime	to	
Returns
Type	Description
LoggerEvent[]	
GetMetrics()
Get current metrics from the strategy

Declaration
public List<StrategyMetric> GetMetrics()
Returns
Type	Description
List<StrategyMetric>	
Log(string, StrategyLoggingLevel)
Write log message

Declaration
protected void Log(string message, StrategyLoggingLevel level = StrategyLoggingLevel.Info)
Parameters
Type	Name	Description
string	message	
StrategyLoggingLevel	level	
OnCreated()
The base class for strategies

Declaration
protected virtual void OnCreated()
OnGetMetrics()
The base class for strategies

Declaration
protected virtual List<StrategyMetric> OnGetMetrics()
Returns
Type	Description
List<StrategyMetric>	
OnInitializeMetrics(Meter)
The base class for strategies

Declaration
protected virtual void OnInitializeMetrics(Meter meter)
Parameters
Type	Name	Description
Meter	meter	
OnRemove()
The base class for strategies

Declaration
protected virtual void OnRemove()
OnRun()
The base class for strategies

Declaration
protected virtual void OnRun()
OnStop()
The base class for strategies

Declaration
protected virtual void OnStop()
Remove()
Remove the strategy

Declaration
public void Remove()
Run()
Run strategy

Declaration
public void Run()
Stop()
Stop strategy

Declaration
public void Stop()
Events
NewLog
Event occured when strategy write a new log

Declaration
public event StrategyEventHandler NewLog
Event Type
Type	Description
StrategyEventHandler	
SettingsChanged
Event occured if any of strategy settings was changed

Declaration
public event Action<Strategy> SettingsChanged
Event Type
Type	Description
Action<Strategy>	
