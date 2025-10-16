# RequestParameters Class

Paste official Quantower documentation here.

**URL**: https://api.quantower.com/docs/TradingPlatform.BusinessLayer.RequestParameters.html
Class RequestParameters
Namespace: TradingPlatform.BusinessLayer
Syntax
public abstract class RequestParameters
Constructors
RequestParameters()
Declaration
protected RequestParameters()
RequestParameters(RequestParameters)
Declaration
protected RequestParameters(RequestParameters origin)
Parameters
Type	Name	Description
RequestParameters	origin	
Properties
CancellationToken
Declaration
public CancellationToken CancellationToken { get; set; }
Property Value
Type	Description
CancellationToken	
RequestId
Declaration
public long RequestId { get; }
Property Value
Type	Description
long	
SendingSource
Declaration
public string SendingSource { get; set; }
Property Value
Type	Description
string	
Type
Declaration
public abstract RequestType Type { get; }
Property Value
Type	Description
RequestType	
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
Equals(RequestParameters)
Indicates whether the current object is equal to another object of the same type.

Declaration
public bool Equals(RequestParameters other)
Parameters
Type	Name	Description
RequestParameters	other	
An object to compare with this object.

Returns
Type	Description
bool	
true if the current object is equal to the other parameter; otherwise, false.

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