using System.Net;
using Liberty.Reservation.Site.WebAPI.Test.InfrastructureOfTest;

namespace Liberty.Reservation.Site.WebAPI.Test.IntegrationTests;

public class GetBookingDetailsEndpointReturnFacilityNotfoundIntTest : BaseIntegrationTest
{
    private const string BaseUrl = "api/booking";

    private GetBookingData GetBookingData { get; }

    public GetBookingDetailsEndpointReturnFacilityNotfoundIntTest() : base("", "")
    {
        GetBookingData = new GetBookingData(Factory, Client);
    }

    [Fact]
    public async Task GetBookingDetails_ReturnOk_ReturnBookingNotfound()
    {
        const string url = $"{BaseUrl}/plans/1000/rooms/1000";
        var response = await Client.GetAsync(url);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
