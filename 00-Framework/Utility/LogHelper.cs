using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.JsonWebTokens;

namespace PubSea.Framework.Utility;

/// <summary>
/// This is an extension on dotnet ILogger
/// </summary>
public static class LogHelper
{
    /// <summary>
    /// Logs with log level as well as datetime in gregorian and jalali formats.
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="logLevel"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    public static void LogEnriched(this ILogger logger, LogLevel logLevel, string? message, params object?[] args)
    {
        var now = DateTime.UtcNow;
        var jalaliNow = now.ToJalaliDateTimeString();

        var @params = new List<object?>(args)
        {
            now.ToString("yyyy/MM/dd HH:mm:ss"),
            jalaliNow,
        };

        logger.Log(logLevel, $"{message}, {{DateTime}}, {{JalaliDateTime}}", @params.ToArray());
    }

    /// <summary>
    /// Logs with log level and exception as well as datetime in gregorian and jalali formats.
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="logLevel"></param>
    /// <param name="exception"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    public static void LogEnriched(this ILogger logger, LogLevel logLevel, Exception? exception, string? message, params object?[] args)
    {
        var now = DateTime.UtcNow;
        var jalaliNow = now.ToJalaliDateTimeString();

        var @params = new List<object?>(args)
        {
            now.ToString("yyyy/MM/dd HH:mm:ss"),
            jalaliNow,
        };

        logger.Log(logLevel, exception, $"{message}, {{DateTime}}, {{JalaliDateTime}}", @params.ToArray());
    }

    /// <summary>
    /// Logs with log level and exception as well as datetime in gregorian and jalali formats and security flag.
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="logLevel"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    public static void LogSecurityEnriched(this ILogger logger, LogLevel logLevel, string? message, params object?[] args)
    {
        var now = DateTime.UtcNow;
        var jalaliNow = now.ToJalaliDateTimeString();

        var @params = new List<object?>(args)
        {
            now.ToString("yyyy/MM/dd HH:mm:ss"),
            jalaliNow,
            1,
        };

        logger.Log(logLevel, $"{message}, {{DateTime}}, {{JalaliDateTime}}, {{SecurityLog}}", @params.ToArray());
    }

    /// <summary>
    /// Logs with log level and exception as well as datetime in gregorian and jalali formats and security flag.
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="logLevel"></param>
    /// <param name="exception"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    public static void LogSecurityEnriched(this ILogger logger, LogLevel logLevel, Exception? exception, string? message, params object?[] args)
    {
        var now = DateTime.UtcNow;
        var jalaliNow = now.ToJalaliDateTimeString();

        var @params = new List<object?>(args)
        {
            now.ToString("yyyy/MM/dd HH:mm:ss"),
            jalaliNow,
            1,
        };

        logger.Log(logLevel, exception, $"{message}, {{DateTime}}, {{JalaliDateTime}}, {{SecurityLog}}", @params.ToArray());
    }

    /// <summary>
    /// Logs activity
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="httpContextAccessor"></param>
    /// <param name="logLevel"></param>
    /// <param name="exception"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    public static void LogActivity(this ILogger logger, IHttpContextAccessor httpContextAccessor,
        LogLevel logLevel, Exception? exception, string? message, params object?[] args)
    {
        var now = DateTime.UtcNow;
        var jalaliNow = now.ToJalaliDateTimeString();

        var userId = long.Parse(httpContextAccessor?.HttpContext?.User.Claims
            .FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value ?? string.Empty);

        var @params = new List<object?>(args)
        {
            now.ToString("yyyy/MM/dd HH:mm:ss"),
            jalaliNow,
            userId,
            1,
        };

        logger.Log(logLevel, exception,
            $"{message}, {{DateTime}}, {{JalaliDateTime}}, {{ActionBy}}, {{SecurityLog}}",
            @params.ToArray());
    }
}
