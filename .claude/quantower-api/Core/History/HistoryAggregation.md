# HistoryAggregation Class

Paste official Quantower documentation here.

**URL**: https://api.quantower.com/docs/TradingPlatform.BusinessLayer.HistoryAggregation.html
Class HistoryAggregation
Namespace: TradingPlatform.BusinessLayer
Syntax
public abstract class HistoryAggregation
Constructors
HistoryAggregation()
Declaration
protected HistoryAggregation()
HistoryAggregation(HistoryAggregation)
Declaration
protected HistoryAggregation(HistoryAggregation origin)
Parameters
Type	Name	Description
HistoryAggregation	origin	
Fields
DELTA_BARS
Declaration
public const string DELTA_BARS = "Delta bars"
Field Value
Type	Description
string	
DOM_AGGREGATED
Declaration
public const string DOM_AGGREGATED = "Aggregated DOM"
Field Value
Type	Description
string	
DOM_BY_TICKS_COUNT
Declaration
public const string DOM_BY_TICKS_COUNT = "DOM by ticks count"
Field Value
Type	Description
string	
DOM_BY_TIME
Declaration
public const string DOM_BY_TIME = "DOM by time"
Field Value
Type	Description
string	
HEIKIN_ASHI
Declaration
public const string HEIKIN_ASHI = "Heikin Ashi"
Field Value
Type	Description
string	
KAGI
Declaration
public const string KAGI = "Kagi"
Field Value
Type	Description
string	
LEVEL2
Declaration
public const string LEVEL2 = "Level2"
Field Value
Type	Description
string	
LINE_BREAK
Declaration
public const string LINE_BREAK = "Line Break"
Field Value
Type	Description
string	
POINTS_AND_FIGURES
Declaration
public const string POINTS_AND_FIGURES = "Points & Figures"
Field Value
Type	Description
string	
POWER_TRADES
Declaration
public const string POWER_TRADES = "Power Trades"
Field Value
Type	Description
string	
PRICE_CHANGES_COUNT_BARS
Declaration
public const string PRICE_CHANGES_COUNT_BARS = "Price changes count bars"
Field Value
Type	Description
string	
RANGE_BARS
Declaration
public const string RANGE_BARS = "Range bars"
Field Value
Type	Description
string	
RENKO
Declaration
public const string RENKO = "Renko"
Field Value
Type	Description
string	
REVERSAL
Declaration
public const string REVERSAL = "Reversal"
Field Value
Type	Description
string	
SETTINGS_AGGREGATION_HISTORY_TYPE
Declaration
public const string SETTINGS_AGGREGATION_HISTORY_TYPE = "History type"
Field Value
Type	Description
string	
SETTINGS_AGGREGATION_PERIOD
Declaration
public const string SETTINGS_AGGREGATION_PERIOD = "Period"
Field Value
Type	Description
string	
SPY_MONEY_BARS
Declaration
public const string SPY_MONEY_BARS = "Spy money bars"
Field Value
Type	Description
string	
TICK
Declaration
public const string TICK = "Tick"
Field Value
Type	Description
string	
TICK_BARS
Declaration
public const string TICK_BARS = "Tick bars"
Field Value
Type	Description
string	
TICK_LAST_AGGREGATED
Declaration
public const string TICK_LAST_AGGREGATED = "Aggregated ticks (Last)"
Field Value
Type	Description
string	
TIME
Declaration
public const string TIME = "Time"
Field Value
Type	Description
string	
TIME_STATISTICS
Declaration
public const string TIME_STATISTICS = "Time statistics"
Field Value
Type	Description
string	
VOLUME
Declaration
public const string VOLUME = "Volume"
Field Value
Type	Description
string	
VOLUME_PROFILE
Declaration
public const string VOLUME_PROFILE = "Volume profile"
Field Value
Type	Description
string	
VWAP
Declaration
public const string VWAP = "VWAP"
Field Value
Type	Description
string	
Properties
DefaultRange
Declaration
public virtual Period DefaultRange { get; }
Property Value
Type	Description
Period	
IsWaitingFirstQuoteRequired
Declaration
public virtual bool IsWaitingFirstQuoteRequired { get; }
Property Value
Type	Description
bool	
Name
Declaration
public abstract string Name { get; }
Property Value
Type	Description
string	
SessionsContainer
Declaration
public ISessionsContainer SessionsContainer { get; set; }
Property Value
Type	Description
ISessionsContainer	
Settings
Declaration
public virtual IList<SettingItem> Settings { get; set; }
Property Value
Type	Description
IList<SettingItem>	
Title
Declaration
public virtual string Title { get; }
Property Value
Type	Description
string	
Methods
Equals(object)
Determines whether the specified object is equal to the current object.

Declaration
public override bool Equals(object obj)
Parameters
Type	Name	Description
object	obj	
The object to compare with the current object.

Returns
Type	Description
bool	
true if the specified object is equal to the current object; otherwise, false.

Overrides
object.Equals(object)
Equals(HistoryAggregation)
Indicates whether the current object is equal to another object of the same type.

Declaration
public bool Equals(HistoryAggregation other)
Parameters
Type	Name	Description
HistoryAggregation	other	
An object to compare with this object.

Returns
Type	Description
bool	
true if the current object is equal to the other parameter; otherwise, false.

GetAggregationToDirectDownload(HistoryMetadata, ISessionsContainer)
Declaration
public abstract HistoryAggregation GetAggregationToDirectDownload(HistoryMetadata metadata, ISessionsContainer sessionsContainer)
Parameters
Type	Name	Description
HistoryMetadata	metadata	
ISessionsContainer	sessionsContainer	
Returns
Type	Description
HistoryAggregation	
GetHashCode()
Serves as the default hash function.

Declaration
public override int GetHashCode()
Returns
Type	Description
int	
A hash code for the current object.

Overrides
object.GetHashCode()