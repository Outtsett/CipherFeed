# VolumeAnalysisManager Class

Paste official Quantower documentation here.

**URL**: https://api.quantower.com/docs/TradingPlatform.BusinessLayer.VolumeAnalysisManager.html
Class VolumeAnalysisManager
Volume Analysis calculations

Namespace: TradingPlatform.BusinessLayer
Syntax
public class VolumeAnalysisManager
Methods
CalculateProfile(HistoricalData)
Calculate volume profile for each bar in History Data

Declaration
public IVolumeAnalysisCalculationProgress CalculateProfile(HistoricalData historicalData)
Parameters
Type	Name	Description
HistoricalData	historicalData	
Returns
Type	Description
IVolumeAnalysisCalculationProgress	
CalculateProfile(HistoricalData, VolumeAnalysisCalculationParameters)
Calculate volume profile for each bar in History Data

Declaration
public IVolumeAnalysisCalculationProgress CalculateProfile(HistoricalData historicalData, VolumeAnalysisCalculationParameters calculationParameters)
Parameters
Type	Name	Description
HistoricalData	historicalData	
VolumeAnalysisCalculationParameters	calculationParameters	
Returns
Type	Description
IVolumeAnalysisCalculationProgress	
CalculateProfile(Symbol, DateTime, DateTime)
Calculate volume profile for requested time range

Declaration
public IVolumeAnalysisCalculationTask CalculateProfile(Symbol symbol, DateTime from, DateTime to)
Parameters
Type	Name	Description
Symbol	symbol	
DateTime	from	
DateTime	to	
Returns
Type	Description
IVolumeAnalysisCalculationTask	
CalculateProfile(VolumeAnalysisCalculationRequest)
Calculate volume profile for requested time range

Declaration
public IVolumeAnalysisCalculationTask CalculateProfile(VolumeAnalysisCalculationRequest request)
Parameters
Type	Name	Description
VolumeAnalysisCalculationRequest	request	
Returns
Type	Description
IVolumeAnalysisCalculationTask