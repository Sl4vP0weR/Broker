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

    public async Task<OneOf<ExchangeRates, Exception>> Get(string @base, DateOnly date, IReadOnlyList<string> currencies)
    {
        if (currencies.Count == 0)
            return new ArgumentOutOfRangeException(nameof(currencies));

        var symbols = string.Join(",", currencies);
        
        var request = new RestRequest(date.ToString(DateFormat));
        request
            .AddQueryParameter(nameof(@base), @base)
            .AddQueryParameter(nameof(symbols), symbols);

        try
        {
            var response = await SendRequest(request);
            if (response.TryPickT1(out var ex, out var responseBody))
                return ex;

            var rates = ParseRates(responseBody);

            return new ExchangeRates(@base, date, rates);
        }
        catch (Exception ex) { return ex; }
    }

    private static ExchangeRatesTable ParseRates(string responseBody)
    {
        var jsonNode = JsonNode.Parse(responseBody).AsObject();
        var rates = jsonNode["rates"].Deserialize<ExchangeRatesTable>();
        return rates;
    }

    private async Task<OneOf<string, WebException>> SendRequest(RestRequest request)
    {
        string result;
        try
        {
            using var client = new RestClient(restOptions);
            client.AddDefaultHeader(APIKeyHeader, apiKey);

            var response = await client.ExecuteAsync(request);
            result = response.Content;
        }
        catch (WebException ex) { return ex; }

        return result;
    }
}