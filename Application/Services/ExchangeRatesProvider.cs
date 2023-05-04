namespace Broker.Application.Services;

public class ExchangeRatesProvider : IExchangeRatesProvider
{
    private readonly IExchangeRatesCache cache;
    private readonly IExchangeRatesAPI api;
    private readonly ApplicationSettings settings;

    public ExchangeRatesProvider(
        IExchangeRatesCache cache,
        IExchangeRatesAPI api,
        IOptions<ApplicationSettings> settings)
    {
        this.cache = cache;
        this.api = api;
        this.settings = settings.Value;
    }

    /// <summary>
    /// If cache entry exists - returns cached value, 
    /// otherwise retrieves rates from API and adds entry to the cache.
    /// </summary>
    /// <exception cref="System.Net.WebException"/>
    public async Task<ExchangeRates> GetOrCache(DateOnly date)
    {
        if (await cache.Exists(date))
        {
            var cachedData = await cache.Get(date);

            if (cachedData.TryGet(out var cachedRates))
                return cachedRates;
        }

        var response = await api.Get(settings.DefaultBase, date, settings.SupportedCurrencies);
        await cache.Set(response);

        return response;
    }

    public async Task<IReadOnlyList<ExchangeRates>> GetOrCacheAll(DateOnly from, DateOnly to)
    {
        var list = new List<ExchangeRates>();

        var daysCount = from.DaysSince(to);
        for (int i = 0; i <= daysCount; i++)
        {
            var date = from.AddDays(i);

            try
            {
                var response = await GetOrCache(date);

                list.Add(response);
            }
            catch { }
        }

        return list;
    }
}