namespace PubSea.Framework.ApiResponse;

/// <summary>
/// This is a uniform http api error
/// </summary>
public sealed class ApiError
{
    public ApiError()
    { }

    public ApiError(string message, string traceId, int code)
    {
        Message = message;
        TraceId = traceId;
        Code = code;
    }

    public string Message { get; init; } = default!;
    public string TraceId { get; init; } = default!;
    public int Code { get; init; }
}