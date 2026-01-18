using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Exceptions;
using Liberty.Reservation.Application.Models.Requests;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest.Utilities;
using Newtonsoft.Json;

namespace Liberty.Reservation.Manager.WebAPI.Test.IntegrationTests;

public class BookingCheckNumberOfRoomsEndpointReturnReservationNotfoundExceptionIntTest : BaseIntegrationTest
{
    private static string BaseUrl => "api/reservations";
    private MockBookingData MockBookingData { get; }

    public BookingCheckNumberOfRoomsEndpointReturnReservationNotfoundExceptionIntTest()
    {
        MockBookingData = new MockBookingData(Factory, Client, FacilityInfo);
    }

    [Fact]
    public async Task CheckNumberOfRooms_ReturnReservationNotfoundException()
    {
        var (reservations, personAgeTypes) = await MockBookingData.CreateReservationsAsync();
        _ = await MockBookingData.CreateRoomGroupAppDateAsync();
        var reservation = reservations.First();
        var personAgeType = personAgeTypes[0];

        var checkInDate = AppDate.GetId(DateTime.UtcNow);

        List<PersonOfBookingPriceRequest> guestsPerRoom =
        [
            new()
            {
                AppDateId = checkInDate,
                RestIndex = 0,
                RoomGroupIndex = 0,
                PersonAgeTypeId = personAgeType.Id,
                Persons = 1,
                MalePersons = 1,
                FemalePersons = 1
            }
        ];
        var bookingPrinceRequest = new BookingPriceRequest
        {
            RestNumber = 1,
            RoomNumber = 1,
            CheckInDate = checkInDate,
            GuestsPerRoom = guestsPerRoom,
            OptionItems = null
        };
        var payload = TestUtil.ToJsonContent(bookingPrinceRequest);

        var response = await Client.PostAsync(
            $"{BaseUrl}/{reservation.Id + 1}/check-room-number",
            payload
        );

        var json = await response.Content.ReadAsStringAsync();
        try
        {
            var exception = JsonConvert.DeserializeObject<ReservationNotfoundException>(json);

            Assert.NotNull(exception);
        }
        catch (Exception)
        {
            Assert.True(false);
        }
    }
}
