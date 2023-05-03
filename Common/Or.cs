namespace Broker.Common;

public record struct Or<T1, T2>
{
    public Optional<T1> First { get; }
    public Optional<T2> Second { get; }

    public Or(T1 firstValue, T2 secondValue) =>
        (First, Second) = (firstValue, secondValue);

    public Or(T1 value) =>
        First = value;

    public Or(T2 value) =>
        Second = value;

    public bool TryPickAny(out T1 firstValue, out T2 secondValue) =>
        TryPickFirst(out firstValue) | TryPickSecond(out secondValue);

    public bool TryPickFirst(out T1 firstValue) => First.TryGet(out firstValue);

    public bool TryPickSecond(out T2 secondValue) => Second.TryGet(out secondValue);

    public static implicit operator T1(Or<T1, T2> optional) => optional.First;
    public static implicit operator T2(Or<T1, T2> optional) => optional.Second;

    public static implicit operator Or<T1, T2>(T1 value) => new(value);
    public static implicit operator Or<T1, T2>(T2 value) => new(value);

    public static implicit operator Or<T1, T2>((T1, T2) tuple) => new(tuple.Item1, tuple.Item2);
}