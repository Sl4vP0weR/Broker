namespace Broker.API.Controllers;

[OutputCache(PolicyName = CachePolicyName)]
[ApiController]
[Route("rates/")]
public class RatesController : ControllerBase
{
    public const string CachePolicyName = "Rates";

    private readonly IMediator mediator;
    private readonly IExchangeRatesProvider provider;
    private readonly ApplicationSettings settings;

    public RatesController(IMediator mediator, IExchangeRatesProvider provider, IOptions<ApplicationSettings> settings)
    {
        this.mediator = mediator;
        this.provider = provider;
        this.settings = settings.Value;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var response = await provider.GetOrCacheAll(settings.MinDate, settings.MaxDate);

        return Ok(response);
    }

    [HttpPost("best")]
    public async Task<IActionResult> FindBestStrategy([FromQuery] FindBestStrategy_Request request)
    {
        var validation = new FindBestStrategy_RequestValidator(settings).Validate(request);
        if(!validation.IsValid)
            return BadRequest(validation.Errors);

        var response = await mediator.Send<FindBestStrategy_Response>(request);

        return Ok(response);
    }
}