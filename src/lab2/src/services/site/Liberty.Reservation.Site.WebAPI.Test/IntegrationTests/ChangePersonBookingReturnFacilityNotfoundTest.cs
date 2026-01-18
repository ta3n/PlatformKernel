using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Models.Requests;
using Liberty.Reservation.Site.Application.Exceptions;
using Liberty.Reservation.Site.WebAPI.Test.InfrastructureOfTest;
using Liberty.Reservation.Site.WebAPI.Test.InfrastructureOfTest.Utilities;
using Newtonsoft.Json;

namespace Liberty.Reservation.Site.WebAPI.Test.IntegrationTests;

public class ChangePersonBookingEndpointReturnFacilityNotfoundIntTest : BaseIntegrationTest
{
    private const string BaseUrl = "api/booking";

    private GetBookingData GetBookingData { get; }

    public ChangePersonBookingEndpointReturnFacilityNotfoundIntTest() : base("", "")
    {
        GetBookingData = new GetBookingData(Factory, Client);
    }

    [Fact]
    public async Task ChangePersonBooking_FacilityNotfoundException()
    {
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
        var payload = TestUtil.ToJsonContent(bookingPrinceRequest);
        var response = await Client.PostAsync(
            $"{BaseUrl}/plans/1000/rooms/1000/change-persons",
            payload
        );

        var json = await response.Content.ReadAsStringAsync();
        try
        {
            var exception = JsonConvert.DeserializeObject<FacilityNotfoundException>(json);

            Assert.NotNull(exception);
        }
        catch (Exception)
        {
            Assert.True(false);
        }
    }
}
