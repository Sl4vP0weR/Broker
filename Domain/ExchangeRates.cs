namespace Broker.Domain;

[DebuggerDisplay("[{Date}] {Base}")]
public struct ExchangeRates
{
    private readonly string @base;
    public string Base => @base;

    [JsonInclude, JsonPropertyName("rates")] 
    public IReadOnlyDictionary<string, decimal> InnerRates { get; }

    [JsonIgnore]
    public IReadOnlyDictionary<string, ExchangeRate> Rates { get; }

    [JsonInclude] 
    public readonly DateOnly Date;

    public readonly ExchangeRate GetBy(string currency) => Rates.TryGetValue(currency.ToUpper(), out var rate) ? rate : default;

    public ExchangeRates(string baseCurrency, DateOnly date, ExchangeRatesTable rates)
    {
        @base = baseCurrency;
        Date = date;
        InnerRates = rates.AsReadOnly();
        Rates = rates.ToDictionary(
                x => x.Key.ToUpper(),
                x => new ExchangeRate(x.Key, x.Value, date)
            ).AsReadOnly();
    }

    public static ExchangeRates GetRandom(Random random, DateOnly date) =>
        new("USD", date, new()
        {
            ["RUB"] = random.NextDecimal(50m, 75m),
            ["JPY"] = random.NextDecimal(10m, 13m),
            ["EUR"] = random.NextDecimal(1m, 1.5m),
            ["GBP"] = random.NextDecimal(1.3m, 1.6m)
        }
    );
}