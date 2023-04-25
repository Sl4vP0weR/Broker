namespace Broker.Application.Interfaces;

public interface IExchangeRatesCache
{
    Task Set(ExchangeRates rates);
    Task<ExchangeRates?> Get(DateOnly date);
    Task<IReadOnlyList<ExchangeRates>> GetAll(DateOnly from, DateOnly to);
    Task<bool> Exists(DateOnly date);
}
