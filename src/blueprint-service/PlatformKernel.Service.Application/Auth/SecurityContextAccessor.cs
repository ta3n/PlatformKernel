using PlatformKernel.SysException;
using PlatformKernel.SysException.Exceptions;

namespace PlatformKernel.Service.Application.Auth;

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
            var userKey = httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(
                    x => x.Type == claimType
                )
                ?.Value;
            logger.LogDebug("{SecurityContext} Current user: {UserKey}", "SecurityContext", userKey);

            return userKey;
        }
    }

    public string? CompactApplicationUserKey
    {
        get
        {
            var userKey = ApplicationUserKey
                ?? throw new AppBaseException(
                    ErrorCode.E0103,
                    "UserKey is null or empty. Please check the JWT token."
                );
            var compactUserKey = string.IsNullOrEmpty(userKey)
                ? null
                : userKey.Replace("-", string.Empty);

            return compactUserKey;
        }
    }

    public string GetLanguageCode()
    {
        const string defaultAcceptLanguage = "en-US";
        var acceptLanguageKey = httpContextAccessor.HttpContext?.Request.Headers.AcceptLanguage.ToString();
        var language = string.IsNullOrEmpty(acceptLanguageKey) ? defaultAcceptLanguage : acceptLanguageKey.Split(',')[0];
        var languageCode = language.Split('-')[0];
        return languageCode;
    }
}
