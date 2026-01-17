using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace SharedKernel.Entity.Utils;

/// <summary>
/// Utility class that provides functionality for determining language-related headers
/// within an HTTP request context.
/// </summary>
public static class LanguageHeaderUtil
{
    /// <summary>
    /// Specifies the default language code used within the application.
    /// </summary>
    /// <remarks>
    /// Its value is set to "ja", representing Japanese as the default language.
    /// This constant is utilized in scenarios where a language-specific operation
    /// or fallback language is required, such as database migrations or handling
    /// language headers in requests.
    /// </remarks>
    public const string DefaultLanguageCode = "ja";

    /// <summary>
    /// Represents the default value for the Accept-Language header when no specific language is provided.
    /// The default language is set to "ja-JP", which corresponds to Japanese (Japan).
    /// </summary>
    public const string DefaultAcceptLanguage = "ja-JP";

    /// Retrieves the primary language code from the Accept-Language header in the HTTP request.
    /// The method accesses the HTTP context to extract the Accept-Language header value, which specifies
    /// the preferred languages of the client. It then parses the header to determine the primary
    /// language code by extracting the portion before the hyphen, if present. If the Accept-Language
    /// header is missing or empty, a default language code is used.
    /// <return>
    /// The primary language code extracted from the Accept-Language header or the default language code
    /// if the header is not present or is empty.
    /// </return>
    public static string GetLanguageCodeFromHeader()
    {
        var context = new HttpContextAccessor().HttpContext;
        var acceptLanguage = context?.Request.Headers[HeaderNames.AcceptLanguage].ToString();
        var language = string.IsNullOrEmpty(acceptLanguage) ? DefaultAcceptLanguage : acceptLanguage.Split(',')[0];
        var languageCode = language.Split('-')[0];

        return languageCode;
    }
}
