namespace Broker.Application;

public record struct StrategySearcher(decimal Current, IReadOnlyList<ExchangeRates> Days, decimal Fees = 1)
{
    public OneOf<Strategy, None> FindBest(string tool)
    {
        ExchangeRate max = default, min = default;
        var days = Days.OrderBy(x => x.Date).Select(x => x.GetBy(tool)).ToList();
        var totalRevenue = Current;

        // O((n^2)/2), may be done better?
        for (int i = 0; i < days.Count; i++)
        {
            var maxTemp = days[i];
            for (int j = i + 1; j < days.Count; j++)
            {
                var minTemp = days[j];

                var revenue = CalculateTotalRevenue(minTemp, maxTemp);
                if (revenue <= totalRevenue)
                    continue;

                max = maxTemp;
                min = minTemp;
                totalRevenue = revenue;
            }
        }
        if (!max.IsValid || !min.IsValid) return new None();

        return new Strategy(tool, max, min, totalRevenue);
    }

    public decimal CalculateTotalRevenue(ExchangeRate min, ExchangeRate max, decimal? fees = null) =>
        ((max * Current) / min) - (fees ?? GetFees(min, max));

    public decimal GetFees(ExchangeRate min, ExchangeRate max) =>
        Math.Max(0, max.Date.DaysSince(min.Date) * Fees);
}
