namespace Broker.Domain;

[DebuggerDisplay("[{Date}] {Currency} - {Value}")]
public record struct ExchangeRate(string Currency = null, decimal Value = 0, DateOnly Date = default)
{
    public static implicit operator decimal(ExchangeRate @this) => @this.Value;

    [JsonIgnore] 
    public readonly bool IsValid => !string.IsNullOrWhiteSpace(Currency);
}   