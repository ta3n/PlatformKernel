using System.Net;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Models.Requests;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest.Utilities;

namespace Liberty.Reservation.Manager.WebAPI.Test.IntegrationTests;

public class BookingCheckNumberOfRoomsEndpointIntTest : BaseIntegrationTest
{
    private static string BaseUrl => "api/reservations";
    private MockBookingData MockBookingData { get; }

    public BookingCheckNumberOfRoomsEndpointIntTest()
    {
        MockBookingData = new MockBookingData(Factory, Client, FacilityInfo);
    }

    [Fact]
    public async Task CheckNumberOfRooms_ReturnOK()
    {
        var (reservations, personAgeTypes) = await MockBookingData.CreateReservationsAsync();
        _ = await MockBookingData.CreateRoomGroupAppDateAsync();
        var reservation = reservations.First();
        var personAgeType = personAgeTypes[0];

        var checkInDate = AppDate.GetId(DateTime.UtcNow.AddDays(1));

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
            $"{BaseUrl}/{reservation.Id}/check-room-number",
            payload
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();
        Assert.NotNull(responseString);
    }
}
