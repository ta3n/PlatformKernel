namespace PlatformKernel.Service.Application.Auth;

public interface ISecurityContextAccessor
{
    string? JwtTokenRaw { get; }
    string? IpAddressClient { get; }
    string? TimeZone { get; }
    string? AcceptLanguage { get; }

    string? ApplicationUserKey { get; }
    string? CompactApplicationUserKey { get; }
}
