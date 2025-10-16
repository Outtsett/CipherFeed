# Exchange Class

Paste official Quantower documentation here.

**URL**: https://api.quantower.com/docs/TradingPlatform.BusinessLayer.Exchange.html
Class Exchange
Contains all information which belong to the given exchange

Namespace: TradingPlatform.BusinessLayer
Syntax
public sealed class Exchange : BusinessObject
Properties
ComplexId
Contains all information which belong to the given exchange

Declaration
public ExchangeComplexIdentifier ComplexId { get; }
Property Value
Type	Description
ExchangeComplexIdentifier	
ExchangeName
Gets Exchange name

Declaration
public string ExchangeName { get; }
Property Value
Type	Description
string	
Id
Gets Exchange Id

Declaration
public string Id { get; }
Property Value
Type	Description
string	
SortIndex
Used for the Exchanges comparing

Declaration
public int SortIndex { get; }
Property Value
Type	Description
int	
Methods
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