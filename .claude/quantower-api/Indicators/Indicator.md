# Indicator Class

Paste official Quantower documentation here.

**URL**: https://api.quantower.com/docs/TradingPlatform.BusinessLayer.Indicator.html
Class Indicator
Base class for all indicators.

Namespace: TradingPlatform.BusinessLayer
Syntax
public abstract class Indicator : ExecutionEntity
Constructors
Indicator()
Base class for all indicators.

Declaration
protected Indicator()
Properties
AllowFitAuto
Specified, whether indicator should participate into price auto scale system.

Declaration
public bool AllowFitAuto { get; set; }
Property Value
Type	Description
bool	
Count
Amount of items in internal buffers

Declaration
public int Count { get; }
Property Value
Type	Description
int	
CurrentChart
Represent access to the chart, that created indicator

Declaration
public IChart CurrentChart { get; set; }
Property Value
Type	Description
IChart	
Digits
Precision amount for formatting price (the count of digits after decimal point); By default = -1, which means to use precision from indicator's symbol

Declaration
public int Digits { get; set; }
Property Value
Type	Description
int	
HelpLink
Base class for all indicators.

Declaration
public virtual string HelpLink { get; }
Property Value
Type	Description
string	
HistoricalData
Represent access to current used historical data.

Declaration
public HistoricalData HistoricalData { get; }
Property Value
Type	Description
HistoricalData	
IsUpdateTypesSupported
Base class for all indicators.

Declaration
protected bool IsUpdateTypesSupported { get; set; }
Property Value
Type	Description
bool	
Labels
Represent access indicator labels

Declaration
public AdditionalInfoItemBasic[] Labels { get; }
Property Value
Type	Description
AdditionalInfoItemBasic[]	
LinesLevels
Base class for all indicators.

Declaration
public LineLevel[] LinesLevels { get; }
Property Value
Type	Description
LineLevel[]	
LinesSeries
Represent access indicator series

Declaration
public LineSeries[] LinesSeries { get; }
Property Value
Type	Description
LineSeries[]	
OnBackGround
Specified, whether indicator should draw on chart background by default.

Declaration
public bool OnBackGround { get; set; }
Property Value
Type	Description
bool	
SeparateWindow
Specified, whether indicator should use main or additional window on the chart

Declaration
public bool SeparateWindow { get; set; }
Property Value
Type	Description
bool	
Settings
Indicator's settings

Declaration
public override IList<SettingItem> Settings { get; set; }
Property Value
Type	Description
IList<SettingItem>	
Overrides
ExecutionEntity.Settings
ShortName
Short name of indicator

Declaration
public virtual string ShortName { get; protected set; }
Property Value
Type	Description
string	
SourceCodeLink
Base class for all indicators.

Declaration
public virtual string SourceCodeLink { get; }
Property Value
Type	Description
string	
Symbol
Access to current Symbol of indicator

Declaration
public Symbol Symbol { get; }
Property Value
Type	Description
Symbol	
TFConfig
Base class for all indicators.

Declaration
public TimeFrameConfig TFConfig { get; }
Property Value
Type	Description
TimeFrameConfig	
UpdateType
Base class for all indicators.

Declaration
public IndicatorUpdateType UpdateType { get; set; }
Property Value
Type	Description
IndicatorUpdateType	
Methods
AddIndicator(Indicator)
Base class for all indicators.

Declaration
public void AddIndicator(Indicator indicator)
Parameters
Type	Name	Description
Indicator	indicator	
AddLabel(string, ComparingType, string, IFormattingDescription)
Base class for all indicators.

Declaration
public AdditionalInfoItemBasic AddLabel(string labelId, ComparingType type, string labelName, IFormattingDescription formattingDescription = null)
Parameters
Type	Name	Description
string	labelId	
ComparingType	type	
string	labelName	
IFormattingDescription	formattingDescription	
Returns
Type	Description
AdditionalInfoItemBasic	
AddLabel(AdditionalInfoItemBasic)
Base class for all indicators.

Declaration
public void AddLabel(AdditionalInfoItemBasic label)
Parameters
Type	Name	Description
AdditionalInfoItemBasic	label	
AddLineLevel(double, string, Color, int, LineStyle)
Base class for all indicators.

Declaration
public LineLevel AddLineLevel(double level, string lineName = "", Color lineColor = default, int lineWidth = 1, LineStyle lineStyle = LineStyle.Solid)
Parameters
Type	Name	Description
double	level	
string	lineName	
Color	lineColor	
int	lineWidth	
LineStyle	lineStyle	
Returns
Type	Description
LineLevel	
AddLineLevel(LineLevel)
Base class for all indicators.

Declaration
public void AddLineLevel(LineLevel lineLevel)
Parameters
Type	Name	Description
LineLevel	lineLevel	
AddLineSeries(string, Color, int, LineStyle)
Base class for all indicators.

Declaration
public LineSeries AddLineSeries(string lineName = "", Color lineColor = default, int lineWidth = 1, LineStyle lineStyle = LineStyle.Solid)
Parameters
Type	Name	Description
string	lineName	
Color	lineColor	
int	lineWidth	
LineStyle	lineStyle	
Returns
Type	Description
LineSeries	
AddLineSeries(LineSeries)
Base class for all indicators.

Declaration
public void AddLineSeries(LineSeries lineSeries)
Parameters
Type	Name	Description
LineSeries	lineSeries	
Ask(int)
Get Ask price

Declaration
protected double Ask(int offset = 0)
Parameters
Type	Name	Description
int	offset	
Offset value

Returns
Type	Description
double	
BeginCloud(int, int, Color, int)
Marks cloud begin between two line series with specific color

Declaration
protected void BeginCloud(int line1Index, int line2Index, Color color, int offset = 0)
Parameters
Type	Name	Description
int	line1Index	
First line series index

int	line2Index	
Second line series index

Color	color	
Cloud color

int	offset	
Offset

Bid(int)
Get Bid price

Declaration
protected double Bid(int offset = 0)
Parameters
Type	Name	Description
int	offset	
Offset value

Returns
Type	Description
double	
Calculate(HistoricalData)
Base class for all indicators.

Declaration
public void Calculate(HistoricalData historicalData)
Parameters
Type	Name	Description
HistoricalData	historicalData	
Clear()
Base class for all indicators.

Declaration
public void Clear()
Close(int)
Get Close price

Declaration
protected double Close(int offset = 0)
Parameters
Type	Name	Description
int	offset	
Offset value

Returns
Type	Description
double	
EndCloud(int, int, Color, int)
Marks cloud end between two line series with specific color

Declaration
protected void EndCloud(int line1Index, int line2Index, Color color, int offset = 0)
Parameters
Type	Name	Description
int	line1Index	
First line series index

int	line2Index	
Second line series index

Color	color	
Cloud color

int	offset	
Offset

FormatPrice(double)
Formatting price, using precision from assigned symbol or Digits value if specified

Declaration
public string FormatPrice(double price)
Parameters
Type	Name	Description
double	price	
Price value

Returns
Type	Description
string	
FundingRate(int)
Get Funding rate

Declaration
protected double FundingRate(int offset = 0)
Parameters
Type	Name	Description
int	offset	
Offset value

Returns
Type	Description
double	
GetBarAppearance(int)
Base class for all indicators.

Declaration
public IndicatorBarAppearance GetBarAppearance(int offset = 0)
Parameters
Type	Name	Description
int	offset	
Returns
Type	Description
IndicatorBarAppearance	
GetLabelValue(string)
Gets the indicator label value by unique Id

Declaration
public ValueProvider GetLabelValue(string labelId)
Parameters
Type	Name	Description
string	labelId	
Unique label Id

Returns
Type	Description
ValueProvider	
ValueProvider

GetLineBreak(int, int, SeekOriginHistory)
Check if the point is a break point.

Declaration
public bool GetLineBreak(int offset = 0, int lineIndex = 0, SeekOriginHistory origin = SeekOriginHistory.End)
Parameters
Type	Name	Description
int	offset	
Offset value

int	lineIndex	
Index of indicator line

SeekOriginHistory	origin	
Offset start point

Returns
Type	Description
bool	
GetPrice(PriceType, int)
Gets the price from historical data

Declaration
protected double GetPrice(PriceType priceType, int offset = 0)
Parameters
Type	Name	Description
PriceType	priceType	
int	offset	
Returns
Type	Description
double	
GetValue(int, int, SeekOriginHistory)
Gets the value of indicator from internal buffer

Declaration
public double GetValue(int offset = 0, int lineIndex = 0, SeekOriginHistory origin = SeekOriginHistory.End)
Parameters
Type	Name	Description
int	offset	
Offset value

int	lineIndex	
Index of indicator line

SeekOriginHistory	origin	
Offset start point

Returns
Type	Description
double	
GetVolumeAnalysisData(int)
Base class for all indicators.

Declaration
protected VolumeAnalysisData GetVolumeAnalysisData(int offset = 0)
Parameters
Type	Name	Description
int	offset	
Returns
Type	Description
VolumeAnalysisData	
High(int)
Get High price

Declaration
protected double High(int offset = 0)
Parameters
Type	Name	Description
int	offset	
Offset value

Returns
Type	Description
double	
Init()
Base class for all indicators.

Declaration
public void Init()
Last(int)
Get Last price

Declaration
protected double Last(int offset = 0)
Parameters
Type	Name	Description
int	offset	
Offset value

Returns
Type	Description
double	
Low(int)
Get Low price

Declaration
protected double Low(int offset = 0)
Parameters
Type	Name	Description
int	offset	
Offset value

Returns
Type	Description
double	
Median(int)
Get Median price

Declaration
protected double Median(int offset = 0)
Parameters
Type	Name	Description
int	offset	
Offset value

Returns
Type	Description
double	
OnClear()
Base class for all indicators.

Declaration
protected virtual void OnClear()
OnInit()
Base class for all indicators.

Declaration
protected virtual void OnInit()
OnPaintChart(PaintChartEventArgs)
Base class for all indicators.

Declaration
public virtual void OnPaintChart(PaintChartEventArgs args)
Parameters
Type	Name	Description
PaintChartEventArgs	args	
OnSettingsUpdated()
Base class for all indicators.

Declaration
protected override void OnSettingsUpdated()
Overrides
ExecutionEntity.OnSettingsUpdated()
OnTryGetMinMax(int, int, out double, out double)
Base class for all indicators.

Declaration
protected virtual bool OnTryGetMinMax(int fromOffset, int toOffset, out double min, out double max)
Parameters
Type	Name	Description
int	fromOffset	
int	toOffset	
double	min	
double	max	
Returns
Type	Description
bool	
OnUpdate(UpdateArgs)
Base class for all indicators.

Declaration
protected virtual void OnUpdate(UpdateArgs args)
Parameters
Type	Name	Description
UpdateArgs	args	
Open(int)
Get Open price

Declaration
protected double Open(int offset = 0)
Parameters
Type	Name	Description
int	offset	
Offset value

Returns
Type	Description
double	
OpenInterest(int)
Get Open interest

Declaration
protected double OpenInterest(int offset = 0)
Parameters
Type	Name	Description
int	offset	
Offset value

Returns
Type	Description
double	
PaintChart(PaintChartEventArgs)
Base class for all indicators.

Declaration
public void PaintChart(PaintChartEventArgs ev)
Parameters
Type	Name	Description
PaintChartEventArgs	ev	
QuoteAssetVolume(int)
Get Volume in quoting asset

Declaration
protected double QuoteAssetVolume(int offset = 0)
Parameters
Type	Name	Description
int	offset	
Offset value

Returns
Type	Description
double	
Refresh()
Recalculate indicator

Declaration
public void Refresh()
RemoveIndicator(Indicator)
Base class for all indicators.

Declaration
public void RemoveIndicator(Indicator indicator)
Parameters
Type	Name	Description
Indicator	indicator	
RemoveLineBreak(int, int, SeekOriginHistory)
Remove line break point.

Declaration
public void RemoveLineBreak(int offset = 0, int lineIndex = 0, SeekOriginHistory origin = SeekOriginHistory.End)
Parameters
Type	Name	Description
int	offset	
Offset value

int	lineIndex	
Index of indicator line

SeekOriginHistory	origin	
Offset start point

SetBarAppearance(IndicatorBarAppearance, int)
Base class for all indicators.

Declaration
public void SetBarAppearance(IndicatorBarAppearance barAppearance, int offset = 0)
Parameters
Type	Name	Description
IndicatorBarAppearance	barAppearance	
int	offset	
SetBarColor(Color?, int)
Base class for all indicators.

Declaration
public void SetBarColor(Color? color = null, int offset = 0)
Parameters
Type	Name	Description
Color?	color	
int	offset	
SetLabelValue(string, bool)
Sets the indicator label by unique Id

Declaration
public void SetLabelValue(string labelId, bool value)
Parameters
Type	Name	Description
string	labelId	
Unique label Id

bool	value	
Value

SetLabelValue(string, DateTime)
Sets the indicator label by unique Id

Declaration
public void SetLabelValue(string labelId, DateTime value)
Parameters
Type	Name	Description
string	labelId	
Unique label Id

DateTime	value	
Value

SetLabelValue(string, double)
Sets the indicator label by unique Id

Declaration
public void SetLabelValue(string labelId, double value)
Parameters
Type	Name	Description
string	labelId	
Unique label Id

double	value	
Value

SetLabelValue(string, int)
Sets the indicator label by unique Id

Declaration
public void SetLabelValue(string labelId, int value)
Parameters
Type	Name	Description
string	labelId	
Unique label Id

int	value	
Value

SetLabelValue(string, long)
Sets the indicator label by unique Id

Declaration
public void SetLabelValue(string labelId, long value)
Parameters
Type	Name	Description
string	labelId	
Unique label Id

long	value	
Value

SetLabelValue(string, string)
Sets the indicator label by unique Id

Declaration
public void SetLabelValue(string labelId, string value)
Parameters
Type	Name	Description
string	labelId	
Unique label Id

string	value	
Value

SetLineBreak(int, int, SeekOriginHistory)
Set line break point.

Declaration
public void SetLineBreak(int offset = 0, int lineIndex = 0, SeekOriginHistory origin = SeekOriginHistory.End)
Parameters
Type	Name	Description
int	offset	
Offset value

int	lineIndex	
Index of indicator line

SeekOriginHistory	origin	
Offset start point

SetValue(double, int, int)
Sets the value of indicator into internal buffer

Declaration
public void SetValue(double value, int lineIndex = 0, int offset = 0)
Parameters
Type	Name	Description
double	value	
Value

int	lineIndex	
Index of indicator line

int	offset	
Offset value

Ticks(int)
Get Ticks

Declaration
protected double Ticks(int offset = 0)
Parameters
Type	Name	Description
int	offset	
Offset value

Returns
Type	Description
double	
Time(int)
Get Time

Declaration
protected DateTime Time(int offset = 0)
Parameters
Type	Name	Description
int	offset	
Offset value

Returns
Type	Description
DateTime	
TryGetMinMax(int, int, out double, out double)
Base class for all indicators.

Declaration
public bool TryGetMinMax(int fromOffset, int toOffset, out double min, out double max)
Parameters
Type	Name	Description
int	fromOffset	
int	toOffset	
double	min	
double	max	
Returns
Type	Description
bool	
Typical(int)
Get Typical price

Declaration
protected double Typical(int offset = 0)
Parameters
Type	Name	Description
int	offset	
Offset value

Returns
Type	Description
double	
Update(UpdateArgs)
Base class for all indicators.

Declaration
public void Update(UpdateArgs args)
Parameters
Type	Name	Description
UpdateArgs	args	
Volume(int)
Get Volume

Declaration
protected double Volume(int offset = 0)
Parameters
Type	Name	Description
int	offset	
Offset value

Returns
Type	Description
double	
Weighted(int)
Get Weighted price

Declaration
protected double Weighted(int offset = 0)
Parameters
Type	Name	Description
int	offset	
Offset value

Returns
Type	Description
double	
