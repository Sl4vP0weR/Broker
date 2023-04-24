namespace Broker.Common;

public static class Extensions
{
    public static decimal NextDecimal(this Random random, decimal min, decimal max, int precision = 2)
    {
        var randomNumber = (decimal)(Math.Round(random.NextDouble(), precision));
        return randomNumber * (max - min) + min;
    }

    public static int DaysSince(this DateOnly from, DateOnly to) =>
        (to.DayNumber - from.DayNumber);
}