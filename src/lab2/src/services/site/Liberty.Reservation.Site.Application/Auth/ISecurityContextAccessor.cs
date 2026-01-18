namespace Liberty.Reservation.Site.Application.Auth;

public interface ISecurityContextAccessor
{
    string? JwtTokenRaw { get; }
    string? IpAddressClient { get; }
    string? TimeZone { get; }
    int TimeZoneOffset { get; }
    string? AcceptLanguage { get; }
    string? FacilityRecordCode { get; }
    string? GetApplicationUserKey();
    string GetFacilityCodeSelected();
    string GetSiteCodeSelected();
    long GetFacilityIdSelected();
    long GetSiteIdSelected();
    string GetLanguageCode();
}
