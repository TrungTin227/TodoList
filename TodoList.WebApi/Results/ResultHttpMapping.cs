using TodoList.Application.Common.Results;
using TodoList.WebApi.Contracts;

namespace TodoList.WebApi.Results;

public static class ResultHttpMapping
{
    // Success: luôn trả 200 cùng envelope; không 204 để giữ format thống nhất.
    public static IResult ToHttpResult<T>(this Result<T> r)
        => r.IsSuccess
            ? TypedResults.Json(
                new ApiResult<T>(true, Data: r.Value),
                statusCode: StatusCodes.Status200OK)
            : TypedResults.Json(
                new ApiResult<object>(false, r.Error.Message, null, r.Error.Code),
                statusCode: MapStatus(r.Error));

    public static IResult ToHttpResult(this Result r)
        => r.IsSuccess
            ? TypedResults.Json(
                new ApiResult<object>(true),
                statusCode: StatusCodes.Status200OK)
            : TypedResults.Json(
                new ApiResult<object>(false, r.Error.Message, null, r.Error.Code),
                statusCode: MapStatus(r.Error));

    private static int MapStatus(Error e) => e.Code switch
    {
        // Chuẩn hoá code ở Application khi Failure(Error) để map chính xác
        "validation"    => StatusCodes.Status400BadRequest,
        "bad_request"   => StatusCodes.Status400BadRequest,
        "unauthorized"  => StatusCodes.Status401Unauthorized,
        "forbidden"     => StatusCodes.Status403Forbidden,
        "not_found"     => StatusCodes.Status404NotFound,
        "conflict"      => StatusCodes.Status409Conflict,
        "too_many"      => StatusCodes.Status429TooManyRequests,
        _               => StatusCodes.Status400BadRequest
    };
}