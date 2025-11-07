using PubSea.Framework.Services.Abstractions;
using PubSea.Framework.Services.Implementations;

namespace PubSea.Framework.Utility;

public static class DateTimeHelper
{
    private static readonly IDateTimeService? _dateTimeService = new DateTimeService();

    /// <summary>
    /// Turns Jalali string with format yyyy/MM/dd into DateTimeOffset
    /// </summary>
    /// <param name="jalaliDate"></param>
    /// <returns></returns>
    public static DateTimeOffset ToGregorianDateTime(this string jalaliDate)
    {
        jalaliDate = jalaliDate.CorrectSpelling();
        var splitDate = jalaliDate.Split('/').Select(int.Parse).ToArray();

        return _dateTimeService!.FromPersianDate(splitDate[0], (byte)splitDate[1], (byte)splitDate[2]);
    }

    /// <summary>
    /// Turns Jalali string with format yyyy/MM/dd into DateTimeOffset
    /// </summary>
    /// <param name="jalaliDate"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static DateTimeOffset? ToGregorianDateTimeOrDefault(this string jalaliDate, DateTimeOffset? defaultValue = default)
    {
        try
        {
            jalaliDate = jalaliDate.CorrectSpelling();
            var splitDate = jalaliDate.Split('/').Select(int.Parse).ToArray();

            return _dateTimeService!.FromPersianDate(splitDate[0], (byte)splitDate[1], (byte)splitDate[2]);
        }
        catch
        {
            return defaultValue;
        }
    }

    /// <summary>
    /// Turns DateTime into Jalali string with format yyyy/MM/dd HH:mm:ss
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    public static string ToJalaliDateTimeString(this DateTime dateTime)
    {
        CheckKind(ref dateTime);
        return _dateTimeService?.ToPersianDate(dateTime)[0..19].ToString().Replace("-", "/").Replace("T", " ")!;
    }

    /// <summary>
    /// Turns DateTime into Jalali string with format yyyy/MM/dd
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    public static string ToJalaliDateString(this DateTime dateTime)
    {
        CheckKind(ref dateTime);
        return _dateTimeService?.ToPersianDate(dateTime)[0..10].ToString().Replace("-", "/")!;
    }

    /// <summary>
    /// Turns DateTime into Jalali string with format MM/dd
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    public static string ToJalaliMonthAndDayString(this DateTime dateTime)
    {
        CheckKind(ref dateTime);
        return _dateTimeService?.ToPersianDate(dateTime)[5..10].ToString().Replace("-", "/")!;
    }

    public static (DateTime nowGregorian, string nowJalali) GetUtcCurrentDateTimes()
    {
        var dateTime = DateTime.UtcNow;
        var jalaliDateTime = DateTime.UtcNow.ToJalaliDateTimeString();
        return (dateTime, jalaliDateTime);
    }

    private static void CheckKind(ref DateTime dateTime)
    {
        if (dateTime.Kind != DateTimeKind.Unspecified)
        {
            return;
        }

        dateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Local);
    }
}
