using FluentValidation;
using MediatR;

namespace TodoList.Application.Common.Behaviors;

public sealed class ValidationBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;
    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators) => _validators = validators;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct)
    {
        if (!_validators.Any()) return await next();

        var ctx = new ValidationContext<TRequest>(request);
        var results = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(ctx, ct)));
        var failures = results.SelectMany(r => r.Errors).Where(e => e is not null).ToList();

        if (failures.Count == 0) return await next();

        var message = string.Join("; ", failures.Select(f => $"{f.PropertyName}: {f.ErrorMessage}"));
        var error = new Error(ErrorCodes.Validation, message);

        if (IsResultType(typeof(TResponse))) return FailureResponse<TResponse>(error);
        // Nếu handler không trả Result/Result<T>, giữ nguyên hành vi: ném exception
        throw new ValidationException(message);
    }

    private static bool IsResultType(Type t) =>
        t == typeof(Result) || (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Result<>));

    private static TRes FailureResponse<TRes>(Error e)
    {
        var t = typeof(TRes);
        if (t == typeof(Result)) return (TRes)(object)Result.Failure(e);

        var inner = t.GenericTypeArguments[0];
        var method = typeof(Result).GetMethods()
            .First(m => m.Name == nameof(Result.Failure) && m.IsGenericMethodDefinition);
        var generic = method.MakeGenericMethod(inner);
        return (TRes)generic.Invoke(null, new object[] { e })!;
    }
}