using Microsoft.Extensions.Logging;
using PubSea.Framework.Exceptions;

namespace PubSea.Framework.Utility;

public static partial class LoggerMessageDefinitions
{
    [LoggerMessage(EventId = 1_000_000, Level = LogLevel.Error,
        Message = "{ExceptionMessage} - {TraceId} - {Code}, {DateTime}, {JalaliDateTime}, {SecurityLog}",
        SkipEnabledCheck = true)]
    private static partial void LogException(this ILogger logger, SeaException exception,
        string exceptionMessage, string traceId, int code,
        DateTime dateTime, string jalaliDateTime, byte securityLog);

    public static void LogException(this ILogger logger, SeaException exception,
        string exceptionMessage, string traceId, int code)
    {
        var (dateTime, jalaliDateTime) = DateTimeHelper.GetUtcCurrentDateTimes();
        LogException(logger, exception, exceptionMessage, traceId, code, dateTime, jalaliDateTime, 0);
    }

    //=======================================================================================================================================//

    [LoggerMessage(EventId = 1_000_001, Level = LogLevel.Critical,
        Message = "Heavy entity framework query, {ElapsedTime}, {Command}, {DateTime}, {JalaliDateTime}, {SecurityLog}",
        SkipEnabledCheck = true)]
    private static partial void LogHeavyQuery(this ILogger logger, TimeSpan elapsedTime, string command, DateTime dateTime, string jalaliDateTime, byte securityLog);

    public static void LogHeavyQuery(this ILogger logger, TimeSpan elapsedTime, string command)
    {
        var (dateTime, jalaliDateTime) = DateTimeHelper.GetUtcCurrentDateTimes();
        LogHeavyQuery(logger, elapsedTime, command, dateTime, jalaliDateTime, 1);
    }
}
