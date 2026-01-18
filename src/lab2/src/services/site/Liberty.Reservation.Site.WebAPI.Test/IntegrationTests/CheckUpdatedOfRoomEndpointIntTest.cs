using System.Net;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Site.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Site.WebAPI.Application.Models.Responses;
using Liberty.Reservation.Site.WebAPI.Test.InfrastructureOfTest;
using Liberty.Reservation.Site.WebAPI.Test.InfrastructureOfTest.Utilities;
using Newtonsoft.Json;

namespace Liberty.Reservation.Site.WebAPI.Test.IntegrationTests;

public class CheckUpdatedOfRoomEndpointIntTest : BaseIntegrationTest
{
    private GetBookingData GetBookingData { get; }
    private const string BaseUrl = "api/booking";

    public CheckUpdatedOfRoomEndpointIntTest()
    {
        GetBookingData = new GetBookingData(Factory, Client);
    }

    [Fact]
    public async Task CheckUpdated_ReturnOk()
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
        var bookingResponse = JsonConvert.DeserializeObject<BookingDetailsResponse>(responseString);

        var request = new CheckChangedRequest(
            bookingResponse!.LastUpdateString,
            []
        );

        response = await Client.PostAsync(
            $"{BaseUrl}/rooms/{roomGroup.Id}/plans/{plan.Id}/check-changed",
            TestUtil.ToJsonContent(request)
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        responseString = await response.Content.ReadAsStringAsync();
        Assert.NotNull(responseString);
    }
}
