using System.Net;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Site.WebAPI.Test.InfrastructureOfTest;

namespace Liberty.Reservation.Site.WebAPI.Test.IntegrationTests;

public class GetRoomDetailsOfBookingForRoomEndpointIntTest : BaseIntegrationTest
{
    private const string BaseUrl = "api/booking";

    private GetBookingData GetBookingData { get; }

    public GetRoomDetailsOfBookingForRoomEndpointIntTest()
    {
        GetBookingData = new GetBookingData(Factory, Client);
    }

    [Fact]
    public async Task GetBookingDetails_ReturnOk_WithBookingDetailsResponse()
    {
        var plan = await GetBookingData.GetPlan(false, PlanTypes.RoomOnly);
        var roomGroup = await GetBookingData.GetRoomGroup();
        var url = $"{BaseUrl}/rooms/{roomGroup.Id}/plans/{plan.Id}";
        var response = await Client.GetAsync(
            url
        );
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var responseString = await response.Content.ReadAsStringAsync();

        Assert.NotNull(responseString);
    }
}
