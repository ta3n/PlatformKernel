using System.Net;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Site.WebAPI.Test.InfrastructureOfTest;

namespace Liberty.Reservation.Site.WebAPI.Test.IntegrationTests;

public class BookingGetOptionItemEndpointReturnFacilityNotfoundIntTest : BaseIntegrationTest
{
    private static string BaseUrl => "api/booking";

    private GetBookingData GetBookingData { get; }

    public BookingGetOptionItemEndpointReturnFacilityNotfoundIntTest() : base("", "")
    {
        GetBookingData = new GetBookingData(Factory, Client);
    }

    [Fact]
    public async Task GetAllOptionItemsOfPlan_ReturnFacilityNotFound()
    {
        var planInfo = await GetBookingData.GetPlan();
        var appDate = AppDate.GetId(DateTime.Now.AddYears(1));
        var url = $"{BaseUrl}/plans/{planInfo.Id}/option-items?AppDate={appDate}";

        var response = await Client.GetAsync(url);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
