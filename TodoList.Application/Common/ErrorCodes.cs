namespace TodoList.Application.Common;

public static class ErrorCodes
{
    public const string Validation   = "validation";
    public const string BadRequest   = "bad_request";
    public const string Unauthorized = "unauthorized";
    public const string Forbidden    = "forbidden";
    public const string NotFound     = "not_found";
    public const string Conflict     = "conflict";
    public const string TooMany      = "too_many";
    public const string ServerError  = "server_error";
}