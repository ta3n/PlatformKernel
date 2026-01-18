namespace Liberty.Reservation.Manager.Application.Auth;

public interface ISecurityContextAccessor
{
    string? JwtTokenRaw { get; }
    string? IpAddressClient { get; }
    string? TimeZone { get; }
    string? AcceptLanguage { get; }

    string? ApplicationUserKey { get; }
    string? CompactApplicationUserKey { get; }
    long FacilityKey { get; }
    string? FacilityRecordCode { get; }

    string GetFacilityCode();
    string GetLanguageCode();
    string? GetFacilityCodeValue();
}
