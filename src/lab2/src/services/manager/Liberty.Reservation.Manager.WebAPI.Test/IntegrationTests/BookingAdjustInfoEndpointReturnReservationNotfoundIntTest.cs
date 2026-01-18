using System.Net;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Models.Requests;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest.Utilities;

namespace Liberty.Reservation.Manager.WebAPI.Test.IntegrationTests;

public class BookingAdjustInfoEndpointReturnReservationNotfoundIntTest : BaseIntegrationTest
{
    private static string BaseUrl => "/api/reservations";
    private MockBookingData MockBookingData { get; }

    public BookingAdjustInfoEndpointReturnReservationNotfoundIntTest()
    {
        MockBookingData = new MockBookingData(Factory, Client, FacilityInfo);
    }

    [Fact]
    public async Task AdjustBookingNotChangeRoomRepresentativesWithoutChangePrice_ReturnReservationNotfound()
    {
        var reservation = (await MockBookingData.CreateReservationsAsync(false)).Reservations.First();

        var bookingAdjustReq = new BookingAdjustRequest(
            true,
            "14:00",
            1,
            1,
            "Free input",
            new ReserverOfReservationAdjustRequest(
                "Reserver full name",
                "Reserver kana",
                Genders.Male,
                "test@liberty.com",
                "Reserver post code",
                "Country",
                "Reserver address 1",
                "Reserver address 2",
                "Reserver address 3",
                "123456789"
            ),
            new GuestOfReservationAdjustRequest(
                "Main user full name",
                "Main user kana",
                Genders.Male,
                null,
                "Main user post code",
                "Country",
                "Main user address 1",
                "Main user address 2",
                "Main user address 3",
                "123456789"
            ),
            null,
            null,
            null,
            null,
            null
        )
        {
            Id = reservation.Id,
            CheckInDateId = reservation.CheckInDate
        };

        var response = await Client.PatchAsync(
            $"{BaseUrl}/{reservation.Id + 1}/change-execution",
            TestUtil.ToJsonContent(bookingAdjustReq)
        );
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task AdjustBookingHasChangingRoomRepresentativesWithoutChangePrice_ReturnReservationNotfound()
    {
        var reservation = (await MockBookingData.CreateReservationsAsync()).Reservations.First();

        var bookingAdjustReq = new BookingAdjustRequest(
            true,
            "14:00",
            1,
            1,
            "Free input",
            new ReserverOfReservationAdjustRequest(
                "Reserver full name",
                "Reserver kana",
                Genders.Male,
                "test@liberty.com",
                "Reserver post code",
                "Country",
                "Reserver address 1",
                "Reserver address 2",
                "Reserver address 3",
                "123456789"
            ),
            new GuestOfReservationAdjustRequest(
                "Main user full name",
                "Main user kana",
                Genders.Male,
                null,
                "Main user post code",
                "Country",
                "Main user address 1",
                "Main user address 2",
                "Main user address 3",
                "123456789"
            ),
            null,
            null,
            [
                new RoomRepresentativeOfReservationAdjustRequest(
                    0,
                    "Full name 1",
                    "Kana 1"
                )
            ],
            null,
            null
        )
        {
            Id = reservation.Id,
            CheckInDateId = reservation.CheckInDate
        };

        var response = await Client.PatchAsync(
            $"{BaseUrl}/{reservation.Id + 1}/change-execution",
            TestUtil.ToJsonContent(bookingAdjustReq)
        );
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
