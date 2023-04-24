namespace Broker.Infrastructure.Mappers;

public class CachedExchangeRatesMapper : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<ExchangeRates, CachedExchangeRates>()
            .MapWith(rates => new CachedExchangeRates
            {
                Base = rates.Base,
                Date = rates.Date,
                Rates = new(rates.InnerRates.ToDictionary(x => x.Key, x => x.Value))
            })
            .Compile();

        config.NewConfig<CachedExchangeRates, ExchangeRates>()
            .MapWith(rates => new ExchangeRates(
                rates.Base, 
                rates.Date, 
                rates.Rates
            ))
            .Compile();
    }
}