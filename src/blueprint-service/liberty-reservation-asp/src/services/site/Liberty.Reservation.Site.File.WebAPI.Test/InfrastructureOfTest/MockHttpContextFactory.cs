using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace Liberty.Reservation.Site.File.WebAPI.Test.InfrastructureOfTest;

public class MockHttpContextFactory(
    IServiceProvider serviceProvider
) : IHttpContextFactory
{
    private readonly DefaultHttpContextFactory _delegate = new(serviceProvider);

    public HttpContext Create(
        IFeatureCollection featureCollection
    )
    {
        var httpContext = _delegate.Create(featureCollection);
        return httpContext;
    }

    public void Dispose(
        HttpContext httpContext
    )
    {
        _delegate.Dispose(httpContext);
    }
}
