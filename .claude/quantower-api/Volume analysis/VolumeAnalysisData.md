# VolumeAnalysisData Class

Paste official Quantower documentation here.

**URL**: https://api.quantower.com/docs/TradingPlatform.BusinessLayer.VolumeAnalysisData.html
Class VolumeAnalysisData
Namespace: TradingPlatform.BusinessLayer
Syntax
public class VolumeAnalysisData
Constructors
VolumeAnalysisData()
Declaration
public VolumeAnalysisData()
Properties
PriceLevels
Volume info for each price

Declaration
public Dictionary<double, VolumeAnalysisItem> PriceLevels { get; set; }
Property Value
Type	Description
Dictionary<double, VolumeAnalysisItem>	
TimeLeft
Declaration
public DateTime TimeLeft { get; set; }
Property Value
Type	Description
DateTime	
Total
Summary calculated Volume info

Declaration
public VolumeAnalysisItem Total { get; set; }
Property Value
Type	Description
VolumeAnalysisItem	
Methods
Calculate(double, double, AggressorFlag)
Declaration
public void Calculate(double price, double size, AggressorFlag aggressorFlag)
Parameters
Type	Name	Description
double	price	
double	size	
AggressorFlag	aggressorFlag	
Combine(VolumeAnalysisData)
Declaration
public void Combine(VolumeAnalysisData data)
Parameters
Type	Name	Description
VolumeAnalysisData	data	
CreateAggregatedSnapshot(double)
Declaration
public VolumeAnalysisData CreateAggregatedSnapshot(double aggregationStep)
Parameters
Type	Name	Description
double	aggregationStep	
Returns
Type	Description
VolumeAnalysisData	
ToString()
Returns a string that represents the current object.

Declaration
public override string ToString()
Returns
Type	Description
string	
A string that represents the current object.

Overrides
object.ToString()
Events
ItemUpdated
Fire in case of price level was added or existing was updated

Declaration
public event EventHandler<VolumeAnalysisDataEventArgs> ItemUpdated
Event Type
Type	Description
EventHandler<VolumeAnalysisDataEventArgs>