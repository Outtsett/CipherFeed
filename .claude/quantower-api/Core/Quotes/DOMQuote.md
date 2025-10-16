# DOMQuote Class

Paste official Quantower documentation here.

**URL**: https://api.quantower.com/docs/TradingPlatform.BusinessLayer.DOMQuote.html
Class DOMQuote
Represent access to DOM2 quote, which contains Bids and Asks.

Namespace: TradingPlatform.BusinessLayer
Syntax
public class DOMQuote : MessageQuote
Properties
Asks
Collection of Asks quotes

Declaration
public List<Level2Quote> Asks { get; set; }
Property Value
Type	Description
List<Level2Quote>	
Bids
Collection of Bids quotes

Declaration
public List<Level2Quote> Bids { get; set; }
Property Value
Type	Description
List<Level2Quote>