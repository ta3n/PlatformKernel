using System.Net;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Models.Requests;
using Liberty.Reservation.Site.WebAPI.Test.InfrastructureOfTest;
using Liberty.Reservation.Site.WebAPI.Test.InfrastructureOfTest.Utilities;

namespace Liberty.Reservation.Site.WebAPI.Test.IntegrationTests;

public class CheckRoomNumberOfRoomEndpointIntTest : BaseIntegrationTest
{
    private GetBookingData GetBookingData { get; }
    private const string BaseUrl = "api/booking";

    public CheckRoomNumberOfRoomEndpointIntTest()
    {
        GetBookingData = new GetBookingData(Factory, Client);
    }

    [Fact]
    public async Task CheckRoomNumber_ReturnOk_WithResponse()
    {
        var roomGroup = await GetBookingData.GetRoomGroup();
        var optionRequest = new List<OptionOfBookingPriceRequest>
        {
            new()
            {
                AppDateId = AppDate.GetId(DateTime.UtcNow),
                RoomGroupIndex = 0,
                Number = 1,
                OptionItemId = 1
            }
        };
        var checkInDate = AppDate.GetId(DateTime.UtcNow);

        List<PersonOfBookingPriceRequest> guestsPerRoom =
        [
            new()
            {
                AppDateId = checkInDate,
                RestIndex = 0,
                RoomGroupIndex = 0,
                PersonAgeTypeId = 1,
                Persons = 1,
                MalePersons = 1,
                FemalePersons = 0
            }
        ];
        var bookingPrinceRequest = new BookingPriceRequest
        {
            RestNumber = 1,
            RoomNumber = 1,
            CheckInDate = checkInDate,
            GuestsPerRoom = guestsPerRoom,
            OptionItems = optionRequest
        };
        var request = TestUtil.ToJsonContent(bookingPrinceRequest);

        var response = await Client.PostAsync(
            $"{BaseUrl}/rooms/{roomGroup.Id}/check-room-number",
            request
        );
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
