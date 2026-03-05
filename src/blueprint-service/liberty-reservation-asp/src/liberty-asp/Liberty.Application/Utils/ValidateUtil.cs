using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Liberty.Application.Utils
{
    public class ValidateUtil
    {


        public static bool IsHalfKana(string val)
        {
            return new Regex("^[\uFF66-\uFF9F]+$").IsMatch(val); ;
        }

        public static void CheckHalfKana(string val)
        {
            if (!IsHalfKana(val))
            {
                throw new InvalidDataException(val);
            }
        }




        public static bool IsInDate(
            DateTime? dateTime,
            DateTime? start,
            DateTime? end)
        {
            // 対象がnullであればfalse
            if (dateTime == null) { return false; }

            try
            {
                CheckInDate(
                    dateTime,
                    start,
                    end);
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
            DateTime? end)
        {
            // 対象がnullであれば何もしない
            if (dateTime == null) { return; }

            if (start != null && start.Value > dateTime.Value)
            {
                throw new InvalidDataException();
                //throw new ValidateOutOfDateException(
                //dateTime,
                //start,
                //end);
            }
            if (end != null && end.Value < dateTime.Value)
            {
                throw new InvalidDataException();
                //throw new ValidateOutOfDateException(
                //dateTime,
                //start,
                //end);
            }
        }


        /// <summary>
        /// メールアドレス検証
        /// 文字列が有効な電子メール形式であるかどうかを検証します
        /// </summary>
        /// <see cref="https://docs.microsoft.com/ja-jp/dotnet/standard/base-types/how-to-verify-that-strings-are-in-valid-email-format"/>
        /// <param name="email"></param>
        /// <returns></returns>
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // Eメールアドレスのフォーマットチェック
                //return Regex.IsMatch(email,
                //          @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                //          @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-0-9a-z]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                //          RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));

                // Normalize the domain
                email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
                                      RegexOptions.None, TimeSpan.FromMilliseconds(200));

                // Examines the domain part of the email and normalizes it.
                string DomainMapper(Match match)
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
                return Regex.IsMatch(email,
                    @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                    @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-0-9a-z]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }
    }
}
