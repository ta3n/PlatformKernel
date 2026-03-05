using System.Net;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Models.Requests;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest.Utilities;

namespace Liberty.Reservation.Manager.WebAPI.Test.IntegrationTests;

public class ReservationPriceCalendarEndpointReturnReservationNotFoundIntTest : BaseIntegrationTest
{
    private static string BaseUrl => "api/reservations";
    private MockBookingData MockBookingData { get; }

    public ReservationPriceCalendarEndpointReturnReservationNotFoundIntTest()
    {
        MockBookingData = new MockBookingData(Factory, Client, FacilityInfo);
    }

    [Fact]
    public async Task GetReservationPriceCalendar_ReturnReservationNotFound()
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
            $"{BaseUrl}/{reservation.Id + 99}/change-persons",
            payload
        );

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
