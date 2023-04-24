namespace Broker.Infrastructure.Services;

public class ExchangeRatesCache : IExchangeRatesCache
{
    private readonly IRedisDatabase database;
    public ExchangeRatesCache(IRedisClient client)
    {
        this.database = client.Db0;
    }

    public Task Set(ExchangeRates rates)
    {
        var key = rates.Date.ToString();

        return database.ReplaceAsync(key, rates.Adapt<CachedExchangeRates>());
    }

    public async Task<OneOf<ExchangeRates, None>> Get(DateOnly date)
    {
        var key = date.ToString();
        var data = await database.GetAsync<CachedExchangeRates>(key);
        if (data is null) return default;

        return data.Adapt<ExchangeRates>();
    }

    public async Task<bool> Exists(DateOnly date)
    {
        var key = date.ToString();
        if (await database.ExistsAsync(key))
            return true;

        var data = await Get(date);
        return data.IsT0;
    }
        

    public async Task<IReadOnlyList<ExchangeRates>> GetAll(DateOnly from, DateOnly to)
    {
        HashSet<string> keys = new();
        var daysPast = to.DaysPast(from);
        for (int i = 0; i <= daysPast; i++)
        {
            var date = from.AddDays(i);
            keys.Add(date.ToString());
        }

        var dict = await database.GetAllAsync<CachedExchangeRates>(keys);
        var rates = dict.Select(x => x.Value.Adapt<ExchangeRates>()).ToList();
        return rates;
    }
}