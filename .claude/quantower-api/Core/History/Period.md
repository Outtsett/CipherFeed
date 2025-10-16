# Period Struct

Paste official Quantower documentation here.

**URL**: https://api.quantower.com/docs/TradingPlatform.BusinessLayer.Period.html
Struct Period
Represents mechanism for supporting predefined and custom periods

Namespace: TradingPlatform.BusinessLayer
Syntax
public struct Period
Constructors
Period(BasePeriod, int)
Creates Period instance with PeriodMultiplier greater than 0

Declaration
public Period(BasePeriod basePeriod, int periodMultiplier)
Parameters
Type	Name	Description
BasePeriod	basePeriod	
int	periodMultiplier	
Properties
BasePeriod
Gets base period type

Declaration
public readonly BasePeriod BasePeriod { get; }
Property Value
Type	Description
BasePeriod	
DAY1
Predefined period

Declaration
public static Period DAY1 { get; }
Property Value
Type	Description
Period	
Duration
Represents mechanism for supporting predefined and custom periods

Declaration
public TimeSpan Duration { get; }
Property Value
Type	Description
TimeSpan	
HOUR1
Predefined period

Declaration
public static Period HOUR1 { get; }
Property Value
Type	Description
Period	
HOUR12
Predefined period

Declaration
public static Period HOUR12 { get; }
Property Value
Type	Description
Period	
HOUR2
Predefined period

Declaration
public static Period HOUR2 { get; }
Property Value
Type	Description
Period	
HOUR3
Predefined period

Declaration
public static Period HOUR3 { get; }
Property Value
Type	Description
Period	
HOUR4
Predefined period

Declaration
public static Period HOUR4 { get; }
Property Value
Type	Description
Period	
HOUR6
Predefined period

Declaration
public static Period HOUR6 { get; }
Property Value
Type	Description
Period	
HOUR8
Predefined period

Declaration
public static Period HOUR8 { get; }
Property Value
Type	Description
Period	
MIN1
Predefined period

Declaration
public static Period MIN1 { get; }
Property Value
Type	Description
Period	
MIN10
Predefined period

Declaration
public static Period MIN10 { get; }
Property Value
Type	Description
Period	
MIN15
Predefined period

Declaration
public static Period MIN15 { get; }
Property Value
Type	Description
Period	
MIN2
Predefined period

Declaration
public static Period MIN2 { get; }
Property Value
Type	Description
Period	
MIN3
Predefined period

Declaration
public static Period MIN3 { get; }
Property Value
Type	Description
Period	
MIN30
Predefined period

Declaration
public static Period MIN30 { get; }
Property Value
Type	Description
Period	
MIN4
Predefined period

Declaration
public static Period MIN4 { get; }
Property Value
Type	Description
Period	
MIN5
Predefined period

Declaration
public static Period MIN5 { get; }
Property Value
Type	Description
Period	
MONTH1
Predefined period

Declaration
public static Period MONTH1 { get; }
Property Value
Type	Description
Period	
PeriodMultiplier
Gets period multiplier

Declaration
public readonly int PeriodMultiplier { get; }
Property Value
Type	Description
int	
SECOND1
Predefined period

Declaration
public static Period SECOND1 { get; }
Property Value
Type	Description
Period	
SECOND10
Predefined period

Declaration
public static Period SECOND10 { get; }
Property Value
Type	Description
Period	
SECOND15
Predefined period

Declaration
public static Period SECOND15 { get; }
Property Value
Type	Description
Period	
SECOND30
Predefined period

Declaration
public static Period SECOND30 { get; }
Property Value
Type	Description
Period	
SECOND5
Predefined period

Declaration
public static Period SECOND5 { get; }
Property Value
Type	Description
Period	
TICK1
Predefined period

Declaration
public static Period TICK1 { get; }
Property Value
Type	Description
Period	
Ticks
Gets ticks value as an result of base period TicksInBasePeriod(BasePeriod) multiplicated by PeriodMultiplier

Declaration
public long Ticks { get; }
Property Value
Type	Description
long	
WEEK1
Predefined period

Declaration
public static Period WEEK1 { get; }
Property Value
Type	Description
Period	
YEAR1
Predefined period

Declaration
public static Period YEAR1 { get; }
Property Value
Type	Description
Period	
Methods
BasePeriodToShortString(BasePeriod)
Returns shorted string according to base period type

Declaration
public static string BasePeriodToShortString(BasePeriod basePeriod)
Parameters
Type	Name	Description
BasePeriod	basePeriod	
Returns
Type	Description
string	
TicksInBasePeriod(BasePeriod)
Returns value in ticks according to base period type

Declaration
public static long TicksInBasePeriod(BasePeriod basePeriod)
Parameters
Type	Name	Description
BasePeriod	basePeriod	
Returns
Type	Description
long	
ToDatesRange(out DateTime, out DateTime)
Converts time gap into dates range

Declaration
public void ToDatesRange(out DateTime from, out DateTime to)
Parameters
Type	Name	Description
DateTime	from	
DateTime	to	
TryParse(string, out Period)
Represents mechanism for supporting predefined and custom periods

Declaration
public static bool TryParse(string value, out Period period)
Parameters
Type	Name	Description
string	value	
Period	period	
Returns
Type	Description
bool	
