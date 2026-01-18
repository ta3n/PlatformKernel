using System.Net;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Models.Requests;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest.Utilities;

namespace Liberty.Reservation.Manager.WebAPI.Test.IntegrationTests;

public class BookingAdjustInfoEndpointIntTest : BaseIntegrationTest
{
    private static string BaseUrl => "/api/reservations";
    private MockBookingData MockBookingData { get; }

    public BookingAdjustInfoEndpointIntTest()
    {
        MockBookingData = new MockBookingData(Factory, Client, FacilityInfo);
    }

    [Fact]
    public async Task AdjustBookingNotChangeRoomRepresentativesWithoutChangePrice_ReturnsOK()
    {
        var reservation = (await MockBookingData.CreateReservationsAsync(false)).Reservations.First();
        await MockBookingData.CreateSystemConfigAsync();
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
            $"{BaseUrl}/{reservation.Id}/change-execution",
            TestUtil.ToJsonContent(bookingAdjustReq)
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();
        Assert.NotNull(responseString);
    }

    [Fact]
    public async Task AdjustBookingHasChangingRoomRepresentativesWithoutChangePrice_ReturnsOK()
    {
        var reservation = (await MockBookingData.CreateReservationsAsync()).Reservations.First();
        await MockBookingData.CreateSystemConfigAsync();
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
            $"{BaseUrl}/{reservation.Id}/change-execution",
            TestUtil.ToJsonContent(bookingAdjustReq)
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();
        Assert.NotNull(responseString);
    }
}
