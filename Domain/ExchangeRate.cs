namespace Broker.Domain;

public record struct ExchangeRate(string Currency = null, decimal Value = 0, DateOnly Date = default)
{
    public static implicit operator decimal(ExchangeRate @this) => @this.Value;
    public static implicit operator string(ExchangeRate @this) => @this.Currency;
    [JsonIgnore] 
    public readonly bool IsValid => !string.IsNullOrWhiteSpace(Currency);
}   