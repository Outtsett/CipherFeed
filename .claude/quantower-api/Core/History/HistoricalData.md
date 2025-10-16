# HistoricalData Class

Paste official Quantower documentation here.

**URL**: https://api.quantower.com/docs/TradingPlatform.BusinessLayer.HistoricalData.html
Class HistoricalData
Represent access to historical data information and indicators control.

Namespace: TradingPlatform.BusinessLayer
Syntax
public class HistoricalData
Constructors
HistoricalData(HistoryRequestParameters)
Represent access to historical data information and indicators control.

Declaration
protected HistoricalData(HistoryRequestParameters historyRequestParameters)
Parameters
Type	Name	Description
HistoryRequestParameters	historyRequestParameters	
Fields
Indicators
Represent access to historical data information and indicators control.

Declaration
protected readonly IndicatorsCollection Indicators
Field Value
Type	Description
IndicatorsCollection	
Parameters
Represent access to historical data information and indicators control.

Declaration
protected HistoryRequestParameters Parameters
Field Value
Type	Description
HistoryRequestParameters	
itemsLocker
Represent access to historical data information and indicators control.

Declaration
protected readonly object itemsLocker
Field Value
Type	Description
object	
Properties
Aggregation
Gets HistoricalData aggregation

Declaration
public HistoryAggregation Aggregation { get; }
Property Value
Type	Description
HistoryAggregation	
AttachedIndicators
Gets array of attached indicators

Declaration
public Indicator[] AttachedIndicators { get; }
Property Value
Type	Description
Indicator[]	
BuiltInIndicators
Gets access to built-in indicators

Declaration
public BuiltInIndicators BuiltInIndicators { get; }
Property Value
Type	Description
BuiltInIndicators	
Count
Gets HistoricalData items amount

Declaration
public virtual int Count { get; }
Property Value
Type	Description
int	
FromTime
Gets HistoricalData left time boundary

Declaration
public DateTime FromTime { get; }
Property Value
Type	Description
DateTime	
IndicatorCalculationBehavior
Represent access to historical data information and indicators control.

Declaration
public IndicatorCalculationBehavior IndicatorCalculationBehavior { get; set; }
Property Value
Type	Description
IndicatorCalculationBehavior	
this[int, SeekOriginHistory]
Retrieves HistoricalData item by indexing offset and direction to find.

Declaration
public virtual IHistoryItem this[int offset, SeekOriginHistory origin = SeekOriginHistory.End] { get; }
Parameters
Type	Name	Description
int	offset	
SeekOriginHistory	origin	
Property Value
Type	Description
IHistoryItem	
NeedSubscribe
Represent access to historical data information and indicators control.

Declaration
protected virtual bool NeedSubscribe { get; }
Property Value
Type	Description
bool	
Symbol
Gets HistoricalData symbol

Declaration
public Symbol Symbol { get; }
Property Value
Type	Description
Symbol	
ToTime
Gets HistoricalData right time boundary

Declaration
public DateTime ToTime { get; }
Property Value
Type	Description
DateTime	
Methods
AddIndicator(string, params SettingItem[])
Creates indicator by it's name and if it successfully created adds it to the HistoricalData

Declaration
public Indicator AddIndicator(string indicatorName, params SettingItem[] settings)
Parameters
Type	Name	Description
string	indicatorName	
SettingItem[]	settings	
Returns
Type	Description
Indicator	
AddIndicator(Indicator)
Adds indicator to the HistoricalData

Declaration
public void AddIndicator(Indicator indicator)
Parameters
Type	Name	Description
Indicator	indicator	
AddNewItem(IHistoryItem, bool, HistoryEventArgs)
Represent access to historical data information and indicators control.

Declaration
protected virtual void AddNewItem(IHistoryItem historyItem, bool updateIndicators = true, HistoryEventArgs e = null)
Parameters
Type	Name	Description
IHistoryItem	historyItem	
bool	updateIndicators	
HistoryEventArgs	e	
CalculateVolumeProfile(VolumeAnalysisCalculationParameters)
Represent access to historical data information and indicators control.

Declaration
public IVolumeAnalysisCalculationProgress CalculateVolumeProfile(VolumeAnalysisCalculationParameters volumeAnalysisCalculationParameters)
Parameters
Type	Name	Description
VolumeAnalysisCalculationParameters	volumeAnalysisCalculationParameters	
Returns
Type	Description
IVolumeAnalysisCalculationProgress	
CreateHistoryProcessor()
Represent access to historical data information and indicators control.

Declaration
protected virtual IHistoryProcessor CreateHistoryProcessor()
Returns
Type	Description
IHistoryProcessor	
GetEnumerator()
Returns an enumerator that iterates through a collection.

Declaration
public IEnumerator GetEnumerator()
Returns
Type	Description
IEnumerator	
An IEnumerator object that can be used to iterate through the collection.

GetIndexByTime(long, SeekOriginHistory)
Gets index by time with counting on search direction

Declaration
public double GetIndexByTime(long time, SeekOriginHistory origin = SeekOriginHistory.End)
Parameters
Type	Name	Description
long	time	
SeekOriginHistory	origin	
Returns
Type	Description
double	
GetTimeToNextBar()
Represent access to historical data information and indicators control.

Declaration
public string GetTimeToNextBar()
Returns
Type	Description
string	
ProcessLast(Last)
Represent access to historical data information and indicators control.

Declaration
protected virtual void ProcessLast(Last last)
Parameters
Type	Name	Description
Last	last	
ProcessLevel2Qute(MessageQuote)
Represent access to historical data information and indicators control.

Declaration
protected virtual void ProcessLevel2Qute(MessageQuote quote)
Parameters
Type	Name	Description
MessageQuote	quote	
ProcessMark(Mark)
Represent access to historical data information and indicators control.

Declaration
protected virtual void ProcessMark(Mark mark)
Parameters
Type	Name	Description
Mark	mark	
ProcessQuote(Quote)
Represent access to historical data information and indicators control.

Declaration
protected virtual void ProcessQuote(Quote quote)
Parameters
Type	Name	Description
Quote	quote	
Reload()
Reloads entire HistoricalData

Declaration
public void Reload()
RemoveIndicator(Indicator)
Removes indicator from the HistoricalData

Declaration
public void RemoveIndicator(Indicator indicator)
Parameters
Type	Name	Description
Indicator	indicator	
SubscribeSymbol()
Represent access to historical data information and indicators control.

Declaration
protected virtual void SubscribeSymbol()
Symbol_NewLast(Symbol, Last)
Represent access to historical data information and indicators control.

Declaration
protected void Symbol_NewLast(Symbol symbol, Last last)
Parameters
Type	Name	Description
Symbol	symbol	
Last	last	
Symbol_NewQuote(Symbol, Quote)
Represent access to historical data information and indicators control.

Declaration
protected void Symbol_NewQuote(Symbol symbol, Quote quote)
Parameters
Type	Name	Description
Symbol	symbol	
Quote	quote	
UnSubscribeSymbol()
Represent access to historical data information and indicators control.

Declaration
protected virtual void UnSubscribeSymbol()
Events
HistoryItemUpdated
Will be triggered when current historical item changed or updated

Declaration
public event HistoryEventHandler HistoryItemUpdated
Event Type
Type	Description
HistoryEventHandler	
HistoryItemVolumeAnalysisUpdated
Will be triggered when volume analysis of current historical item changed or updated

Declaration
public event Action HistoryItemVolumeAnalysisUpdated
Event Type
Type	Description
Action	
NewHistoryItem
Will be triggered when new historical item created

Declaration
public event HistoryEventHandler NewHistoryItem
Event Type
Type	Description
HistoryEventHandler	
