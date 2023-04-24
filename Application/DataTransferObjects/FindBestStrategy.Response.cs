namespace Broker.Application.DataTransferObjects;

public record struct FindBestStrategy_Response(string Tool, ExchangeRate BuyAt, ExchangeRate SellAt, decimal Revenue);