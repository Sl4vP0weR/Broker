﻿namespace Broker.Application.Interfaces;

public interface IExchangeRatesAPI
{
    Task<OneOf<ExchangeRates, Exception>> Get(string @base, DateOnly date, IReadOnlyList<string> currencies);
}