# ConnectionInfo Class

Paste official Quantower documentation here.

**URL**: https://api.quantower.com/docs/TradingPlatform.BusinessLayer.ConnectionInfo.html
Class ConnectionInfo
Represents all needed parameters for the connection constructing process.

Namespace: TradingPlatform.BusinessLayer
Syntax
public sealed class ConnectionInfo
Properties
AllowCreateCustomConnections
Represents all needed parameters for the connection constructing process.

Declaration
public bool AllowCreateCustomConnections { get; }
Property Value
Type	Description
bool	
ConnectionId
Gets connection Id

Declaration
public string ConnectionId { get; }
Property Value
Type	Description
string	
ConnectionLogoPath
Represents all needed parameters for the connection constructing process.

Declaration
public string ConnectionLogoPath { get; }
Property Value
Type	Description
string	
ConnectionState
Gets ConnectionState

Declaration
public ConnectionState ConnectionState { get; }
Property Value
Type	Description
ConnectionState	
Copyrights
Represents all needed parameters for the connection constructing process.

Declaration
public string Copyrights { get; }
Property Value
Type	Description
string	
CreationType
Specifies how connection was created: by default or by user

Declaration
public ConnectionCreationType CreationType { get; }
Property Value
Type	Description
ConnectionCreationType	
Group
Gets connection group

Declaration
public string Group { get; }
Property Value
Type	Description
string	
IsFavourite
Favorites one will be displayed in Control center toolbar

Declaration
public bool IsFavourite { get; set; }
Property Value
Type	Description
bool	
Links
Represents all needed parameters for the connection constructing process.

Declaration
public List<ConnectionInfoLink> Links { get; }
Property Value
Type	Description
List<ConnectionInfoLink>	
Name
Gets a user friendly name of the connection

Declaration
public string Name { get; }
Property Value
Type	Description
string	
Settings
ICustomizable realization

Declaration
public IList<SettingItem> Settings { get; set; }
Property Value
Type	Description
IList<SettingItem>	
SyncMsgProcessing
Represents all needed parameters for the connection constructing process.

Declaration
public bool SyncMsgProcessing { get; set; }
Property Value
Type	Description
bool	
VendorInfo
Represents all needed parameters for the connection constructing process.

Declaration
public VendorInfo VendorInfo { get; }
Property Value
Type	Description
VendorInfo	
VendorName
Gets vendor's name

Declaration
public string VendorName { get; }
Property Value
Type	Description
string	
VendorSettings
Gets vendor's settings

Declaration
public IList<SettingItem> VendorSettings { get; }
Property Value
Type	Description
IList<SettingItem>	
Methods
CompareTo(object)
Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.

Declaration
public int CompareTo(object obj)
Parameters
Type	Name	Description
object	obj	
An object to compare with this instance.

Returns
Type	Description
int	
A value that indicates the relative order of the objects being compared. The return value has these meanings:

Value	Meaning
Less than zero	This instance precedes obj in the sort order.
Zero	This instance occurs in the same position in the sort order as obj.
Greater than zero	This instance follows obj in the sort order.
Exceptions
Type	Condition
ArgumentException	
obj is not the same type as this instance.

CompareTo(ConnectionInfo)
Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.

Declaration
public int CompareTo(ConnectionInfo other)
Parameters
Type	Name	Description
ConnectionInfo	other	
An object to compare with this instance.

Returns
Type	Description
int	
A value that indicates the relative order of the objects being compared. The return value has these meanings:

Value	Meaning
Less than zero	This instance precedes other in the sort order.
Zero	This instance occurs in the same position in the sort order as other.
Greater than zero	This instance follows other in the sort order.
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