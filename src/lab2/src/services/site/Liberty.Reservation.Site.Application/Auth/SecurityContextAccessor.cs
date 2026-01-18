using System.IdentityModel.Tokens.Jwt;
using Liberty.Reservation.Site.Application.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Liberty.Reservation.Site.Application.Auth;

public class SecurityContextAccessor(
    IHttpContextAccessor httpContextAccessor,
    IConfiguration config
) : ISecurityContextAccessor
{
    public const string FacilityCodeHeaderKey = "X-Facility-Code";
    public const string FacilityHeaderRecordCode = "X-Facility-Record-Code";
    public const string SiteCodeOfFacilityHeaderKey = "X-Site-Code";

    public const string FacilityIdHeaderKey = "X-Facility-Id";
    public const string SiteIdOfFacilityHeaderKey = "X-Site-Id";

    private const string UserJwtClaimType = "code";

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

    public int TimeZoneOffset
    {
        get
        {
            var headerValue = httpContextAccessor.HttpContext?.Request.Headers["Time-Zone-Offset"];
            return int.TryParse(headerValue, out var offset) ? offset : 0;
        }
    }

    public string? AcceptLanguage => httpContextAccessor.HttpContext?.Request.Headers.AcceptLanguage;

    public string? FacilityRecordCode
    {
        get
        {
            var facilityRecordCode = httpContextAccessor.HttpContext?.Request.Headers[FacilityHeaderRecordCode].ToString();

            return facilityRecordCode;
        }
    }

    public string? GetApplicationUserKey()
    {
        var authorizationHeader = httpContextAccessor.HttpContext?.Request.Headers.Authorization.ToString();
        if (string.IsNullOrEmpty(authorizationHeader))
        {
            return null;
        }

        if (!authorizationHeader.StartsWith("Bearer "))
        {
            throw new UserKeyUnauthorizedException();
        }

        var jwtToken = authorizationHeader["Bearer ".Length..];
        var expectedIssuer = config["Identity:Jwt:Authority"] ?? string.Empty;

        if (!ValidateJwt(jwtToken, expectedIssuer))
        {
            throw new UserKeyUnauthorizedException();
        }

        try
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwtToken);

            var codeClaim = token.Claims.FirstOrDefault(c => c.Type == UserJwtClaimType) ?? throw new UserKeyUnauthorizedException();
            var userKey = codeClaim.Value;
            return userKey;
        }
        catch (Exception)
        {
            throw new UserKeyUnauthorizedException();
        }
    }

    public string GetFacilityCodeSelected()
    {
        var facilityCode = httpContextAccessor.HttpContext?.Request.Headers[FacilityCodeHeaderKey].ToString();
        if (string.IsNullOrEmpty(facilityCode))
        {
            throw new FacilityNotfoundException();
        }

        return facilityCode;
    }

    public string GetSiteCodeSelected()
    {
        var siteCodeOfFacility = httpContextAccessor.HttpContext?.Request.Headers[SiteCodeOfFacilityHeaderKey].ToString();
        if (string.IsNullOrEmpty(siteCodeOfFacility))
        {
            throw new SiteNotAlreadyInFacilityException();
        }

        return siteCodeOfFacility;
    }

    public long GetFacilityIdSelected()
    {
        var facilityKey = httpContextAccessor.HttpContext?.Request.Headers[FacilityIdHeaderKey].ToString();
        if (!long.TryParse(facilityKey, out var facilityId) || facilityId <= 0)
        {
            throw new FacilityNotfoundException();
        }

        return facilityId;
    }

    public long GetSiteIdSelected()
    {
        var siteKey = httpContextAccessor.HttpContext?.Request.Headers[SiteIdOfFacilityHeaderKey].ToString();
        if (!long.TryParse(siteKey, out var siteIdOfFacility) || siteIdOfFacility <= 0)
        {
            throw new FacilityNotfoundException();
        }

        return siteIdOfFacility;
    }

    public string GetLanguageCode()
    {
        const string defaultAcceptLanguage = "ja-JP";
        var acceptLanguageKey = httpContextAccessor.HttpContext?.Request.Headers.AcceptLanguage.ToString();
        var language = string.IsNullOrEmpty(acceptLanguageKey) ? defaultAcceptLanguage : acceptLanguageKey.Split(',')[0];
        var languageCode = language.Split('-')[0];
        return languageCode;
    }

    private static bool ValidateJwt(
        string jwt,
        string expectedIssuer
    )
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            if (!tokenHandler.CanReadToken(jwt))
            {
                Console.WriteLine("Invalid JWT user token.");
                return false;
            }

            var token = tokenHandler.ReadJwtToken(jwt);

            var now = DateTime.UtcNow;
            if (token.ValidFrom > now || token.ValidTo < now)
            {
                return false;
            }

            if (!string.IsNullOrEmpty(expectedIssuer))
            {
                var isValidIssuer = ValidateIssuer(
                    tokenHandler,
                    jwt,
                    expectedIssuer
                );

                if (!isValidIssuer)
                {
                    return false;
                }
            }

            var codeClaim = token.Claims.FirstOrDefault(
                    x => x.Type == UserJwtClaimType
                )
                ?.Value;

            return !string.IsNullOrEmpty(codeClaim);
        }
        catch (SecurityTokenExpiredException)
        {
            Console.WriteLine("JWT user has expired.");
        }
        catch (SecurityTokenException ex)
        {
            Console.WriteLine($"Invalid JWT user: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }

        return false;
    }

    private static bool ValidateIssuer(
        JwtSecurityTokenHandler handler,
        string token,
        string expectedIssuer
    )
    {
        var jwtToken = handler.ReadJwtToken(token);
        var issuer = jwtToken.Issuer;
        return string.Equals(issuer, expectedIssuer, StringComparison.OrdinalIgnoreCase);
    }
}
