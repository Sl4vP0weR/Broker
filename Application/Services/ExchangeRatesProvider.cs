namespace Broker.Application.Services;

public class ExchangeRatesProvider : IExchangeRatesProvider
{
    private readonly IExchangeRatesCache cache;
    private readonly IExchangeRatesAPI api;
    private readonly ApplicationSettings settings;
    public ExchangeRatesProvider(IExchangeRatesCache cache, IExchangeRatesAPI api, IOptions<ApplicationSettings> settings)
    {
        this.cache = cache;
        this.api = api;
        this.settings = settings.Value;
    }
    public async Task<OneOf<ExchangeRates, Exception>> GetOrCache(DateOnly date)
    {
        if (await cache.Exists(date))
        {
            var data = await cache.Get(date);

            if (data.TryPickT0(out var cachedRates, out _) && cachedRates.Date == date)
                return cachedRates;
        }

        var response = await api.Get(settings.DefaultBase, date, settings.SupportedCurrencies);

        if(response.TryPickT0(out var rates, out _))
            await cache.Set(rates);

        return response;
    }

    public async Task<IReadOnlyList<ExchangeRates>> GetOrCacheAll(DateOnly from, DateOnly to)
    {
        var daysPast = to.DaysPast(from);
        List<ExchangeRates> list = new();
        for (int i = 0; i <= daysPast; i++)
        {
            var date = from.AddDays(i);

            var response = await GetOrCache(date);
            if (!response.TryPickT0(out var rates, out _)) continue;

            list.Add(rates);
        }
        return list;
    }
}