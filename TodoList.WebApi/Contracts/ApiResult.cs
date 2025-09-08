namespace TodoList.WebApi.Contracts;

public sealed record ApiResult<T>(
    bool IsSuccess,
    string? Message = null,
    T? Data = default, 
    string? ErrorCode = null
);