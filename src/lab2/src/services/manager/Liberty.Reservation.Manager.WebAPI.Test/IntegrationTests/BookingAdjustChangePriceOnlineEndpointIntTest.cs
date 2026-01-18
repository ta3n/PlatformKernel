using System.Net;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Models.Requests;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest.Utilities;

namespace Liberty.Reservation.Manager.WebAPI.Test.IntegrationTests;

public class BookingOnlineAdjustChangePriceEndpointIntTest : BaseIntegrationTest
{
    private static string BaseUrl => "/api/reservations";
    private MockBookingData MockBookingData { get; }

    public BookingOnlineAdjustChangePriceEndpointIntTest()
    {
        MockBookingData = new MockBookingData(Factory, Client, FacilityInfo);
    }

    [Fact]
    public async Task AdjustBookingOnlinePaymentWithChangePrice_ReturnsOK()
    {
        var (reservations, personAgeTypes) = await MockBookingData.CreateReservationsOnlinePaymentAsync();
        var reservation = reservations.First();
        var orderId = await MockBookingData.CreateOrderReservationAsync(reservation.Id);
        await MockBookingData.CreateSystemConfigAsync();
        await MockBookingData.CreatePaymentOnlineAsync(orderId);

        await AdjustBookingWithChangePrice(personAgeTypes, reservation);
    }

    private async Task AdjustBookingWithChangePrice(
        List<PersonAgeType> personAgeTypes,
        Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation reservation
    )
    {
        var checkInDate = AppDate.GetDateTime(reservation.CheckInDate);
        var checkInTime = DateTime.UtcNow.AddHours(0.5).ToString("HH\\:mm");

        var bookingAdjustReq = new BookingAdjustRequest(
            true,
            checkInTime,
            2,
            2,
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
            [
                new NightPeopleOfReservationAdjustRequest(
                    AppDate.GetId(checkInDate),
                    [
                        new RoomNightOfReservationAdjustRequest(
                            0,
                            [
                                new PeopleOfReservationAdjustRequest(
                                    personAgeTypes[0].Id,
                                    1,
                                    Genders.Male
                                ),
                                new PeopleOfReservationAdjustRequest(
                                    personAgeTypes[0].Id,
                                    1,
                                    Genders.Female
                                )
                            ]
                        ),
                        new RoomNightOfReservationAdjustRequest(
                            1,
                            [
                                new PeopleOfReservationAdjustRequest(
                                    personAgeTypes[0].Id,
                                    1,
                                    Genders.Male
                                ),
                                new PeopleOfReservationAdjustRequest(
                                    personAgeTypes[0].Id,
                                    1,
                                    Genders.Female
                                )
                            ]
                        )
                    ]
                ),
                new NightPeopleOfReservationAdjustRequest(
                    AppDate.GetId(checkInDate.AddDays(1)),
                    [
                        new RoomNightOfReservationAdjustRequest(
                            0,
                            [
                                new PeopleOfReservationAdjustRequest(
                                    personAgeTypes[0].Id,
                                    1,
                                    Genders.Male
                                ),
                                new PeopleOfReservationAdjustRequest(
                                    personAgeTypes[0].Id,
                                    1,
                                    Genders.Female
                                )
                            ]
                        ),
                        new RoomNightOfReservationAdjustRequest(
                            1,
                            [
                                new PeopleOfReservationAdjustRequest(
                                    personAgeTypes[0].Id,
                                    1,
                                    Genders.Male
                                ),
                                new PeopleOfReservationAdjustRequest(
                                    personAgeTypes[0].Id,
                                    1,
                                    Genders.Female
                                )
                            ]
                        )
                    ]
                )
            ],
            null,
            [
                new RoomRepresentativeOfReservationAdjustRequest(
                    0,
                    "Full name 1",
                    "Kana 1"
                ),
                new RoomRepresentativeOfReservationAdjustRequest(
                    1,
                    "Full name 2",
                    "Kana 2"
                )
            ],
            null,
            null
        )
        {
            Id = reservation.Id,
            CheckInDateId = AppDate.GetId(checkInDate)
        };

        var contentReq = TestUtil.ToJsonContent(bookingAdjustReq);
        var response = await Client.PatchAsync(
            $"{BaseUrl}/{reservation.Id}/change-execution",
            contentReq
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();
        Assert.NotNull(responseString);
    }
}
