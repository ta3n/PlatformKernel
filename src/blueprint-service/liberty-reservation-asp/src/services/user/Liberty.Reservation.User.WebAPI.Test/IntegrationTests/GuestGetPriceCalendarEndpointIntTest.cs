using System.Net;
using Liberty.ApplicationShared.Utils;
using Liberty.Entity.ValueObjects;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.User.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.User.WebAPI.Test.InfrastructureOfTest.Utilities;
using ReservationEntity = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation;

namespace Liberty.Reservation.User.WebAPI.Test.IntegrationTests;

public class GuestGetPriceCalendarEndpointIntTest : BaseReservationEndpointIntTest
{
    private static new string BaseUrl => "api/guest/booking";

    [Fact]
    public async Task Guest_GetBookingPriceCalendar_ReturnOk_WithBookingPriceCalendarResponse()
    {
        var plan = await CreatePlanAsync();
        var roomGroup = await CreateRoomGroupAsync();
        var site = await CreateSiteAsync();

        var planRoomGroupRepo = Factory.GetRequiredService<IPlanRoomGroupRepository>();
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
            UseDailyPerson = true
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
            ReservationState = ReservationStatus.Temporary
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

        var optionItemRepo = Factory.GetRequiredService<IOptionItemRepository>();
        var mockOptionItem = new OptionItem
        {
            Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Test" } },
            Description = "Test",
            Code = EntityUtil.CreateCode()
        };
        var optionItem = await optionItemRepo!.AddAsync(mockOptionItem, true);

        var planOptionItemRepo = Factory.GetRequiredService<IPlanOptionItemRepository>();
        var mockPlanOptionItem = new PlanOptionItem
        {
            PlanId = plan.Id,
            OptionItemId = optionItem.Id
        };
        _ = await planOptionItemRepo!.AddAsync(mockPlanOptionItem, true);

        var optionItemAppDateRepo = Factory.GetRequiredService<IOptionItemAppDateRepository>();
        var mockOptionAppDate = new OptionItemAppDate
        {
            OptionItemId = optionItem.Id,
            AppDateId = appDate.Id
        };
        _ = await optionItemAppDateRepo!.AddAsync(mockOptionAppDate, true);

        var guestCode = GetGuestCode(reservation);
        var payload = PayloadBookingPriceRequest();

        var response = await Client.PostAsync(
            $"{BaseUrl}/{guestCode}/change-persons",
            payload
        );

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
