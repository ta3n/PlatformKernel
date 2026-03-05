using System.Net;
using Liberty.Reservation.Site.WebAPI.Test.InfrastructureOfTest;

namespace Liberty.Reservation.Site.WebAPI.Test.IntegrationTests;

public class GetFacilityBookingEndpointReturnFacilityNotfoundIntTest() : BaseIntegrationTest("", "")
{
    private const string BaseUrl = "api/booking";

    [Fact]
    public async Task GetFacilityBooking_ReturnFacilityNotFound()
    {
        var response = await Client.GetAsync($"{BaseUrl}/facility");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
