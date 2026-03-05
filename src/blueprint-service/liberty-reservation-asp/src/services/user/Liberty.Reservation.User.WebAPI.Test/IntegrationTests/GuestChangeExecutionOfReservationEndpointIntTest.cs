using Liberty.ApplicationShared.Utils;
using Liberty.Entity.Utils;
using Liberty.Entity.ValueObjects;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Application.Models.Requests;
using Liberty.Reservation.User.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.User.WebAPI.Test.InfrastructureOfTest.Utilities;
using ReservationEntity = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation;

namespace Liberty.Reservation.User.WebAPI.Test.IntegrationTests;

public class GuestChangeExecutionOfReservationEndpointIntTest : BaseReservationEndpointIntTest
{
    private static new string BaseUrl => "api/guest/booking";

    [Fact]
    public async Task Guest_ChangeExecution_ReturnOK()
    {
        var facilityRepo = Factory.GetRequiredService<IFacilityRepository>();
        var mockFacility = new Facility
        {
            Id = 1,
            Code = EntityUtil.CreateCode()
        };
        var facility = await facilityRepo!.AddAsync(mockFacility, true);

        var planRepo = Factory.GetRequiredService<IPlanRepository>();

        var plan = new Plan
        {
            Name = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Name" } },
            Description = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Description" } },
            NumberOfStayLimitMin = 1,
            NumberOfStayLimitMax = int.MaxValue,
            IsOnLinePayment = true,
            IsOnSidePayment = true,
            CancelLimit = new TimeSpan(12, 0, 0),
            FacilityPlans = new List<FacilityPlan> { new() { FacilityId = facility.Id } },
            Cancellation = new()
            {
                Description = new MultilingualText { { TestUtil.DefaultLanguageCode, "Test" } },
                Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Test" } },
                CancellationCancellationDatas =
                [
                    new()
                    {
                        CancellationData = new()
                        {
                            Code = EntityUtil.CreateCode(),
                            DayStart = 0,
                            DayEnd = 10,
                            Rate = 1
                        }
                    }
                ]
            }
        };
        await planRepo!.AddAsync(plan, true);
        var roomGroup = await CreateRoomGroupAsync();
        var site = await CreateSiteAsync();

        var planRoomGroupRepo = Factory.GetRequiredService<IPlanRoomGroupRepository>();
        var mockPlanRoom = new PlanRoomGroup
        {
            PlanId = plan.Id,
            RoomGroupId = roomGroup.Id
        };
        _ = await planRoomGroupRepo!.AddAsync(mockPlanRoom, true);
        var reservationRepository = Factory.GetRequiredService<IReservationRepository>();
        _ = reservationRepository!.GetQueryableWithAsNoTracking().ToList();

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
            CheckInDate = AppDate.GetId(DateTime.UtcNow.AddDays(10)),
            RestNumber = 1,
            RoomNumber = 1,
            ReservationState = ReservationStatus.Reserved,
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

        var personAgeTypeRepo = Factory.GetRequiredService<IPersonAgeTypeRepository>();
        var mockPersonAgeType = new PersonAgeType
        {
            Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Test" } },
            Code = EntityUtil.CreateCode(),
            AgeMax = 10,
            AgeMin = 1
        };
        var personAgeType = await personAgeTypeRepo!.AddAsync(mockPersonAgeType, true);

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

        _ = await reservationRoomAppDatePersonRepo!.AddAsync(mockReservationRoomAppDatePerson, true);

        var guestCode = GetGuestCode(reservation);

        var bookingAdjustReq = new BookingAdjustRequest(
            true,
            "14:00",
            1,
            1,
            "Free input",
            new ReserverOfReservationAdjustRequest(
                "Reserver full name",
                "Reserver kana",
                Genders.None,
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
            $"{BaseUrl}/{guestCode}/change-execution",
            TestUtil.ToJsonContent(bookingAdjustReq)
        );

        Assert.NotNull(response);
    }
}
