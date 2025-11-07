namespace PubSea.Framework.Services.Abstractions;

public interface IDateTimeService
{
    /// <summary>
    /// Local datetime
    /// </summary>
    DateTimeOffset Now { get; }

    /// <summary>
    /// UTC datetime
    /// </summary>
    DateTimeOffset UtcNow { get; }

    /// <summary>
    /// Returuns datetime with the requested offset
    /// </summary>
    /// <param name="datetime"></param>
    /// <param name="offset"></param>
    /// <returns></returns>
    DateTimeOffset GetDateTimeWithOffset(DateTime datetime, TimeSpan offset);

    /// <summary>
    /// Turns a gregorian datetime into persian datetime
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    ReadOnlySpan<char> ToPersianDate(DateTime dateTime);

    /// <summary>
    /// Turns persian datetime to gregorian datetime
    /// </summary>
    /// <param name="year">Year</param>
    /// <param name="month">Month</param>
    /// <param name="day">Day</param>
    /// <param name="hour">Hour</param>
    /// <param name="minute">Minute</param>
    /// <param name="second">Second</param>
    /// <param name="millisecond">Millisecond</param>
    /// <param name="offset">Offset</param>
    /// <returns></returns>
    DateTimeOffset FromPersianDate(int year, byte month, byte day,
        byte hour = 0, byte minute = 0, byte second = 0, int millisecond = 0, TimeSpan offset = default);
}
