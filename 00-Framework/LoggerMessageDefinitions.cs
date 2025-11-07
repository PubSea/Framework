using Microsoft.Extensions.Logging;
using PubSea.Framework.Utility;
using System.Net;

namespace PubSea.Framework;

public static partial class LoggerMessageDefinitions
{
    [LoggerMessage(EventId = 101, Level = LogLevel.Error,
        Message = "{Message}, {TraceId}, {DateTime}, {JalaliDateTime}",
        SkipEnabledCheck = true)]
    private static partial void LogErrorWithTraceId(this ILogger logger, Exception ex, string message,
        string traceId, DateTime dateTime, string jalaliDateTime);

    public static void LogErrorWithTraceId(this ILogger logger, Exception ex, string message, string traceId)
    {
        var (dateTime, jalaliDateTime) = DateTimeHelper.GetUtcCurrentDateTimes();
        LogErrorWithTraceId(logger, ex, message, traceId, dateTime, jalaliDateTime);
    }

    //=======================================================================================================================================//

    [LoggerMessage(EventId = 444, Level = LogLevel.Error,
    Message = "Request was not in success status codes, {StatusCode}, {RequestCurl}, {Content}, {DateTime}, {JalaliDateTime}, {SecurityLog}",
    SkipEnabledCheck = true)]
    private static partial void LogUnsuccessfullHttpRequest(this ILogger logger,
    HttpStatusCode statusCode, string? requestCurl, string content,
    DateTime dateTime, string jalaliDateTime, byte securityLog);

    public static void LogUnsuccessfullHttpRequest(this ILogger logger,
        HttpStatusCode statusCode, string? requestCurl, string content)
    {
        var (dateTime, jalaliDateTime) = DateTimeHelper.GetUtcCurrentDateTimes();
        LogUnsuccessfullHttpRequest(logger, statusCode, requestCurl, content, dateTime, jalaliDateTime, 1);
    }

    //=======================================================================================================================================//

    [LoggerMessage(EventId = 2000, Level = LogLevel.Information,
    Message = "File is saving in object store, {FilePath}, {ContentType}, {DateTime}, {JalaliDateTime}, {SecurityLog}",
    SkipEnabledCheck = true)]
    private static partial void LogFileSavingStarted(this ILogger logger,
    string filePath, string contentType,
    DateTime dateTime, string jalaliDateTime, byte securityLog);

    public static void LogFileSavingStarted(this ILogger logger, string filePath, string contentType)
    {
        var (dateTime, jalaliDateTime) = DateTimeHelper.GetUtcCurrentDateTimes();
        LogFileSavingStarted(logger, filePath, contentType, dateTime, jalaliDateTime, 1);
    }

    //=======================================================================================================================================//

    [LoggerMessage(EventId = 2001, Level = LogLevel.Information,
    Message = "File saving in object store finished successfully, {FilePath}, {ContentType}, {DateTime}, {JalaliDateTime}, {SecurityLog}",
    SkipEnabledCheck = true)]
    private static partial void LogFileSavingFinished(this ILogger logger,
    string filePath, string contentType,
    DateTime dateTime, string jalaliDateTime, byte securityLog);

    public static void LogFileSavingFinished(this ILogger logger, string filePath, string contentType)
    {
        var (dateTime, jalaliDateTime) = DateTimeHelper.GetUtcCurrentDateTimes();
        LogFileSavingFinished(logger, filePath, contentType, dateTime, jalaliDateTime, 1);
    }

    //=======================================================================================================================================//

}
