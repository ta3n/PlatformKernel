using System.Net;
using Liberty.ApplicationShared.Utils;
using Liberty.Entity.ValueObjects;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.User.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.User.WebAPI.Application.Models.Requests;
using Liberty.Reservation.User.WebAPI.Test.InfrastructureOfTest.Utilities;
using ReservationEntity = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation;

namespace Liberty.Reservation.User.WebAPI.Test.IntegrationTests;

public class ConfirmReservationOfGuestReservationEndpointIntTest : BaseReservationEndpointIntTest
{
    private static new string BaseUrl => "api/guest/booking";

    [Fact]
    public async Task Guest_ConfirmReservation_ReturnOK()
    {
        var plan = await CreatePlanAsync();
        var roomGroup = await CreateRoomGroupAsync();
        var site = await CreateSiteAsync();

        var planRoomGroupRepo = Factory.GetRequiredService<IPlanRoomGroupRepository>();
        var roomGroupAppDateRepo = Factory.GetRequiredService<IRoomGroupAppDateRepository>();
        var mockRoomGroupAppDate = new RoomGroupAppDate
        {
            RoomGroupId = roomGroup.Id,
            SellNumber = 10,
            AppDate = new AppDate
            {
                DateTime = DateTime.UtcNow,
                Id = AppDate.GetId(DateTime.UtcNow)
            },
            IsEnabled = true
        };

        await roomGroupAppDateRepo!.AddAsync(mockRoomGroupAppDate, true);

        var mockPlanRoom = new PlanRoomGroup
        {
            PlanId = plan.Id,
            RoomGroupId = roomGroup.Id
        };
        _ = await planRoomGroupRepo!.AddAsync(mockPlanRoom, true);

        var facilityRepo = Factory.GetRequiredService<IFacilityRepository>();
        var mockFacility = new Facility
        {
            Code = EntityUtil.CreateCode(),
            IsEnabled = true
        };
        var facility = await facilityRepo!.AddAsync(mockFacility, true);

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
            IsEnabled = true,
            ReservationPlanRoomGroupAppDates =
            [
                new()
                {
                    BookingDateId = AppDate.GetId(DateTime.Now),
                    PlanId = plan.Id,
                    RoomGroupId = roomGroup.Id
                }
            ]
        };
        var reservation = await reservationRepo!.AddAsync(mockReservation, true);

        var appDateRepo = Factory.GetRequiredService<IAppDateRepository>();

        var planRoomGroupSiteAppDatePriceDataRepository =
            Factory.GetRequiredService<IPlanRoomGroupSiteAppDatePriceDataRepository>();

        var planRoomGroupSiteAppDatePriceData = new PlanRoomGroupSiteAppDatePriceData
        {
            SiteId = site.Id,
            PlanId = plan.Id,
            RoomGroupId = roomGroup.Id,
            DateCalendar = AppDate.GetId(DateTime.UtcNow),
            IsEnabled = true,
            PriceData = new PriceData
            {
                Price = 100,
                IsEnabled = true,
                Code = EntityUtil.CreateCode()
            }
        };

        await planRoomGroupSiteAppDatePriceDataRepository!.AddAsync(planRoomGroupSiteAppDatePriceData, true);

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

        var bookingConfirmRequest = new BookingConfirmRequest { Id = reservation.Id };

        var response = await Client.PatchAsync(
            $"{BaseUrl}/{guestCode}/confirm",
            TestUtil.ToJsonContent(bookingConfirmRequest)
        );

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        var responseString = await response.Content.ReadAsStringAsync();
        Assert.NotNull(responseString);
    }
}
