# HistoryAggregationRenko Class

Paste official Quantower documentation here.

**URL**: https://api.quantower.com/docs/TradingPlatform.BusinessLayer.HistoryAggregationRenko.html
Class HistoryAggregationRenko
Namespace: TradingPlatform.BusinessLayer
Syntax
public sealed class HistoryAggregationRenko : HistoryAggregationTime, IHistoryAggregationHistoryTypeSupport
Constructors
HistoryAggregationRenko(Period, HistoryType, int, RenkoStyle, int, int, bool, bool)
Declaration
public HistoryAggregationRenko(Period period, HistoryType historyType, int brickSize, RenkoStyle renkoStyle, int extension = 100, int inversion = 100, bool showWicks = false, bool buildCurrentBar = true)
Parameters
Type	Name	Description
Period	period	
HistoryType	historyType	
int	brickSize	
RenkoStyle	renkoStyle	
int	extension	
int	inversion	
bool	showWicks	
bool	buildCurrentBar	
Fields
SETTINGS_AGGREGATION_RENKO_BRICK_SIZE
Declaration
public const string SETTINGS_AGGREGATION_RENKO_BRICK_SIZE = "Brick size"
Field Value
Type	Description
string	
SETTINGS_AGGREGATION_RENKO_BUILD_CURRENT_BAR
Declaration
public const string SETTINGS_AGGREGATION_RENKO_BUILD_CURRENT_BAR = "Build current bar"
Field Value
Type	Description
string	
SETTINGS_AGGREGATION_RENKO_EXTENSION
Declaration
public const string SETTINGS_AGGREGATION_RENKO_EXTENSION = "Extension, %"
Field Value
Type	Description
string	
SETTINGS_AGGREGATION_RENKO_INVERSION
Declaration
public const string SETTINGS_AGGREGATION_RENKO_INVERSION = "Inversion, %"
Field Value
Type	Description
string	
SETTINGS_AGGREGATION_RENKO_SHOW_WICKS
Declaration
public const string SETTINGS_AGGREGATION_RENKO_SHOW_WICKS = "Show wicks"
Field Value
Type	Description
string	
SETTINGS_AGGREGATION_RENKO_STYLE
Declaration
public const string SETTINGS_AGGREGATION_RENKO_STYLE = "Style"
Field Value
Type	Description
string	
Properties
BrickSize
Declaration
public int BrickSize { get; }
Property Value
Type	Description
int	
BuildCurrentBar
Declaration
public bool BuildCurrentBar { get; }
Property Value
Type	Description
bool	
Extension
Declaration
public int Extension { get; }
Property Value
Type	Description
int	
Inversion
Declaration
public int Inversion { get; }
Property Value
Type	Description
int	
Name
Declaration
public override string Name { get; }
Property Value
Type	Description
string	
Overrides
HistoryAggregationTime.Name
RenkoStyle
Declaration
public RenkoStyle RenkoStyle { get; }
Property Value
Type	Description
RenkoStyle	
Settings
Declaration
public override IList<SettingItem> Settings { get; set; }
Property Value
Type	Description
IList<SettingItem>	
Overrides
HistoryAggregationTime.Settings
ShowWicks
Declaration
public bool ShowWicks { get; }
Property Value
Type	Description
bool	
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
HistoryAggregationTime.GetAggregationToDirectDownload(HistoryMetadata, ISessionsContainer)
GetBaseAggregation()
Declaration
public HistoryAggregation GetBaseAggregation()
Returns
Type	Description
HistoryAggregation	
Implements
IHistoryAggregationHistoryTypeSupport