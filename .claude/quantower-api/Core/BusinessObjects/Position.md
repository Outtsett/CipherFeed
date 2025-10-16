# Position Class

Paste official Quantower documentation here.

**URL**: https://api.quantower.com/docs/TradingPlatform.BusinessLayer.Position.html
Class Position
Represents trading information about related position

Namespace: TradingPlatform.BusinessLayer
Syntax
public class Position : TradingObject
Properties
CurrentPrice
The market price obtainable from your broker.

Declaration
public double CurrentPrice { get; }
Property Value
Type	Description
double	
Fee
Gets fee amount for the position.

Declaration
public PnLItem Fee { get; }
Property Value
Type	Description
PnLItem	
GrossPnL
Gets Profit/loss (without swaps or commissions) all calculated based on the current broker's price. For open position it shows the profit/loss you would make if you close the position at the current price. If position closed, this parameter show profit/loss what trader have after closing this position.

Declaration
public PnLItem GrossPnL { get; }
Property Value
Type	Description
PnLItem	
GrossPnLTicks
Returns ticks amount between open and current price.

Declaration
public double GrossPnLTicks { get; }
Property Value
Type	Description
double	
LiquidationPrice
Represents trading information about related position

Declaration
public double LiquidationPrice { get; }
Property Value
Type	Description
double	
NetPnL
Gets Profit/loss calculated based on the current broker's price. For open position it shows the profit/loss you would make if you close the position at the current price. If position closed, this parameter show profit/loss what trader have after closing this position.

Declaration
public PnLItem NetPnL { get; }
Property Value
Type	Description
PnLItem	
OpenPrice
Gets position open order price

Declaration
public double OpenPrice { get; }
Property Value
Type	Description
double	
OpenTime
Gets position openning time

Declaration
public DateTime OpenTime { get; }
Property Value
Type	Description
DateTime	
Quantity
Gets position quantity value

Declaration
public double Quantity { get; }
Property Value
Type	Description
double	
StopLoss
Gets StopLoss order which belongs to the position

Declaration
public Order StopLoss { get; }
Property Value
Type	Description
Order	
Swaps
Gets PnL swaps

Declaration
public PnLItem Swaps { get; }
Property Value
Type	Description
PnLItem	
TakeProfit
Gets TakeProfit order which belongs to the position

Declaration
public Order TakeProfit { get; }
Property Value
Type	Description
Order	
Methods
BuildMessage()
Represents trading information about related position

Declaration
public MessageOpenPosition BuildMessage()
Returns
Type	Description
MessageOpenPosition	
Close(double)
Closes position if quantity is not specified else - uses partial closing operation.

Declaration
public virtual TradingOperationResult Close(double closeQuantity = -1)
Parameters
Type	Name	Description
double	closeQuantity	
Returns
Type	Description
TradingOperationResult	
ForceRecalculatePnl()
Represents trading information about related position

Declaration
public void ForceRecalculatePnl()
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
Updated
Will be triggered on each UpdateByMessage(MessageOpenPosition) and UpdatePnl(PnL) invocation

Declaration
public event Action<Position> Updated
Event Type
Type	Description
Action<Position>	
