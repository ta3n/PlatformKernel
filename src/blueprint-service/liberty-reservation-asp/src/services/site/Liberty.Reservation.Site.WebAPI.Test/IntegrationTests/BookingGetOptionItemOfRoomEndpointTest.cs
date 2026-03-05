using System.Net;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Site.WebAPI.Test.InfrastructureOfTest;

namespace Liberty.Reservation.Site.WebAPI.Test.IntegrationTests;

public class BookingGetOptionItemForRoomEndpointIntTest : BaseIntegrationTest
{
    private static string BaseUrl => "api/booking";

    private GetBookingData GetBookingData { get; }

    public BookingGetOptionItemForRoomEndpointIntTest()
    {
        GetBookingData = new GetBookingData(Factory, Client);
    }

    [Fact]
    public async Task GetAllOptionItemsOfPlan_ReturnOk_WithOptionItemOfBookingResponse()
    {
        var plan = await GetBookingData.GetPlan(false, PlanTypes.RoomOnly);
        var roomInfo = await GetBookingData.GetRoomGroup();
        var appDate = AppDate.GetId(DateTime.Now);
        var url = $"{BaseUrl}/rooms/{roomInfo.Id}/plans/{plan.Id}/option-items?AppDate={appDate}";
        var response = await Client.GetAsync($"{url}");
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var responseString = await response.Content.ReadAsStringAsync();
        Assert.NotNull(responseString);
    }
}
