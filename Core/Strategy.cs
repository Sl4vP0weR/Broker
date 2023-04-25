namespace Broker.Core;

public record struct Strategy(
    string Currency,
    ExchangeRate BuyAt,
    ExchangeRate SellAt,
    decimal TotalRevenue
);