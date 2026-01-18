using Liberty.Reservation.Manager.WebAPI.Test.Configuration;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest;

public class MockHttpContextFactory(
    IServiceProvider serviceProvider,
    MockClaimsPrincipalProvider mockClaimsPrincipalProvider
)
    : IHttpContextFactory
{
    private readonly DefaultHttpContextFactory _delegate = new(serviceProvider);

    public HttpContext Create(
        IFeatureCollection featureCollection
    )
    {
        var httpContext = _delegate.Create(featureCollection);
        httpContext.User = mockClaimsPrincipalProvider.User;

        httpContext.Request.Headers.AcceptLanguage = TestUtil.DefaultAcceptLanguage;

        var facilityKey = mockClaimsPrincipalProvider.User.Claims.SingleOrDefault(x => x.Type == "FacilityCode");
        httpContext.Request.Headers.Append("X-Facility-Key", facilityKey!.Value);

        return httpContext;
    }

    public void Dispose(
        HttpContext httpContext
    )
    {
        _delegate.Dispose(httpContext);
    }
}
