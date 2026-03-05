using System.Net;
using Liberty.ApplicationShared.Utils;
using Liberty.Entity.ValueObjects;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Manager.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest.Utilities;

namespace Liberty.Reservation.Manager.WebAPI.Test.IntegrationTests;

public class ReservationDetailEndpointReturnReservationNotfoundExceptionIntTest : BaseIntegrationTest
{
    private static string BaseUrl => "api/reservations";
    private MockBookingData MockBookingData { get; }

    public ReservationDetailEndpointReturnReservationNotfoundExceptionIntTest()
    {
        MockBookingData = new MockBookingData(Factory, Client, FacilityInfo);
    }

    [Fact]
    public async Task GetReservationDetails_ReturnReservationNotfoundException()
    {
        var plan = await MockBookingData.CreatePlanAsync();
        var roomGroup = await MockBookingData.CreateRoomGroupAsync();
        var site = await MockBookingData.CreateSiteAsync();

        var planRoomGroupRepo = Factory.GetRequiredService<IPlanRoomGroupRepository>();
        var mockPlanRoom = new PlanRoomGroup
        {
            PlanId = plan.Id,
            RoomGroupId = roomGroup.Id
        };
        _ = await planRoomGroupRepo!.AddAsync(mockPlanRoom, true);

        var reservationRepo = Factory.GetRequiredService<IReservationRepository>();
        var mockReservation = new Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation
        {
            Code = EntityUtil.CreateCode(),
            Facility = FacilityInfo,
            Plan = plan,
            RoomGroup = roomGroup,
            Site = site,
            MainUser = new CustomerInfo { Code = EntityUtil.CreateCode() },
            Reserver = new CustomerInfo { Code = EntityUtil.CreateCode() },
            ReservationDateTime = DateTime.UtcNow,
            CheckInDate = 20240909,
            RestNumber = 1,
            RoomNumber = 1
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

        var response = await Client.GetAsync($"{BaseUrl}/{reservation.Id + 1}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
