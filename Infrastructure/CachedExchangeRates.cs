namespace Broker.Infrastructure;

internal class CachedExchangeRates
{
    public string Base { get; set; }
    public ExchangeRatesTable Rates { get; set; }
    public DateOnly Date { get; set; }
}
