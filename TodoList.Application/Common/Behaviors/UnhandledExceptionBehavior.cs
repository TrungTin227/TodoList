using MediatR;

namespace TodoList.Application.Common.Behaviors;

public sealed class UnhandledExceptionBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct)
    {
        try
        {
            return await next();
        }
        catch (OperationCanceledException)
        {
            var err = new Error(ErrorCodes.BadRequest, "Operation cancelled");
            if (IsResultType(typeof(TResponse))) return FailureResponse<TResponse>(err);
            throw;
        }
        catch (Exception)
        {
            var err = new Error(ErrorCodes.ServerError, "Unexpected server error");
            if (IsResultType(typeof(TResponse))) return FailureResponse<TResponse>(err);
            throw;
        }
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