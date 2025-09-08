using MediatR;
using TodoList.Application.Common.Results;

namespace TodoList.Application.Common.Behaviors;

public sealed class UnhandledExceptionBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, Result<TResponse>>
{
    public async Task<Result<TResponse>> Handle(
        TRequest request,
        RequestHandlerDelegate<Result<TResponse>> next,
        CancellationToken ct)
    {
        try
        {
            return await next();
        }
        catch (OperationCanceledException)
        {
            return Result<TResponse>.Failure(new Error(ErrorCodes.BadRequest, "Operation cancelled"));
        }
        catch (Exception)
        {
            return Result<TResponse>.Failure(new Error(ErrorCodes.ServerError, "Unexpected server error"));
        }
    }
}