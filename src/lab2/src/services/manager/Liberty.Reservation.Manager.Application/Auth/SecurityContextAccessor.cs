using System.Globalization;

namespace Liberty.Reservation.Manager.Application.Auth;

public class SecurityContextAccessor(
    ILogger<SecurityContextAccessor> logger,
    IHttpContextAccessor httpContextAccessor
) : ISecurityContextAccessor
{
    public const string FacilityHeaderKey = "X-Facility-Key";
    public const string FacilityHeaderId = "X-Facility-Id";
    public const string FacilityHeaderTimeZone = "X-Facility-Time-Zone";
    public const string FacilityHeaderRecordCode = "X-Facility-Record-Code";
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
                ?? throw new AppLibertyException(
                    ErrorCode.E0103,
                    "UserKey is null or empty. Please check the JWT token."
                );
            var compactUserKey = string.IsNullOrEmpty(userKey)
                ? null
                : userKey.Replace("-", string.Empty);

            return compactUserKey;
        }
    }

    public long FacilityKey
    {
        get
        {
            var existingFacility = ExistingFacility();

            return existingFacility.Id;
        }
    }

    public string? FacilityRecordCode
    {
        get
        {
            var facilityRecordCode = httpContextAccessor.HttpContext?.Request.Headers[FacilityHeaderRecordCode].ToString();

            return facilityRecordCode;
        }
    }

    private Facility ExistingFacility()
    {
        var facilityHeaderValue = httpContextAccessor.HttpContext?.Request.Headers[FacilityHeaderId];
        if (string.IsNullOrEmpty(facilityHeaderValue))
        {
            throw new FacilityNotfoundException();
        }

        _ = long.TryParse(facilityHeaderValue, out var selectFacilityId);

        logger.LogDebug("{SecurityContext} Current facility: {FacilityKey}", "SecurityContext", selectFacilityId);

        return new Facility { Id = selectFacilityId };
    }

    public string GetFacilityCode()
    {
        var facilityCode = httpContextAccessor.HttpContext?.Request.Headers[FacilityHeaderKey].ToString();
        if (string.IsNullOrEmpty(facilityCode))
        {
            throw new FacilityNotfoundException();
        }

        return facilityCode;
    }

    public string? GetFacilityCodeValue()
    {
        var facilityCode = httpContextAccessor.HttpContext?.Request.Headers[FacilityHeaderKey].ToString();

        return facilityCode;
    }

    public TimeSpan? GetFacilityTimeZoneOffset()
    {
        var facilityTimeZone = httpContextAccessor.HttpContext?
            .Request.Headers[FacilityHeaderTimeZone]
            .ToString();

        if (string.IsNullOrWhiteSpace(facilityTimeZone))
        {
            return TimeSpan.FromHours(9);
        }

        if (TimeSpan.TryParse(facilityTimeZone, CultureInfo.InvariantCulture, out var offset))
        {
            return offset;
        }

        return TimeSpan.FromHours(9);
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
