using PubSea.Framework.Services.Abstractions;
using System.Globalization;
using System.Text;

namespace PubSea.Framework.Services.Implementations;

internal sealed class DateTimeService : IDateTimeService
{
    DateTimeOffset IDateTimeService.Now => DateTimeOffset.Now;

    DateTimeOffset IDateTimeService.UtcNow => DateTimeOffset.UtcNow;

    DateTimeOffset IDateTimeService.GetDateTimeWithOffset(DateTime datetime, TimeSpan offset)
    {
        return new DateTimeOffset(datetime, offset);
    }

    ReadOnlySpan<char> IDateTimeService.ToPersianDate(DateTime dateTime)
    {
        var now = dateTime.ToLocalTime();
        var p = new PersianCalendar();
        now = CheckMinMax(now, p);

        var date = new StringBuilder(23);
        date.Append(p.GetYear(now));
        date.Append('-');
        date.Append(p.GetMonth(now).ToString().PadLeft(2, '0'));
        date.Append('-');
        date.Append(p.GetDayOfMonth(now).ToString().PadLeft(2, '0'));
        date.Append('T');
        date.Append(p.GetHour(now).ToString().PadLeft(2, '0'));
        date.Append(':');
        date.Append(p.GetMinute(now).ToString().PadLeft(2, '0'));
        date.Append(':');
        date.Append(p.GetSecond(now).ToString().PadLeft(2, '0'));
        date.Append('.');
        date.Append(p.GetMilliseconds(now).ToString().PadRight(3, '0'));

        return date.ToString();
    }

    DateTimeOffset IDateTimeService.FromPersianDate(int year, byte month, byte day,
        byte hour, byte minute, byte second, int millisecond, TimeSpan offset)
    {
        PersianCalendar calendar = new();
        return calendar.ToDateTime(year, month, day, hour, minute, second, millisecond);
    }

    private static DateTime CheckMinMax(DateTime now, PersianCalendar p)
    {
        if (now < p.MinSupportedDateTime)
        {
            now = p.MinSupportedDateTime;
        }

        if (now > p.MaxSupportedDateTime)
        {
            now = p.MaxSupportedDateTime;
        }

        return now;
    }
}
