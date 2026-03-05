using Liberty.SysException.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Liberty.Reservation.User.Application.Auth;

public class SecurityContextAccessor(
    ILogger<SecurityContextAccessor> logger,
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

    public string? ApplicationUserKey
    {
        get
        {
            const string claimType = "code";
            var user = httpContextAccessor.HttpContext?.User;
            if (user == null || user.Identity?.IsAuthenticated == false)
            {
                return null;
            }

            var userKeyValue = httpContextAccessor.HttpContext?.User.Claims
                    .FirstOrDefault(
                        x => x.Type == claimType
                    )
                    ?.Value
                ?? throw new AppLibertyException("User key not found");

            logger.LogDebug("{SecurityContext} Current user: {UserKey}", "SecurityContext", userKeyValue);

            return userKeyValue;
        }
    }

    public string GetLanguageCode()
    {
        const string defaultAcceptLanguage = "ja-JP";
        var acceptLanguageKey = httpContextAccessor.HttpContext?.Request.Headers.AcceptLanguage.ToString();
        var language = string.IsNullOrEmpty(acceptLanguageKey) ? defaultAcceptLanguage : acceptLanguageKey.Split(',')[0];
        var languageCode = language.Split('-')[0];
        return languageCode;
    }
}
