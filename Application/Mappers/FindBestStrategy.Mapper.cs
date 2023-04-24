namespace Broker.Application.Mappers;

public class FindBestStrategy_Mapper : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Strategy, FindBestStrategy_Response>()
            .Map(dest => dest.Revenue, src => src.TotalRevenue)
            .Map(dest => dest.Tool, src => src.Currency)
            .Map(dest => dest.BuyAt, src => src.BuyAt)
            .Map(dest => dest.SellAt, src => src.SellAt)
            .Compile();
    }
}