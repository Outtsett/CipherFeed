# Connection Class

Paste official Quantower documentation here.

**URL**: https://api.quantower.com/docs/TradingPlatform.BusinessLayer.Connection.html
Class Connection
Represents information about connection and provides an access to the current trading information(Symbols, Orders, Position, Accounts etc.).

Namespace: TradingPlatform.BusinessLayer
Syntax
public sealed class Connection
Properties
BusinessObjects
Provides access to all business objects which are belong to this connection

Declaration
public IBusinessObjectsProvider BusinessObjects { get; }
Property Value
Type	Description
IBusinessObjectsProvider	
ConnectingProgress
Represents information about connection and provides an access to the current trading information(Symbols, Orders, Position, Accounts etc.).

Declaration
public string ConnectingProgress { get; }
Property Value
Type	Description
string	
DateTimeUtcNow
Represents information about connection and provides an access to the current trading information(Symbols, Orders, Position, Accounts etc.).

Declaration
public DateTime DateTimeUtcNow { get; }
Property Value
Type	Description
DateTime	
HistoryMetaData
Gets a matched available metadata info with the vendor's side

Declaration
public HistoryMetadata HistoryMetaData { get; }
Property Value
Type	Description
HistoryMetadata	
Id
Gets connection Id

Declaration
public string Id { get; }
Property Value
Type	Description
string	
Info
Represents information about connection and provides an access to the current trading information(Symbols, Orders, Position, Accounts etc.).

Declaration
public ConnectionInfo Info { get; }
Property Value
Type	Description
ConnectionInfo	
LastConnectionResult
Represents information about connection and provides an access to the current trading information(Symbols, Orders, Position, Accounts etc.).

Declaration
public ConnectionResult LastConnectionResult { get; }
Property Value
Type	Description
ConnectionResult	
MessagesQueueDepth
Messages count that one is waited to process

Declaration
public int MessagesQueueDepth { get; }
Property Value
Type	Description
int	
Name
Gets connection Name

Declaration
public string Name { get; set; }
Property Value
Type	Description
string	
NewsFeedSettings
Represents information about connection and provides an access to the current trading information(Symbols, Orders, Position, Accounts etc.).

Declaration
public IEnumerable<SettingItem> NewsFeedSettings { get; }
Property Value
Type	Description
IEnumerable<SettingItem>	
PingTime
Represents connection ping time

Declaration
public TimeSpan? PingTime { get; }
Property Value
Type	Description
TimeSpan?	
RoundTripTime
Represents information about connection and provides an access to the current trading information(Symbols, Orders, Position, Accounts etc.).

Declaration
public TimeSpan? RoundTripTime { get; }
Property Value
Type	Description
TimeSpan?	
ServerTime
Represents information about connection and provides an access to the current trading information(Symbols, Orders, Position, Accounts etc.).

Declaration
public DateTime ServerTime { get; }
Property Value
Type	Description
DateTime	
Settings
Contains list of connection settings. Will be reused on each population time.

Declaration
public IList<SettingItem> Settings { get; set; }
Property Value
Type	Description
IList<SettingItem>	
State
Gets connection's state (Connected/Connecting/Fail etc.)

Declaration
public ConnectionState State { get; }
Property Value
Type	Description
ConnectionState	
TotalSubscriptionsCount
Represents information about connection and provides an access to the current trading information(Symbols, Orders, Position, Accounts etc.).

Declaration
public int TotalSubscriptionsCount { get; }
Property Value
Type	Description
int	
TradesHistoryMetadata
Represents information about connection and provides an access to the current trading information(Symbols, Orders, Position, Accounts etc.).

Declaration
public TradesHistoryMetadata TradesHistoryMetadata { get; }
Property Value
Type	Description
TradesHistoryMetadata	
Type
Defines connection type

Declaration
public ConnectionType Type { get; set; }
Property Value
Type	Description
ConnectionType	
Uptime
Represents information about connection and provides an access to the current trading information(Symbols, Orders, Position, Accounts etc.).

Declaration
public TimeSpan Uptime { get; }
Property Value
Type	Description
TimeSpan	
VendorName
Gets connection's vendor name

Declaration
public string VendorName { get; }
Property Value
Type	Description
string	
VolumeAnalysisMetadata
Represents information about connection and provides an access to the current trading information(Symbols, Orders, Position, Accounts etc.).

Declaration
public VolumeAnalysisMetadata VolumeAnalysisMetadata { get; }
Property Value
Type	Description
VolumeAnalysisMetadata	
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

Connect()
Establishes a connection to a specified vendor

Declaration
public ConnectionResult Connect()
Returns
Type	Description
ConnectionResult	
Disconnect()
Closes a connection.

Declaration
public void Disconnect()
GetNews(GetNewsRequestParameters)
Represents information about connection and provides an access to the current trading information(Symbols, Orders, Position, Accounts etc.).

Declaration
public IEnumerable<NewsArticle> GetNews(GetNewsRequestParameters requestParameters)
Parameters
Type	Name	Description
GetNewsRequestParameters	requestParameters	
Returns
Type	Description
IEnumerable<NewsArticle>	
GetNewsArticleContent(GetNewsArticleContentRequestParameters)
Represents information about connection and provides an access to the current trading information(Symbols, Orders, Position, Accounts etc.).

Declaration
public string GetNewsArticleContent(GetNewsArticleContentRequestParameters requestParameters)
Parameters
Type	Name	Description
GetNewsArticleContentRequestParameters	requestParameters	
Returns
Type	Description
string	
GetOrdersHistory(OrdersHistoryRequestParameters)
Represents information about connection and provides an access to the current trading information(Symbols, Orders, Position, Accounts etc.).

Declaration
public IList<OrderHistory> GetOrdersHistory(OrdersHistoryRequestParameters parameters)
Parameters
Type	Name	Description
OrdersHistoryRequestParameters	parameters	
Returns
Type	Description
IList<OrderHistory>	
GetTrades(TradesHistoryRequestParameters)
Represents information about connection and provides an access to the current trading information(Symbols, Orders, Position, Accounts etc.).

Declaration
public IList<Trade> GetTrades(TradesHistoryRequestParameters parameters)
Parameters
Type	Name	Description
TradesHistoryRequestParameters	parameters	
Returns
Type	Description
IList<Trade>	
SendCustomRequest(RequestParameters)
Represents information about connection and provides an access to the current trading information(Symbols, Orders, Position, Accounts etc.).

Declaration
public void SendCustomRequest(RequestParameters parameters)
Parameters
Type	Name	Description
RequestParameters	parameters	
SubscribeNewsUpdates(SubscribeNewsRequestParameters, Action<NewsArticle>)
Represents information about connection and provides an access to the current trading information(Symbols, Orders, Position, Accounts etc.).

Declaration
public void SubscribeNewsUpdates(SubscribeNewsRequestParameters subscribeNewsRequestParameters, Action<NewsArticle> updateAction)
Parameters
Type	Name	Description
SubscribeNewsRequestParameters	subscribeNewsRequestParameters	
Action<NewsArticle>	updateAction	
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
UnsubscribeNewsUpdates(SubscribeNewsRequestParameters, Action<NewsArticle>)
Represents information about connection and provides an access to the current trading information(Symbols, Orders, Position, Accounts etc.).

Declaration
public void UnsubscribeNewsUpdates(SubscribeNewsRequestParameters subscribeNewsRequestParameters, Action<NewsArticle> updateAction)
Parameters
Type	Name	Description
SubscribeNewsRequestParameters	subscribeNewsRequestParameters	
Action<NewsArticle>	updateAction	
Events
ConnectingProgressChanged
Will be triggered when ConnectingProgress changed.

Declaration
public event EventHandler<ConnectionConnectingProgressChangedEventArgs> ConnectingProgressChanged
Event Type
Type	Description
EventHandler<ConnectionConnectingProgressChangedEventArgs>	
NewPerformedRequest
Represents information about connection and provides an access to the current trading information(Symbols, Orders, Position, Accounts etc.).

Declaration
public event EventHandler<PerformedRequestEventArgs> NewPerformedRequest
Event Type
Type	Description
EventHandler<PerformedRequestEventArgs>	
NewRequest
Represents information about connection and provides an access to the current trading information(Symbols, Orders, Position, Accounts etc.).

Declaration
public event EventHandler<RequestEventArgs> NewRequest
Event Type
Type	Description
EventHandler<RequestEventArgs>	
StateChanged
Will be triggered when State changed.

Declaration
public event EventHandler<ConnectionStateChangedEventArgs> StateChanged
Event Type
Type	Description
EventHandler<ConnectionStateChangedEventArgs>