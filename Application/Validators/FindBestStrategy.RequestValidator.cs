namespace Broker.Application.Validators;

public class FindBestStrategy_RequestValidator : AbstractValidator<FindBestStrategy_Request>
{
    public FindBestStrategy_RequestValidator() { }
    public FindBestStrategy_RequestValidator(ApplicationSettings settings)
    {
        var wrongFormat = "Wrong date format.";

        RuleFor(x => x.StartDate)
            .Must(DateOnlyParsable)
            .WithMessage(wrongFormat);
        RuleFor(x => x.EndDate)
            .Must(DateOnlyParsable)
            .WithMessage(wrongFormat);

        RuleFor(x => x.StartDate)
            .Must(x => DateOnly.Parse(x) >= settings.MinDate)
            .WithMessage($"Not in the available period of {settings.AvailablePeriodDays} days.");
        RuleFor(x => x.EndDate)
            .Must(x => DateOnly.Parse(x) <= settings.MaxDate)
            .WithMessage("Not in the available period.");

        RuleFor(x => x.MoneyUSD).GreaterThan(0m);
    }
    private bool DateOnlyParsable(string value) => DateOnly.TryParse(value, out _);
}