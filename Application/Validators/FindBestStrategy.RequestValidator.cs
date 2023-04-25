namespace Broker.Application.Validators;

public class FindBestStrategy_RequestValidator : AbstractValidator<FindBestStrategy_Request>
{
    public const string 
        BadDateFormatMessage = "Bad date format.",
        StartDateInvalid = "Not in the available period of {0} days.",
        EndDateInvalid = "Not in the available period.";

    public FindBestStrategy_RequestValidator(IOptions<ApplicationSettings> settings) : this(settings.Value) { }
    public FindBestStrategy_RequestValidator(ApplicationSettings settings)
    {
        RuleFor(x => x.StartDate)
            .Must(DateParsable)
            .WithMessage(BadDateFormatMessage);

        RuleFor(x => x.EndDate)
            .Must(DateParsable)
            .WithMessage(BadDateFormatMessage);

        RuleFor(x => x.StartDate)
            .Must(x => DateOnly.Parse(x) >= settings.MinDate)
            .WithMessage(string.Format(StartDateInvalid, settings.AvailablePeriodDays));

        RuleFor(x => x.EndDate)
            .Must(x => DateOnly.Parse(x) <= settings.MaxDate)
            .WithMessage(EndDateInvalid);

        RuleFor(x => x.MoneyUSD).GreaterThan(0m);
    }

    private bool DateParsable(string value) => DateOnly.TryParse(value, out _);
}