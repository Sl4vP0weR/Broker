﻿namespace Broker.Application.Validators;

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
            .Must(x => DateParse(x, out var date) && date >= settings.MinDate)
            .WithMessage(string.Format(StartDateInvalid, settings.AvailablePeriodDays));

        RuleFor(x => x.EndDate)
            .Must(x => DateParse(x, out var date) && date <= settings.MaxDate)
            .WithMessage(EndDateInvalid);

        RuleFor(x => x.MoneyUSD).GreaterThan(0m);
    }

    private bool DateParse(string value, out DateOnly date) => DateOnly.TryParse(value, out date);
    private bool DateParsable(string value) => DateParse(value, out _);
}