namespace Broker.Tests;

public class RatesTest
{
    public const string 
        DefaultBase = "USD",
        DefaultTool = "RUB";

    [Fact]
    public void FindBest_Random()
    {
        Random random = new (42);

        var days = new List<ExchangeRates>();
        for (int i = 1; i <= 30; i++)
            days.Add(ExchangeRates.GetRandom(random, new(2023, 4, i)));

        var searcher = new StrategySearcher(100, days);
        var bestData = searcher.FindBest(DefaultTool);

        Assert.True(bestData.TryPickT0(out var best, out _));

        Assert.True(best.BuyAt.Date < best.SellAt.Date);
        Assert.Equal(140.06m, best.TotalRevenue, 2);
    }

    [Fact]
    public void FindBest_FirstExample()
    {
        var values = new List<decimal>()
        {
            60.17m,
            72.99m,
            66.01m,
            61.44m,
            59.79m,
            59.79m,
            59.79m,
            54.78m,
            54.80m
        };

        var days = values
            .Select((x, i) =>
                new ExchangeRates(DefaultBase, new(2014, 12, 15 + i), new() { 
                    [DefaultTool] = values[i] 
                })
            ).ToList();

        var searcher = new StrategySearcher(100, days);

        var bestData = searcher.FindBest(DefaultTool);
        Assert.True(bestData.TryPickT0(out var best, out _));

        Assert.Equal(127.24m, best.TotalRevenue, 2);
    }

    [Fact]
    public void FindBest_SecondExample()
    {
        var days = new List<ExchangeRates>()
        {
            new (DefaultBase, new(2014, 12, 5), new() { 
                    [DefaultTool] = 40 
                }),
            new (DefaultBase, new(2014, 12, 7), new() { 
                    [DefaultTool] = 35 
                }),
            new (DefaultBase, new(2014, 12, 19), new() { 
                    [DefaultTool] = 30 
                })
        };

        var searcher = new StrategySearcher(50, days);

        var bestData = searcher.FindBest(DefaultTool);
        Assert.True(bestData.TryPickT0(out var best, out _));

        Assert.Equal(55.1m, best.TotalRevenue, 1);
    }
}