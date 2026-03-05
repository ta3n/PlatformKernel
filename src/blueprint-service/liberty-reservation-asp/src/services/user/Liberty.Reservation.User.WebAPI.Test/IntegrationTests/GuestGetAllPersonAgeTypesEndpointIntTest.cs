using System.Net;
using Liberty.ApplicationShared.Utils;
using Liberty.Entity.ValueObjects;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.User.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.User.WebAPI.Test.InfrastructureOfTest.Utilities;

namespace Liberty.Reservation.User.WebAPI.Test.IntegrationTests;

public class GuestGetAllPersonAgeTypesEndpointIntTest : BaseReservationEndpointIntTest
{
    private static new string BaseUrl => "api/guest/booking";

    [Fact]
    public async Task Guest_GetAllPersonAgeTypes_ReturnOK_WithPersonAgeTypes()
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
        var mockReservation = new Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation
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
            RoomNumber = 1
        };
        var reservation = await reservationRepo!.AddAsync(mockReservation, true);

        var guestCode = GetGuestCode(reservation);

        var response = await Client.GetAsync($"{BaseUrl}/{guestCode}/person-age-types");

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();
        Assert.NotNull(responseString);
    }
}
