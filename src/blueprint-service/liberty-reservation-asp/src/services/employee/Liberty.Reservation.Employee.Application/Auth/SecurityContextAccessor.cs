using Microsoft.AspNetCore.Http;

namespace Liberty.Reservation.Employee.Application.Auth;

public class SecurityContextAccessor(
    IHttpContextAccessor httpContextAccessor
) : ISecurityContextAccessor
{
    public string? JwtTokenRaw => httpContextAccessor.HttpContext?.Request.Headers.Authorization;

    public string? IpAddressClient
    {
        get
        {
            var httpContext = httpContextAccessor.HttpContext;
            var ipAddress = httpContext?.Connection.RemoteIpAddress?.ToString();
            return ipAddress;
        }
    }

    public string? TimeZone => httpContextAccessor.HttpContext?.Request.Headers["Time-Zone"];

    public string? AcceptLanguage => httpContextAccessor.HttpContext?.Request.Headers.AcceptLanguage;

    public string GetLanguageCode()
    {
        const string defaultAcceptLanguage = "ja-JP";
        var acceptLanguageKey = httpContextAccessor.HttpContext?.Request.Headers.AcceptLanguage.ToString();
        var language = string.IsNullOrEmpty(acceptLanguageKey) ? defaultAcceptLanguage : acceptLanguageKey.Split(',')[0];
        var languageCode = language.Split('-')[0];
        return languageCode;
    }
}
