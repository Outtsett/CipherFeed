# HistoryAggregationPointsAndFigures Class

Paste official Quantower documentation here.

**URL**: https://api.quantower.com/docs/TradingPlatform.BusinessLayer.HistoryAggregationPointsAndFigures.html
Class HistoryAggregationPointsAndFigures
Namespace: TradingPlatform.BusinessLayer
Syntax
public sealed class HistoryAggregationPointsAndFigures : HistoryAggregationTime, IHistoryAggregationHistoryTypeSupport
Constructors
HistoryAggregationPointsAndFigures(HistoryAggregationPointsAndFigures)
Declaration
protected HistoryAggregationPointsAndFigures(HistoryAggregationPointsAndFigures aggregation)
Parameters
Type	Name	Description
HistoryAggregationPointsAndFigures	aggregation	
HistoryAggregationPointsAndFigures(Period, HistoryType, int, int, PointsAndFiguresStyle)
Declaration
public HistoryAggregationPointsAndFigures(Period period, HistoryType historyType, int boxSize, int reversal, PointsAndFiguresStyle style)
Parameters
Type	Name	Description
Period	period	
HistoryType	historyType	
int	boxSize	
int	reversal	
PointsAndFiguresStyle	style	
Properties
BoxSize
Declaration
public int BoxSize { get; }
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
Reversal
Declaration
public int Reversal { get; }
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
HistoryAggregationTime.Settings
Style
Declaration
public PointsAndFiguresStyle Style { get; }
Property Value
Type	Description
PointsAndFiguresStyle	
Implements
IHistoryAggregationHistoryTypeSupport