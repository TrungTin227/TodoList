using System.Net;
using System.Text.Json;
using TodoList.Application.Common.Exceptions;
using TodoList.WebApi.Contracts;

namespace TodoList.Web.Middlewares;
public sealed class GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try { await next(context); }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext ctx, Exception ex)
    {
        ctx.Response.ContentType = "application/json";
        ApiResult<object> response;

        switch (ex)
        {
            case AppValidationException appVal:
                ctx.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                // Tạo đối tượng mới với các giá trị phù hợp
                response = new ApiResult<object>(false, "Validation failed", appVal.Errors);
                break;

            case UnauthorizedAccessException:
                ctx.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                response = new ApiResult<object>(false, "Unauthorized access");
                break;

            case KeyNotFoundException:
                ctx.Response.StatusCode = (int)HttpStatusCode.NotFound;
                response = new ApiResult<object>(false, "Resource not found");
                break;

            case ArgumentException arg:
                ctx.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response = new ApiResult<object>(false, arg.Message);
                break;

            default:
                ctx.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response = new ApiResult<object>(false, "An internal server error occurred");
                break;
        }

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        await ctx.Response.WriteAsync(json);
    }
}

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandling(this IApplicationBuilder app)
        => app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
}