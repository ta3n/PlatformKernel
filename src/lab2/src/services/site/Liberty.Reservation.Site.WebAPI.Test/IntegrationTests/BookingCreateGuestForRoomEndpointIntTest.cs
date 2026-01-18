using System.Net;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Application.Models.Requests;
using Liberty.Reservation.Site.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Site.WebAPI.Test.InfrastructureOfTest;
using Liberty.Reservation.Site.WebAPI.Test.InfrastructureOfTest.Utilities;

namespace Liberty.Reservation.Site.WebAPI.Test.IntegrationTests;

public class BookingCreateGuestForRoomEndpointIntTest : BaseIntegrationTest
{
    private static string BaseUrl => "api/booking";

    private GetBookingData GetBookingData { get; }

    public BookingCreateGuestForRoomEndpointIntTest()
    {
        GetBookingData = new GetBookingData(Factory, Client);
    }

    [Fact]
    public async Task CreateBooking_ReturnOk_WithBookingResponse()
    {
        var (_, personAgeTypes) = await GetBookingData.GetReservationsAsync();
        var roomGroup = await GetBookingData.GetRoomGroup();
        var checkInDate = DateTime.Now;
        var planRoomGroupRepo = Factory.GetRequiredService<IPlanRoomGroupRepository>()
            ?? throw new ArgumentException(nameof(IPlanRoomGroupRepository));

        var planRoomGroups = await planRoomGroupRepo.GetAllAsync();
        var planRoomGroupUpdates = new List<PlanRoomGroup>();
        foreach (var planRoomGroup in planRoomGroups)
        {
            planRoomGroup.IsEnabled = true;
            planRoomGroupUpdates.Add(planRoomGroup);
        }

        _ = await planRoomGroupRepo.UpdateRangeAsync(planRoomGroupUpdates, true);

        var bookingAdjustReq = new BookingAdjustRequest(
            true,
            "14:00",
            2,
            2,
            "Free input",
            new ReserverOfReservationAdjustRequest(
                "Reserver full name",
                "Reserver kana",
                Genders.Male,
                "test@liberty.com",
                "014574",
                "Country",
                "Reserver address 1",
                "Reserver address 2",
                "Reserver address 3",
                "0214155455"
            ),
            new GuestOfReservationAdjustRequest(
                "Main user full name",
                "Main user kana",
                Genders.Male,
                AppDate.GetId(
                    new(
                        1993,
                        1,
                        1,
                        0,
                        0,
                        0,
                        DateTimeKind.Local
                    )
                ),
                "014574",
                "Country",
                "Main user address 1",
                "Main user address 2",
                "Main user address 3",
                "0214155455"
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
        ) { CheckInDateId = AppDate.GetId(checkInDate) };

        var payload = new SiteBookingCreateRequest(
            AppDate.GetId(checkInDate),
            "12:00",
            "16:00",
            PaymentTypes.OnLinePayment,
            bookingAdjustReq,
            null,
            null
        );
        var url = BaseUrl + $"/rooms/{roomGroup.Id}";
        var contentReq = TestUtil.ToJsonContent(payload);
        var response = await Client.PostAsync(url, contentReq);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
