namespace Broker.Application.Interfaces;

public interface IExchangeRatesAPI
{
    Task<Or<ExchangeRates, Exception>> Get(string @base, DateOnly date, IReadOnlyList<string> currencies);
}