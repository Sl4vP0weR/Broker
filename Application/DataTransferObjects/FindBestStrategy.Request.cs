namespace Broker.Application.DataTransferObjects;

public record FindBestStrategy_Request(string StartDate, string EndDate, decimal MoneyUSD) : IRequest, IRequest<FindBestStrategy_Response>;