namespace Broker.Application.Interfaces;

public interface IExchangeRatesProvider
{
    Task<ExchangeRates> GetOrCache(DateOnly date);
    Task<IReadOnlyList<ExchangeRates>> GetOrCacheAll(DateOnly from, DateOnly to);
}