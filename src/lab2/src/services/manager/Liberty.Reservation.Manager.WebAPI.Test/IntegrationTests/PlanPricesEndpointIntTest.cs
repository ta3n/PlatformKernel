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

public class PlanPricesEndpointIntTest : BaseIntegrationTest
{
    private const string BaseUrl = "api/plan-prices";

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
            Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Name}" } },
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
            Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Name}" } },
            Description = "Description",
            Url = "test.com"
        };

        var newSite = await siteRepo!.AddAsync(mockData, true);

        return newSite;
    }

    private async Task<PlanRoomGroupSite> CreateRoomTypeSiteAsync(
        long planId,
        long roomTypeId,
        long siteId
    )
    {
        var planRoomGroupSiteRepo = Factory.GetRequiredService<IPlanRoomGroupSiteRepository>();
        var mockData = new PlanRoomGroupSite
        {
            PlanId = planId,
            RoomGroupId = roomTypeId,
            SiteId = siteId
        };

        var newPlanRoomGroupSite = await planRoomGroupSiteRepo!.AddAsync(mockData, true);

        return newPlanRoomGroupSite;
    }

    [Fact]
    public async Task GetRoomTypeStandardPrice_ReturnOk_WithRoomTypeStandardPriceResponse()
    {
        var roomType = await CreateRoomGroupAsync();
        var site = await CreateSiteAsync();
        var plan = await CreatePlanAsync();

        _ = await CreateRoomTypeSiteAsync(plan.Id, roomType.Id, site.Id);

        var response = await Client.GetAsync(
            $"{BaseUrl}/{plan.Id}/room-groups/{roomType.Id}/destinations/{site.Id}/standard-price"
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();

        Assert.NotNull(responseString);
    }

    [Fact]
    public async Task GetRoomTypeChildrenPrice_ReturnOk_WithRoomTypeChildrenPriceResponse()
    {
        var roomType = await CreateRoomGroupAsync();
        var site = await CreateSiteAsync();
        var plan = await CreatePlanAsync();

        _ = await CreateRoomTypeSiteAsync(plan.Id, roomType.Id, site.Id);

        var response = await Client.GetAsync(
            $"{BaseUrl}/{plan.Id}/room-groups/{roomType.Id}/destinations/{site.Id}/children-price"
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();

        Assert.NotNull(responseString);
    }

    [Fact]
    public async Task GetRoomTypeSale_ReturnOk_WithRoomTypeSaleResponse()
    {
        var roomType = await CreateRoomGroupAsync();
        var site = await CreateSiteAsync();
        var plan = await CreatePlanAsync();

        _ = await CreateRoomTypeSiteAsync(plan.Id, roomType.Id, site.Id);

        var response = await Client.GetAsync(
            $"{BaseUrl}/{plan.Id}/room-groups/{roomType.Id}/destinations/{site.Id}/sale"
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();

        Assert.NotNull(responseString);
    }

    [Fact]
    public async Task GetRoomTypePriceCalendar_ReturnOk_WithRoomTypePriceCalendarResponse()
    {
        var roomType = await CreateRoomGroupAsync();
        var site = await CreateSiteAsync();
        var plan = await CreatePlanAsync();

        _ = await CreateRoomTypeSiteAsync(plan.Id, roomType.Id, site.Id);

        var response = await Client.GetAsync(
            $"{BaseUrl}/{plan.Id}/room-groups/{roomType.Id}/destinations/{site.Id}/price-calendar"
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();

        Assert.NotNull(responseString);
    }

    [Fact]
    public async Task GetRoomTypeDiscount_ReturnOk_WithRoomTypeDiscountResponse()
    {
        var roomType = await CreateRoomGroupAsync();
        var site = await CreateSiteAsync();
        var plan = await CreatePlanAsync();

        _ = await CreateRoomTypeSiteAsync(plan.Id, roomType.Id, site.Id);

        var response = await Client.GetAsync(
            $"{BaseUrl}/{plan.Id}/room-groups/{roomType.Id}/destinations/{site.Id}/discount"
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();

        Assert.NotNull(responseString);
    }
}
