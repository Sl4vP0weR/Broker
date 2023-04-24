namespace Broker.Application.Interfaces;

public interface IExchangeRatesProvider
{
    Task<OneOf<ExchangeRates, Exception>> GetOrCache(DateOnly date);
    Task<IReadOnlyList<ExchangeRates>> GetOrCacheAll(DateOnly from, DateOnly to);
}