namespace Broker.Infrastructure.Services;

public class FixerExchangeRatesAPI : IExchangeRatesAPI
{
    private readonly string apiKey;
    private readonly RestClientOptions restOptions;
    public const string
        APIKeyHeader = "apikey",
        DateFormat = "yyyy'-'MM'-'dd";

    public FixerExchangeRatesAPI(IConfiguration configuration)
    {
        apiKey = configuration["FixerAPI:Key"];
        restOptions = new("https://api.apilayer.com/fixer/");
    }

    /// <exception cref="ArgumentOutOfRangeException"/>
    /// <exception cref="WebException"/>
    /// <exception cref="ParsingException"/>
    public async Task<ExchangeRates> Get(string @base, DateOnly date, IReadOnlyList<string> currencies)
    {
        if (currencies.Count == 0)
            throw new ArgumentOutOfRangeException(nameof(currencies));

        var symbols = string.Join(",", currencies);

        var request = new RestRequest(date.ToString(DateFormat));
        request
            .AddQueryParameter(nameof(@base), @base)
            .AddQueryParameter(nameof(symbols), symbols);

        var response = await SendRequest(request);
        var rates = ParseRates(response);

        return new ExchangeRates(@base, date, rates);
    }

    private static ExchangeRatesTable ParseRates(string responseBody)
    {
        try
        {
            var jsonNode = JsonNode.Parse(responseBody).AsObject();
            var rates = jsonNode["rates"].Deserialize<ExchangeRatesTable>();
            return rates;
        }
        catch { throw new ParsingException(); }
    }

    private async Task<string> SendRequest(RestRequest request)
    {
        using RestClient client = new(restOptions);
        client.AddDefaultHeader(APIKeyHeader, apiKey);

        var response = await client.ExecuteAsync(request);
        return response.Content;
    }
}