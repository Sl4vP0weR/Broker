namespace Broker.Application.RequestHandlers;

public class FindBestStrategyHandler : IRequestHandler<FindBestStrategy_Request, FindBestStrategy_Response>
{
    private readonly IExchangeRatesProvider provider;
    private readonly ApplicationSettings settings;

    public FindBestStrategyHandler(
        IExchangeRatesProvider provider, 
        IOptions<ApplicationSettings> settings)
    {
        this.provider = provider;
        this.settings = settings.Value;
    }

    public async Task<FindBestStrategy_Response> Handle(FindBestStrategy_Request request, CancellationToken cancellationToken)
    {
        var start = DateOnly.Parse(request.StartDate);
        var end = DateOnly.Parse(request.EndDate);

        var days = await provider.GetOrCacheAll(start, end);

        var searcher = new StrategySearcher(request.MoneyUSD, days, 1);

        var best = FindBest(searcher);
        var response = best.Adapt<FindBestStrategy_Response>(); // map strategy to response

        response.Revenue -= request.MoneyUSD;

        return response;
    }

    private Strategy FindBest(StrategySearcher searcher)
    {
        var strategies = new List<Strategy>();

        foreach (var tool in settings.SupportedCurrencies)
        {
            var response = searcher.FindBest(tool);
            if (!response.TryPickT0(out var strategy, out _)) continue;

            strategies.Add(strategy);
        }

        if (!strategies.Any()) return default;

        return strategies.MaxBy(x => x.TotalRevenue);
    }
}