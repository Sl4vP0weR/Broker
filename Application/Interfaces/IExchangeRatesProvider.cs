namespace Broker.Application.Interfaces;

public interface IExchangeRatesProvider
{
    Task<Or<ExchangeRates, Exception>> GetOrCache(DateOnly date);
    Task<IReadOnlyList<ExchangeRates>> GetOrCacheAll(DateOnly from, DateOnly to);
}