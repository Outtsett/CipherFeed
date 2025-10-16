# Account Class

Paste official Quantower documentation here.

**URL**: https://api.quantower.com/docs/TradingPlatform.BusinessLayer.Account.html
Class Account
Contains all user's account information

Namespace: TradingPlatform.BusinessLayer
Syntax
public class Account : BusinessObject
Properties
AccountCurrency
Gets base currency of account. Account CCY is always equal to the server CCY in AlgoStudio

Declaration
public Asset AccountCurrency { get; }
Property Value
Type	Description
Asset	
Balance
Gets current balance of the account.

Declaration
public double Balance { get; }
Property Value
Type	Description
double	
Id
Gets account unique code.

Declaration
public string Id { get; }
Property Value
Type	Description
string	
Name
Obtaining account name.

Declaration
public string Name { get; }
Property Value
Type	Description
string	
NettingType
Contains all user's account information

Declaration
public NettingType NettingType { get; set; }
Property Value
Type	Description
NettingType	
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

CompareTo(Account)
Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.

Declaration
public int CompareTo(Account other)
Parameters
Type	Name	Description
Account	other	
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
Equals(Account)
Indicates whether the current object is equal to another object of the same type.

Declaration
public bool Equals(Account other)
Parameters
Type	Name	Description
Account	other	
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
Events
Updated
Will be triggered on each account information updating

Declaration
public event Action<Account> Updated
Event Type
Type	Description
Action<Account>	