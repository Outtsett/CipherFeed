# ReportRequestParameters Class

Paste official Quantower documentation here.

**URL**: https://api.quantower.com/docs/TradingPlatform.BusinessLayer.ReportRequestParameters.html
Class ReportRequestParameters
Namespace: TradingPlatform.BusinessLayer
Syntax
public class ReportRequestParameters : ProgressRequestParameters<float>
Constructors
ReportRequestParameters()
Declaration
public ReportRequestParameters()
ReportRequestParameters(ReportRequestParameters)
Declaration
public ReportRequestParameters(ReportRequestParameters origin)
Parameters
Type	Name	Description
ReportRequestParameters	origin	
Properties
ReportType
Declaration
public ReportType ReportType { get; set; }
Property Value
Type	Description
ReportType	
Type
Declaration
public override RequestType Type { get; }
Property Value
Type	Description
RequestType	
Overrides
RequestParameters.Type