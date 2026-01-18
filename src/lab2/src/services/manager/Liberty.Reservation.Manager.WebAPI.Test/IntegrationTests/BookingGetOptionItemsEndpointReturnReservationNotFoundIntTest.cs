using System.Net;
using Liberty.ApplicationShared.Utils;
using Liberty.Entity.ValueObjects;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Manager.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest.Utilities;

namespace Liberty.Reservation.Manager.WebAPI.Test.IntegrationTests;

public class BookingGetOptionItemsEndpointReturnReservationNotFoundIntTest : BaseIntegrationTest
{
    private static string BaseUrl => "api/reservations";
    private MockBookingData MockBookingData { get; }

    public BookingGetOptionItemsEndpointReturnReservationNotFoundIntTest()
    {
        MockBookingData = new MockBookingData(Factory, Client, FacilityInfo);
    }

    [Fact]
    public async Task GetAllOptionItems_ReturnReservationNotFound_WithOptionItemOfPlanResponse()
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
            FacilityId = FacilityInfo.Id,
            SiteId = site.Id,
            PlanId = plan.Id,
            RoomGroupId = roomGroup.Id,
            MainUser = new CustomerInfo { Code = EntityUtil.CreateCode() },
            Reserver = new CustomerInfo { Code = EntityUtil.CreateCode() },
            ReservationDateTime = DateTime.UtcNow,
            CheckInDate = 20240909,
            RestNumber = 1,
            RoomNumber = 1,
            IsEnabled = true
        };
        var reservation = await reservationRepo!.AddAsync(mockReservation, true);

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

        var appDateRepo = Factory.GetRequiredService<IAppDateRepository>();
        var mockDate = new AppDate
        {
            Id = AppDate.GetId(DateTime.UtcNow.AddDays(TestUtil.RandomInt(1, 20))),
            Code = EntityUtil.CreateCode()
        };
        mockDate.DateTime = AppDate.GetDateTime(mockDate.Id);
        var appDate = await appDateRepo!.AddAsync(mockDate, true);

        var optionItemAppDateRepo = Factory.GetRequiredService<IOptionItemAppDateRepository>();
        var mockOptionAppDate = new OptionItemAppDate
        {
            OptionItemId = optionItem.Id,
            AppDateId = appDate.Id
        };
        _ = await optionItemAppDateRepo!.AddAsync(mockOptionAppDate, true);

        var response = await Client.GetAsync($"{BaseUrl}/{reservation.Id + 1}/option-items");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
