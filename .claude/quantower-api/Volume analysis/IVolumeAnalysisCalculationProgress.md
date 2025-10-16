# IVolumeAnalysisCalculationProgress Interface

Paste official Quantower documentation here.

**URL**: https://api.quantower.com/docs/TradingPlatform.BusinessLayer.IVolumeAnalysisCalculationProgress.html
Interface IVolumeAnalysisCalculationProgress
Namespace: TradingPlatform.BusinessLayer
Syntax
public interface IVolumeAnalysisCalculationProgress
Properties
CalculationParameters
Declaration
VolumeAnalysisCalculationParameters CalculationParameters { get; }
Property Value
Type	Description
VolumeAnalysisCalculationParameters	
IsAborted
Declaration
bool IsAborted { get; }
Property Value
Type	Description
bool	
ProgressBarIndex
Declaration
int ProgressBarIndex { get; }
Property Value
Type	Description
int	
ProgressPercent
Declaration
int ProgressPercent { get; }
Property Value
Type	Description
int	
State
Declaration
VolumeAnalysisCalculationState State { get; }
Property Value
Type	Description
VolumeAnalysisCalculationState	
Methods
AbortLoading()
Declaration
void AbortLoading()
Wait(CancellationToken)
Declaration
void Wait(CancellationToken token = default)
Parameters
Type	Name	Description
CancellationToken	token	
Events
ProgressChanged
Declaration
event EventHandler<VolumeAnalysisTaskEventArgs> ProgressChanged
Event Type
Type	Description
EventHandler<VolumeAnalysisTaskEventArgs>	
StateChanged
Declaration
event EventHandler<VolumeAnalysisTaskEventArgs> StateChanged
Event Type
Type	Description
EventHandler<VolumeAnalysisTaskEventArgs>