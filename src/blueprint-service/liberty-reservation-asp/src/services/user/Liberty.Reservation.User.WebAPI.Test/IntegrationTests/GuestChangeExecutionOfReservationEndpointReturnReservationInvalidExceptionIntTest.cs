using System.Net;
using Liberty.ApplicationShared.Utils;
using Liberty.Entity.ValueObjects;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Application.Models.Requests;
using Liberty.Reservation.User.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.User.WebAPI.Test.InfrastructureOfTest.Utilities;
using ReservationEntity = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation;

namespace Liberty.Reservation.User.WebAPI.Test.IntegrationTests;

public class GuestChangeExecutionOfReservationEndpointReturnReservationInvalidExceptionIntTest : BaseReservationEndpointIntTest
{
    private static new string BaseUrl => "api/guest/booking";

    [Fact]
    public async Task Guest_ChangeExecutionOfReservation_ReturnReservationInvalidException()
    {
        var plan = await CreatePlanAsync();
        var roomGroup = await CreateRoomGroupAsync();
        var site = await CreateSiteAsync();

        var personAgeTypeRepo = Factory.GetRequiredService<IPersonAgeTypeRepository>();
        var mockPersonAgeType = new PersonAgeType
        {
            Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Test" } },
            Code = EntityUtil.CreateCode()
        };
        var personAgeType = await personAgeTypeRepo!.AddAsync(mockPersonAgeType, true);

        var facilityRepo = Factory.GetRequiredService<IFacilityRepository>();
        var mockFacility = new Facility
        {
            Code = EntityUtil.CreateCode(),
            IsEnabled = true
        };
        var facility = await facilityRepo!.AddAsync(mockFacility, true);

        var facilityPersonAgeRepo = Factory.GetRequiredService<IFacilityPersonAgeTypeRepository>();
        var facilityPersonAge = new FacilityPersonAgeType
        {
            FacilityId = facility.Id,
            PersonAgeTypeId = personAgeType.Id
        };
        await facilityPersonAgeRepo!.AddAsync(facilityPersonAge, true);
        var reservationRepo = Factory.GetRequiredService<IReservationRepository>();
        var mockReservation = new ReservationEntity
        {
            Code = EntityUtil.CreateCode(),
            FacilityId = facility.Id,
            SiteId = site.Id,
            PlanId = plan.Id,
            RoomGroupId = roomGroup.Id,
            UserCode = MockUserCode,
            MainUser = new CustomerInfo { Code = EntityUtil.CreateCode() },
            Reserver = new CustomerInfo { Code = EntityUtil.CreateCode() },
            ReservationDateTime = DateTime.UtcNow,
            CheckInDate = AppDate.GetId(DateTime.UtcNow),
            RestNumber = 1,
            RoomNumber = 1,
            ReservationState = ReservationStatus.Temporary,
            IsEnabled = true
        };
        var reservation = await reservationRepo!.AddAsync(mockReservation, true);

        var appDateRepo = Factory.GetRequiredService<IAppDateRepository>();
        var mockDate = new AppDate
        {
            DateTime = DateTime.UtcNow,
            Code = EntityUtil.CreateCode()
        };
        var appDate = await appDateRepo!.AddAsync(mockDate, true);

        var reservationRoomAppDatePersonRepo =
            Factory.GetRequiredService<IReservationRoomGroupAppDatePersonAgeTypeRepository>();
        var mockReservationRoomAppDatePerson = new ReservationRoomGroupAppDatePersonAgeType
        {
            ReservationId = reservation.Id,
            RoomGroupId = roomGroup.Id,
            BookingDateId = appDate.Id,
            RestIndex = 0,
            RoomGroupIndex = 0,
            MaleNumber = 0,
            FemaleNumber = 1,
            GenderNoneNumber = 0,
            UnitPrice = 10,
            PersonAgeTypeId = personAgeType.Id
        };

        var checkInDate = DateTime.Now;

        _ = await reservationRoomAppDatePersonRepo!.AddAsync(mockReservationRoomAppDatePerson, true);

        var guestCode = GetGuestCode(reservation);

        var bookingAdjustReq = new BookingAdjustRequest(
            true,
            "14:00",
            2,
            2,
            "Free input",
            new ReserverOfReservationAdjustRequest(
                "Reserver full name",
                "Reserver kana",
                Genders.None,
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
                null,
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
                                    personAgeType.Id,
                                    1,
                                    Genders.Male
                                ),
                                new PeopleOfReservationAdjustRequest(
                                    personAgeType.Id,
                                    1,
                                    Genders.Female
                                )
                            ]
                        ),
                        new RoomNightOfReservationAdjustRequest(
                            1,
                            [
                                new PeopleOfReservationAdjustRequest(
                                    personAgeType.Id,
                                    1,
                                    Genders.Male
                                ),
                                new PeopleOfReservationAdjustRequest(
                                    personAgeType.Id,
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
                                    personAgeType.Id,
                                    1,
                                    Genders.Male
                                ),
                                new PeopleOfReservationAdjustRequest(
                                    personAgeType.Id,
                                    1,
                                    Genders.Female
                                )
                            ]
                        ),
                        new RoomNightOfReservationAdjustRequest(
                            1,
                            [
                                new PeopleOfReservationAdjustRequest(
                                    personAgeType.Id,
                                    1,
                                    Genders.Male
                                ),
                                new PeopleOfReservationAdjustRequest(
                                    personAgeType.Id,
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
        );

        var response = await Client.PatchAsync(
            $"{BaseUrl}/{guestCode}/change-execution",
            TestUtil.ToJsonContent(bookingAdjustReq)
        );

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
