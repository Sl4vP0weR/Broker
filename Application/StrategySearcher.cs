namespace Broker.Application;

public record struct StrategySearcher(decimal Current, IReadOnlyList<ExchangeRates> Days, decimal Fees = 1)
{
    public OneOf<Strategy, None> FindBest(string tool)
    {
        ExchangeRate max = default, min = default;
        var days = Days.Select(x => x.GetBy(tool)).OrderBy(x => x.Date).ToList();
        var ordered = days.OrderBy(x => x.Value).ToList();
        var totalRevenue = Current;

        // O(n^2), may be done better?
        for (int i = ordered.Count - 1; i > 0; i--)
        {
            var maxTemp = ordered[i];
            for (int j = 0; j < ordered.Count; j++)
            {
                var minTemp = ordered[j];

                if (maxTemp.Date >= minTemp.Date)
                    continue;

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

    public decimal GetFees(ExchangeRate from, ExchangeRate to) =>
        Math.Max(0, from.Date.DaysPast(to.Date) * Fees);
}
