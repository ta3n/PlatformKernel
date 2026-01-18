using Liberty.Reservation.Site.Application.Auth;
using Liberty.Reservation.Site.WebAPI.Test.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace Liberty.Reservation.Site.WebAPI.Test.InfrastructureOfTest;

public class MockHttpContextFactory(
    IServiceProvider serviceProvider,
    MockClaimsPrincipalProvider mockClaimsPrincipalProvider
) : IHttpContextFactory
{
    private readonly DefaultHttpContextFactory _delegate = new(serviceProvider);

    public HttpContext Create(
        IFeatureCollection featureCollection
    )
    {
        var httpContext = _delegate.Create(featureCollection);
        httpContext.User = mockClaimsPrincipalProvider.User;
        var facilityCode = mockClaimsPrincipalProvider.User.Claims.SingleOrDefault(x => x.Type == "FacilityCode");
        var siteCodeOfFacility = mockClaimsPrincipalProvider.User.Claims.SingleOrDefault(x => x.Type == "SiteCode");

        httpContext.Request.Headers.Append(SecurityContextAccessor.FacilityCodeHeaderKey, facilityCode!.Value);
        httpContext.Request.Headers.Append(SecurityContextAccessor.SiteCodeOfFacilityHeaderKey, siteCodeOfFacility!.Value);
        httpContext.Request.Headers.Append("Time-Zone-Offset", "9");
        httpContext.Request.Headers.Append("Accept-Language", "en-Us");

        return httpContext;
    }

    public void Dispose(
        HttpContext httpContext
    )
    {
        _delegate.Dispose(httpContext);
    }
}
