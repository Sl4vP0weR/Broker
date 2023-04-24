namespace Broker.Infrastructure.Interfaces;

public interface IExchangeRatesCache
{
    Task Set(ExchangeRates rates);
    Task<OneOf<ExchangeRates, None>> Get(DateOnly date);
    Task<IReadOnlyList<ExchangeRates>> GetAll(DateOnly from, DateOnly to);
    Task<bool> Exists(DateOnly date);
}
