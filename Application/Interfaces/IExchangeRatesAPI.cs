namespace Broker.Application.Interfaces;

public interface IExchangeRatesAPI
{
    Task<ExchangeRates> Get(string @base, DateOnly date, IReadOnlyList<string> currencies);
}