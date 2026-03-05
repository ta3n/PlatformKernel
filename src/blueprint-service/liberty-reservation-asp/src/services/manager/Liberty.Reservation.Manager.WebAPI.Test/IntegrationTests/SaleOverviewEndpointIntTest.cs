using System.Net;
using Liberty.ApplicationShared.Utils;
using Liberty.Entity.ValueObjects;
using Liberty.Entity.Utils;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Manager.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest.Utilities;

namespace Liberty.Reservation.Manager.WebAPI.Test.IntegrationTests;

public class SaleOverviewEndpointIntTest : BaseIntegrationTest
{
    private static string BaseUrl => "api/sales";

    private async Task<Plan> CreatePlanAsync()
    {
        var planRepo = Factory.GetRequiredService<IPlanRepository>();
        var facilityPlanService = Factory.GetRequiredService<IFacilityPlanService>();

        var mockData = new Plan
        {
            Name = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Name" } },
            Description = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Description" } }
        };
        var newPlan = await planRepo!.AddAsync(mockData, true);

        var mockFacilityPlan = new FacilityPlan
        {
            PlanId = newPlan.Id,
            FacilityId = FacilityInfo.Id
        };
        _ = await facilityPlanService!.CreateAsync(mockFacilityPlan);

        return newPlan;
    }

    private async Task<RoomGroup> CreateRoomGroupAsync()
    {
        var roomGroupRepo = Factory.GetRequiredService<IRoomGroupRepository>();

        var mockData = new RoomGroup
        {
            Code = EntityUtil.CreateCode(),
            Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Name" } },
            Description = new MultilingualText { { TestUtil.DefaultLanguageCode, "Description" } }
        };

        var newRoomGroup = await roomGroupRepo!.AddAsync(mockData, true);

        return newRoomGroup;
    }

    private async Task<Site> CreateSiteAsync()
    {
        var siteRepo = Factory.GetRequiredService<ISiteRepository>();
        var mockData = new Site
        {
            Code = EntityUtil.CreateCode(),
            Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Name" } },
            Description = "Description",
            Url = "test.com"
        };

        var newSite = await siteRepo!.AddAsync(mockData, true);

        return newSite;
    }

    [Fact]
    public async Task GetSaleOverview_ReturnOK_WithSaleOverviewResponse()
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
            ReservationDateTime = DateTime.UtcNow
        };
        _ = await reservationRepo!.AddAsync(mockReservation, true);

        var response = await Client.GetAsync($"{BaseUrl}/overview?startAppDateId=20240909&endAppDateId=20240909");

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();
        Assert.NotNull(responseString);
    }
}
