using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Models.Requests;
using Liberty.Reservation.Site.Application.Exceptions;
using Liberty.Reservation.Site.WebAPI.Test.InfrastructureOfTest;
using Liberty.Reservation.Site.WebAPI.Test.InfrastructureOfTest.Utilities;
using Newtonsoft.Json;

namespace Liberty.Reservation.Site.WebAPI.Test.IntegrationTests;

public class CheckNightNumberEndpointReturnBookingInvalidNightNumberIntTest : BaseIntegrationTest
{
    private GetBookingData GetBookingData { get; }
    private const string BaseUrl = "api/booking";

    public CheckNightNumberEndpointReturnBookingInvalidNightNumberIntTest()
    {
        GetBookingData = new GetBookingData(Factory, Client);
    }

    [Fact]
    public async Task CheckNightNumber_BookingInvalidNightNumberException()
    {
        var (_, plan) = await GetBookingData.GetAllBooking();
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
            RestNumber = 111,
            RoomNumber = 1,
            CheckInDate = checkInDate,
            GuestsPerRoom = guestsPerRoom,
            OptionItems = optionRequest
        };
        var request = TestUtil.ToJsonContent(bookingPrinceRequest);
        var response = await Client.PostAsync(
            $"{BaseUrl}/plans/{plan.Id}/rooms/{roomGroup.Id}/check-night-number",
            request
        );

        var json = await response.Content.ReadAsStringAsync();
        try
        {
            var exception = JsonConvert.DeserializeObject<BookingInvalidNightNumberException>(json);

            Assert.NotNull(exception);
        }
        catch (Exception)
        {
            Assert.True(false);
        }
    }
}
