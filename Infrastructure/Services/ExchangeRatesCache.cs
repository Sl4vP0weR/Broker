namespace Broker.Infrastructure.Services;

public class ExchangeRatesCache : IExchangeRatesCache
{
    private readonly IRedisClient client;
    private readonly IRedisDatabase database;

    public ExchangeRatesCache(IRedisClient client)
    {
        this.client = client;
        this.database = client.Db0;
    }

    public Task Set(ExchangeRates rates)
    {
        var key = rates.Date.ToString();

        return database.ReplaceAsync(key, rates.Adapt<CachedExchangeRates>());
    }

    public async Task<ExchangeRates?> Get(DateOnly date)
    {
        var key = date.ToString();

        var data = await database.GetAsync<CachedExchangeRates>(key);
        if (data is null) return null;

        return data.Adapt<ExchangeRates>();
    }

    public async Task<IReadOnlyList<ExchangeRates>> GetAll(DateOnly from, DateOnly to)
    {
        var keys = new HashSet<string>();

        var daysCount = from.DaysSince(to);
        for (int i = 0; i <= daysCount; i++)
        {
            var date = from.AddDays(i);
            keys.Add(date.ToString());
        }

        var cachedRates = await database.GetAllAsync<CachedExchangeRates>(keys);
        var rates = cachedRates.Select(x => x.Value.Adapt<ExchangeRates>()).ToList();

        return rates;
    }

    public Task<bool> Exists(DateOnly date)
    {
        var key = date.ToString();
        return database.ExistsAsync(key);
    }
}