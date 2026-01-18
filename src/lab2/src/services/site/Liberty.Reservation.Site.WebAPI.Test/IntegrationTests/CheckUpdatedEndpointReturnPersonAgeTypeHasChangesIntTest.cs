using Liberty.Reservation.Site.Application.Exceptions;
using Liberty.Reservation.Site.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Site.WebAPI.Test.InfrastructureOfTest;
using Liberty.Reservation.Site.WebAPI.Test.InfrastructureOfTest.Utilities;
using Newtonsoft.Json;

namespace Liberty.Reservation.Site.WebAPI.Test.IntegrationTests;

public class CheckUpdatedEndpointReturnPersonAgeTypeHasChangesIntTest : BaseIntegrationTest
{
    private GetBookingData GetBookingData { get; }
    private const string BaseUrl = "api/booking";

    public CheckUpdatedEndpointReturnPersonAgeTypeHasChangesIntTest()
    {
        GetBookingData = new GetBookingData(Factory, Client);
    }

    [Fact]
    public async Task CheckUpdated_PersonAgeTypeHasChangesException()
    {
        // var facilityRepo = Factory.GetRequiredService<IFacilityRepository>();
        // var facilityUpdatedAt = await facilityRepo!.GetFacilityUpdatedTimeAvailableAsync(FacilityInfo.Id);
        var (_, plan) = await GetBookingData.GetAllBooking();
        var roomGroup = await GetBookingData.GetRoomGroup();
        // var site = await GetBookingData.GetSite();
        var request = new CheckChangedRequest(
            "",
            []
        );

        var response = await Client.PostAsync(
            $"{BaseUrl}/plans/{plan.Id}/rooms/{roomGroup.Id}/check-changed",
            TestUtil.ToJsonContent(request)
        );

        var json = await response.Content.ReadAsStringAsync();
        try
        {
            var exception = JsonConvert.DeserializeObject<PersonAgeTypeHasChangesException>(json);

            Assert.NotNull(exception);
        }
        catch (Exception)
        {
            Assert.True(false);
        }
    }
}
