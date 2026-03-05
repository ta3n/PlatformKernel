using System.Net;
using Liberty.Reservation.Site.WebAPI.Test.InfrastructureOfTest;

namespace Liberty.Reservation.Site.WebAPI.Test.IntegrationTests;

public class GetBookingDetailsEndpointIntTest : BaseIntegrationTest
{
    private const string BaseUrl = "api/booking";

    private GetBookingData GetBookingData { get; }

    public GetBookingDetailsEndpointIntTest()
    {
        GetBookingData = new GetBookingData(Factory, Client);
    }

    [Fact]
    public async Task GetBookingDetails_ReturnOk_WithBookingDetailsResponse()
    {
        var roomGroup = await GetBookingData.GetRoomGroup();
        var plan = await GetBookingData.GetPlan();
        var url = $"{BaseUrl}/plans/{plan.Id}/rooms/{roomGroup.Id}";
        var response = await Client.GetAsync(
            url
        );
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var responseString = await response.Content.ReadAsStringAsync();

        Assert.NotNull(responseString);
    }
}
