namespace TodoList.Application.Common.Exceptions;

public sealed class AppValidationException : Exception
{
    public IReadOnlyDictionary<string, string[]> Errors { get; }

    public AppValidationException(IDictionary<string, string[]> errors)
        : base("Validation failed")
        => Errors = new Dictionary<string, string[]>(errors);
}