namespace Broker.Domain;

public class ExchangeRatesTable : Dictionary<string, decimal> 
{
    public ExchangeRatesTable() { }
    public ExchangeRatesTable(IDictionary<string, decimal> innerDictionary) : base(innerDictionary) { }
    public new void Add(string currency, decimal rate) =>
        base.Add(currency.ToUpper(), rate);
}