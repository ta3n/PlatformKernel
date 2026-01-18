using System.Globalization;
using System.Text.RegularExpressions;

namespace Liberty.ApplicationShared.Utils;

public static class ConvertUtil
{
    private static readonly CultureInfo CultureInfo = new("en-US");

    public static string ExtractMailAccount(
        string val
    )
    {
        return val.Split('@')[0];
    }

    public static string Format(
        string format,
        params object[] vals
    )
    {
        return string.Format(
            CultureInfo,
            format,
            vals
        );
    }

    public static string ToString(
        byte val,
        string format
    )
    {
        return val.ToString(
            format,
            CultureInfo
        );
    }

    public static string ToString(
        DateTime val,
        string format
    )
    {
        return val.ToString(
            format,
            CultureInfo
        );
    }

    public static string? ToString(
        TimeSpan? val,
        string format
    )
    {
        return val?.ToString(
            format,
            CultureInfo
        );
    }

    public static string ToString(
        int val,
        string format
    )
    {
        return val.ToString(
            format,
            CultureInfo
        );
    }

    public static string ToString(
        int val
    )
    {
        return val.ToString(CultureInfo);
    }

    public static string? ToString(
        int? val
    )
    {
        return val?.ToString();
    }

    public static string ToString(
        long val,
        string format
    )
    {
        return val.ToString(
            format,
            CultureInfo
        );
    }

    public static string ToString(
        long val
    )
    {
        return val.ToString(CultureInfo);
    }

    public static string? ToString(
        long? val
    )
    {
        return val?.ToString(CultureInfo);
    }

    public static int ToInt(
        string val
    )
    {
        return int.Parse(
            val,
            CultureInfo
        );
    }

    public static long ToLong(
        string val
    )
    {
        return long.Parse(
            val,
            CultureInfo
        );
    }

    public static long ToLong(
        DateTime val
    )
    {
        return long.Parse(
            ToString(val, "yyyyMMdd"),
            CultureInfo
        );
    }

    public static DateTime ToDateTime(
        string val
    )
    {
        return DateTime.Parse(val, CultureInfo);
    }

    public static DateTime ToDateTime2(
        string val,
        string format
    )
    {
        return DateTime.ParseExact(
            val,
            format,
            DateTimeFormatInfo.InvariantInfo,
            DateTimeStyles.NoCurrentDateDefault
        );
    }

    public static DateTime ToDateTime(
        string val,
        string regex
    )
    {
        // FIXME 第二引数をチェッカーに使い、内部では他のToDateTimeと同様の処理を使っているので、チェック処理はValidateUtilで実装し、変換はToDateTimeを使うように。
        if (!Regex.IsMatch(val, regex))
        {
            throw new FormatException();
        }

        return DateTime.Parse(
            val,
            CultureInfo
        );
    }

    public static TimeSpan? ToNullableTimeSpan(
        string val
    )
    {
        if (string.IsNullOrEmpty(val))
        {
            return null;
        }

        return TimeSpan.Parse(
            val,
            CultureInfo
        );
    }

    public static string? ToMail(
        string val
    )
    {
        return string.IsNullOrEmpty(val) ? null : val.ToLower(CultureInfo);
    }
}
