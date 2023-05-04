namespace Broker.Core.Exceptions;

/// <summary>
/// Replacement for any parsing exception.
/// </summary>
public class ParsingException : AggregateException
{
    public ParsingException(IEnumerable<Exception> innerExceptions) : base(innerExceptions) { }
    public ParsingException(params Exception[] innerExceptions) : base(innerExceptions) { }
}