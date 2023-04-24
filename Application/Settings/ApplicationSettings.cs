namespace Broker.Application.Settings;

public class ApplicationSettings
{
    public const string SectionName = nameof(ApplicationSettings);

    public int AvailablePeriodDays { get; set; } = 60;
    public string DefaultBase { get; set; } = "USD";
    public List<string> SupportedCurrencies { get; set; } = new();

    [JsonIgnore]
    public DateOnly MinDate => MaxDate.AddDays(-AvailablePeriodDays);
    [JsonIgnore]
    public DateOnly MaxDate => DateOnly.FromDateTime(DateTime.UtcNow);
}