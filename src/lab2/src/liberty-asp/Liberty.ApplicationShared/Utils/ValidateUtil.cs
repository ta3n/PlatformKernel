using System.Globalization;
using System.Text.RegularExpressions;

namespace Liberty.ApplicationShared.Utils;

public static partial class ValidateUtil
{
    private static bool IsHalfKana(
        string val
    )
    {
        return HalfKanaRegex().IsMatch(val);
    }

    public static void CheckHalfKana(
        string val
    )
    {
        if (!IsHalfKana(val))
        {
            throw new InvalidDataException(val);
        }
    }

    public static bool IsInDate(
        DateTime? dateTime,
        DateTime? start,
        DateTime? end
    )
    {
        // 対象がnullであればfalse
        if (dateTime == null)
        {
            return false;
        }

        try
        {
            CheckInDate(
                dateTime,
                start,
                end
            );
            return true;
        }
        catch (InvalidDataException)
        {
            return false;
        }
    }

    public static void CheckInDate(
        DateTime? dateTime,
        DateTime? start,
        DateTime? end
    )
    {
        // 対象がnullであれば何もしない
        if (dateTime == null)
        {
            return;
        }

        if ((start != null && start.Value > dateTime.Value) || (end != null && end.Value < dateTime.Value))
        {
            throw new InvalidDataException();
        }
    }

    public static bool IsValidEmail(
        string email
    )
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return false;
        }

        try
        {
            // Normalize the domain
            email = Regex.Replace(
                email,
                @"(@)(.+)$",
                DomainMapper,
                RegexOptions.None,
                TimeSpan.FromMilliseconds(200)
            );

            // Examines the domain part of the email and normalizes it.
            static string DomainMapper(
                Match match
            )
            {
                // Use IdnMapping class to convert Unicode domain names.
                var idn = new IdnMapping();

                // Pull out and process domain name (throws ArgumentException on invalid)
                var domainName = idn.GetAscii(match.Groups[2].Value);

                return match.Groups[1].Value + domainName;
            }
        }
        catch (RegexMatchTimeoutException)
        {
            return false;
        }
        catch (ArgumentException)
        {
            return false;
        }

        try
        {
            return Regex.IsMatch(
                email,
                """^(?(")(".+?(?<!\\)"@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))"""
                + @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-0-9a-z]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                RegexOptions.IgnoreCase,
                TimeSpan.FromMilliseconds(250)
            );
        }
        catch (RegexMatchTimeoutException)
        {
            return false;
        }
    }

    [GeneratedRegex("^[\uFF66-\uFF9F]+$")]
    private static partial Regex HalfKanaRegex();
}
