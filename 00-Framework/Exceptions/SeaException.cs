namespace PubSea.Framework.Exceptions;

/// <summary>
/// This is uniform exception in all applications. Every exception can carry a exception status 
/// which would be translated to http status code if the final endpoint is http rest api or it 
/// to grpc status code if the final endpoint is a grpc service.
/// </summary>
public sealed class SeaException : Exception
{
    public const int INTERNAL_ERROR_CODE = 50_000;
    public const int INVALID_REQUEST_CODE = 40_000;
    public const int UNAUTHENTICATED_CODE = 40_001;
    public const int FORBIDDEN_CODE = 40_003;
    public const int NOT_FOUND_CODE = 40_004;
    public const int TOO_MANY_REQUEST_CODE = 40_029;
    public const int INVALID_REDIRECT_URL_CODE = 30_002;

    public const string BASE_MESSAGE =
        "یک خطای غیر منتظره رخ داد. لطفا با ادمین تماس بگیرید.";

    public SeaException(string message, int code, ExceptionStatus exceptionStatus, Exception innerException, string? logMessage = default)
        : base(GetMessage(message), innerException)
    {
        ExceptionStatus = exceptionStatus;
        LogMessage = logMessage;
        TraceId = TraceIdGenerator.Generate();
        Code = code;
    }

    public SeaException(string message, int code, Exception innerException, string? logMessage = default)
        : base(GetMessage(message), innerException)
    {
        LogMessage = logMessage;
        TraceId = TraceIdGenerator.Generate();
        Code = code;
    }

    public SeaException(string message, Exception innerException, string? logMessage = default)
        : base(GetMessage(message), innerException)
    {
        LogMessage = logMessage;
        TraceId = TraceIdGenerator.Generate();
        Code = INTERNAL_ERROR_CODE;
    }

    public SeaException(string message, int code, ExceptionStatus exceptionStatus, string? logMessage = default)
        : base(GetMessage(message))
    {
        ExceptionStatus = exceptionStatus;
        LogMessage = logMessage;
        TraceId = TraceIdGenerator.Generate();
        Code = code;
    }

    public SeaException(string message, ExceptionStatus exceptionStatus, string? logMessage = default)
        : base(GetMessage(message))
    {
        ExceptionStatus = exceptionStatus;
        LogMessage = logMessage;
        TraceId = TraceIdGenerator.Generate();
        Code = INTERNAL_ERROR_CODE;
    }

    public SeaException(string message, int code, string? logMessage = default)
        : base(GetMessage(message))
    {
        LogMessage = logMessage;
        TraceId = TraceIdGenerator.Generate();
        Code = code;
    }

    public SeaException(string message, string? logMessage = default)
        : base(GetMessage(message))
    {
        LogMessage = logMessage;
        TraceId = TraceIdGenerator.Generate();
        Code = INTERNAL_ERROR_CODE;
    }

    public SeaException(ExceptionStatus exceptionStatus, string? logMessage = default)
        : base(BASE_MESSAGE)
    {
        ExceptionStatus = exceptionStatus;
        LogMessage = logMessage;
        TraceId = TraceIdGenerator.Generate();
        Code = INTERNAL_ERROR_CODE;
    }

    public SeaException(string? logMessage = default)
        : base(BASE_MESSAGE)
    {
        LogMessage = logMessage;
        TraceId = TraceIdGenerator.Generate();
        Code = INTERNAL_ERROR_CODE;
    }

    public string TraceId { get; private set; } = default!;
    public int Code { get; private set; }
    public ExceptionStatus ExceptionStatus { get; private set; } = ExceptionStatus.Internal;
    public string? LogMessage { get; private set; } = BASE_MESSAGE;

    private static string GetMessage(string message)
    {
        return string.IsNullOrWhiteSpace(message) ? BASE_MESSAGE : message;
    }
}