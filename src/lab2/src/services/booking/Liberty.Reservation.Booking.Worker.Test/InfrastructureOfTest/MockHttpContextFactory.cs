using Liberty.Reservation.Booking.Worker.Test.Configration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace Liberty.Reservation.Booking.Worker.Test.InfrastructureOfTest;

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
        return httpContext;
    }

    public void Dispose(
        HttpContext httpContext
    )
    {
        _delegate.Dispose(httpContext);
    }
}
