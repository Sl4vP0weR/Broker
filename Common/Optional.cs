namespace Broker.Common;

public record struct Optional<T>(T? Value = default)
{
    [DebuggerHidden]
    public readonly bool IsOccupied => !IsEmpty;

    public readonly bool IsEmpty => Value is null;

    public static Optional<T> Empty => new();

    public T Get() => Value;

    public Optional<T> Or(T value) =>
        Value ?? value;

    public Optional<T> Or(Func<T> valueProducer) =>
        Value ?? valueProducer();

    public bool TryGet(out T value)
    {
        value = Value;
        return IsOccupied;
    }

    public static implicit operator T(Optional<T> optional) => optional.Value;

    public static implicit operator Optional<T>(T? value) => new(value);
}