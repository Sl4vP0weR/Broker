namespace Broker.Infrastructure.Services;

public class FixerExchangeRatesAPI : IExchangeRatesAPI
{
    private readonly string apiKey;

    public FixerExchangeRatesAPI(IConfiguration configuration)
    {
        apiKey = configuration["FixerAPI:Key"];
    }

    public async Task<OneOf<ExchangeRates, Exception>> Get(string @base, DateOnly date, IReadOnlyList<string> currencies)
    {
        if (currencies.Count == 0)
            return new ArgumentOutOfRangeException(nameof(currencies));

        var symbols = string.Join(",", currencies);
        var url = $"https://api.apilayer.com/fixer/{date:yyyy'-'MM'-'dd}?base={@base}&symbols={symbols}";

        try
        {
            var response = await SendRequest(url);
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

    private async Task<OneOf<string, WebException>> SendRequest(string url)
    {
        string response;
        try
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("apikey", apiKey);

            response = await client.GetStringAsync(url);
        }
        catch (WebException ex) { return ex; }

        return response;
    }
}