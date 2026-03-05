using System.Net;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Site.WebAPI.Test.InfrastructureOfTest;

namespace Liberty.Reservation.Site.WebAPI.Test.IntegrationTests;

public class BookingGetOptionItemEndpointIntTest : BaseIntegrationTest
{
    private static string BaseUrl => "api/booking";

    private GetBookingData GetBookingData { get; }

    public BookingGetOptionItemEndpointIntTest()
    {
        GetBookingData = new GetBookingData(Factory, Client);
    }

    [Fact]
    public async Task GetAllOptionItemsOfPlan_ReturnOk_WithOptionItemOfBookingResponse()
    {
        var planInfo = await GetBookingData.GetPlan();
        var appDate = AppDate.GetId(DateTime.Now);
        var url = $"{BaseUrl}/plans/{planInfo.Id}/option-items?AppDate={appDate}";
        var response = await Client.GetAsync($"{url}");
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var responseString = await response.Content.ReadAsStringAsync();
        Assert.NotNull(responseString);
    }
}
