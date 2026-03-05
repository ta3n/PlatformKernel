using System.Net;
using Liberty.Reservation.Site.WebAPI.Test.InfrastructureOfTest;

namespace Liberty.Reservation.Site.WebAPI.Test.IntegrationTests;

public class GetFacilityBookingEndpointIntTest : BaseIntegrationTest
{
    private const string BaseUrl = "api/booking";

    [Fact]
    public async Task GetFacilityBooking_ReturnOk_WithBookingFacilityResponse()
    {
        var response = await Client.GetAsync(
            $"{BaseUrl}/facility"
        );
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var responseString = await response.Content.ReadAsStringAsync();

        Assert.NotNull(responseString);
    }
}
