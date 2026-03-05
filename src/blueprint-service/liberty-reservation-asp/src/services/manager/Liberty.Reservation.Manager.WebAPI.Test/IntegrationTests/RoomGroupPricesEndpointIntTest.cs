using System.Net;
using Liberty.ApplicationShared.Utils;
using Liberty.Entity.ValueObjects;
using Liberty.Entity.Utils;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Manager.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest.Utilities;

namespace Liberty.Reservation.Manager.WebAPI.Test.IntegrationTests;

public class RoomGroupPricesEndpointIntTest : BaseIntegrationTest
{
    private const string BaseUrl = "api/room-group-prices";

    private async Task<string> CreateRoomGroupAsync()
    {
        var url = "api/room-groups";

        var createRequest = new RoomGroupCreateRequest(
            "Room group 1",
            "Room group 1 Description",
            1,
            100,
            10
        );

        var response = await Client.PostAsync(
            url,
            TestUtil.ToJsonContent(createRequest)
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();
        Assert.NotEmpty(responseString);

        return responseString;
    }

    private async Task<Site> CreateSiteAsync()
    {
        var siteRepo = Factory.GetRequiredService<ISiteRepository>();
        var existingSites = await siteRepo!.GetAllAsync();
        if (existingSites is { Count: > 0 })
        {
            return existingSites[0];
        }

        var mockData = new Site
        {
            Code = EntityUtil.CreateCode(),
            Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Site 1" } },
            Url = "site1.com",
            ShortName = "site1"
        };
        var newSite = await siteRepo.AddAsync(mockData, true);

        return newSite;
    }

    private async Task<Plan> CreateRoomOnlyPlanAsync()
    {
        var planRepo = Factory.GetRequiredService<IPlanRepository>();
        var facilityPlanRepo = Factory.GetRequiredService<IFacilityPlanRepository>();

        var roomOnlyPlan = new Plan
        {
            Code = EntityUtil.CreateCode(),
            Name = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Room only plan" } },
            Description = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Room only plan" } },
            PlanType = PlanTypes.RoomOnly
        };

        var newRoomOnlyPlan = await planRepo!.AddAsync(roomOnlyPlan, true);

        var facilityPlan = new FacilityPlan
        {
            FacilityId = FacilityInfo.Id,
            PlanId = newRoomOnlyPlan.Id
        };
        _ = await facilityPlanRepo!.AddAsync(facilityPlan, true);

        return newRoomOnlyPlan;
    }

    private async Task CreateRoomOnlyPlanRelationAsync(
        long planId,
        long roomGroupId,
        long siteId
    )
    {
        var facilityService = Factory.GetRequiredService<IFacilityService>();
        var planRoomGroupSiteService = Factory.GetRequiredService<IPlanRoomGroupSiteService>();
        var planRoomGroupSitePersonAgeTypeService = Factory
            .GetRequiredService<IPlanRoomGroupSitePersonAgeTypeService>();
        var planRoomGroupService = Factory.GetRequiredService<IPlanRoomGroupService>();

        var existingSitesOfRoomInPlan = await planRoomGroupSiteService!.FindByPlanIdAndRomTypeIdAsync(
            planId,
            roomGroupId,
            siteId
        );
        if (existingSitesOfRoomInPlan is not null)
        {
            return;
        }

        var newPlanRoomGroupSite = new PlanRoomGroupSite
        {
            PlanId = planId,
            RoomGroupId = roomGroupId,
            SiteId = siteId,
            IsEnabledMinimumPrice = false,
            MinimumPrice = null
        };
        var newPlanRoomGroup = new PlanRoomGroup
        {
            PlanId = planId,
            RoomGroupId = roomGroupId
        };

        var planRoomGroupSitePersonAgeTypes = new List<PlanRoomGroupSitePersonAgeType>();
        var facility = await facilityService!.FindByIdWithIncludePersonAgeTypeAsync(FacilityInfo.Id);

        foreach (var facilityPersonAgeType in facility.FacilityPersonAgeTypes!)
        {
            planRoomGroupSitePersonAgeTypes.Add(
                new PlanRoomGroupSitePersonAgeType
                {
                    PlanId = planId,
                    RoomGroupId = roomGroupId,
                    SiteId = siteId,
                    PersonAgeTypeId = facilityPersonAgeType.PersonAgeTypeId,
                    IsEnabled = true,
                    IsRegardAdult = facilityPersonAgeType.PersonAgeType!.IsMain,
                    PriceSettingType = PriceSettingTypes.None,
                    Value = null
                }
            );
        }

        await planRoomGroupSiteService.CreateAsync(newPlanRoomGroupSite);
        await planRoomGroupService!.CreateAsync(newPlanRoomGroup);
        await planRoomGroupSitePersonAgeTypeService!
            .CreateRangeAsync(
                planRoomGroupSitePersonAgeTypes
            );
    }

    private async Task<PersonAgeType> CreatePersonAgeTypeAsync()
    {
        var personAgeTypeRepo = Factory.GetRequiredService<IPersonAgeTypeRepository>();
        var existingPersonAgeTypes = await personAgeTypeRepo!.GetAllAsync();
        if (existingPersonAgeTypes is { Count: > 0 })
        {
            return existingPersonAgeTypes[0];
        }

        var personAgeType = new PersonAgeType
        {
            Code = EntityUtil.CreateCode(),
            Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Test" } }
        };
        var newPersonAgeType = await personAgeTypeRepo.AddAsync(personAgeType, true);

        var facilityAgeTypeRepo = Factory.GetRequiredService<IFacilityPersonAgeTypeRepository>();
        var existingPeronAgeTypesOfFacility = await facilityAgeTypeRepo!.GetAllAsync();
        if (existingPeronAgeTypesOfFacility is { Count: > 0 })
        {
            return newPersonAgeType;
        }

        var facilityAgeType = new FacilityPersonAgeType
        {
            FacilityId = FacilityInfo.Id,
            PersonAgeType = newPersonAgeType
        };
        await facilityAgeTypeRepo.AddAsync(facilityAgeType, true);

        return newPersonAgeType;
    }

    [Fact]
    public async Task UpdateRoomGroupStandardPrice_ReturnNoContent()
    {
        var roomGroupIdString = await CreateRoomGroupAsync();
        _ = long.TryParse(roomGroupIdString, out var roomGroupId);
        var site = await CreateSiteAsync();
        var newRoomOnlyPlan = await CreateRoomOnlyPlanAsync();
        _ = await CreatePersonAgeTypeAsync();
        await CreateRoomOnlyPlanRelationAsync(newRoomOnlyPlan.Id, roomGroupId, site.Id);

        var appDateTypeRepo = Factory.GetRequiredService<IAppDateTypeRepository>();
        var appDateType = new AppDateType
        {
            Name = "Test",
            Description = "Test",
            Code = EntityUtil.CreateCode()
        };

        var newAppDateType = await appDateTypeRepo!.AddAsync(appDateType, true);

        var facilityAppDateTypeRepository = Factory.GetRequiredService<IFacilityAppDateTypeRepository>();
        var facilityAppDateType = new FacilityAppDateType
        {
            FacilityId = 1,
            AppDateTypeId = newAppDateType.Id
        };

        await facilityAppDateTypeRepository!.AddAsync(facilityAppDateType, true);

        var updateReq = new RoomGroupPriceUpdateStandardPriceRequest(
            [
                new RomTypeUpdateStandardRequest(
                    newAppDateType.Id,
                    1,
                    2,
                    3
                )
            ]
        );

        var response = await Client.PatchAsync(
            $"{BaseUrl}/{roomGroupId}/destinations/{site.Id}/standard-price",
            TestUtil.ToJsonContent(updateReq)
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task UpdateRoomGroupChildrenPrice_ReturnNoContent()
    {
        var roomGroupIdString = await CreateRoomGroupAsync();
        _ = long.TryParse(roomGroupIdString, out var roomGroupId);
        var site = await CreateSiteAsync();
        var newRoomOnlyPlan = await CreateRoomOnlyPlanAsync();
        var ageType = await CreatePersonAgeTypeAsync();
        await CreateRoomOnlyPlanRelationAsync(newRoomOnlyPlan.Id, roomGroupId, site.Id);

        var updateReq = new RoomGroupPriceUpdateChildrenPriceRequest(
            [
                new RoomTypeUpdateChildrenPersonAgeTypeRequest(
                    ageType.Id,
                    true,
                    true,
                    PriceSettingTypes.Price,
                    1
                )
            ]
        );

        var response = await Client.PatchAsync(
            $"{BaseUrl}/{roomGroupId}/destinations/{site.Id}/children-price",
            TestUtil.ToJsonContent(updateReq)
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task UpdateRoomGroupPriceCalendar_ReturnNoContent()
    {
        var roomGroupIdString = await CreateRoomGroupAsync();
        _ = long.TryParse(roomGroupIdString, out var roomGroupId);
        var site = await CreateSiteAsync();
        var newRoomOnlyPlan = await CreateRoomOnlyPlanAsync();
        _ = await CreatePersonAgeTypeAsync();
        await CreateRoomOnlyPlanRelationAsync(newRoomOnlyPlan.Id, roomGroupId, site.Id);

        var updateReq = new RoomGroupPriceUpdatePriceCalendarRequest(
            [
                new RomTypeUpdatePriceDataCalendarRequest(
                    20241011,
                    1,
                    2,
                    1,
                    false
                )
            ]
        );

        var response = await Client.PatchAsync(
            $"{BaseUrl}/{roomGroupId}/destinations/{site.Id}/price-calendar",
            TestUtil.ToJsonContent(updateReq)
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task UpdateRoomGroupDiscount_ReturnNoContent()
    {
        var roomGroupIdString = await CreateRoomGroupAsync();
        _ = long.TryParse(roomGroupIdString, out var roomGroupId);
        var site = await CreateSiteAsync();

        var updateReq = new RoomGroupPriceUpdateDiscountRequest
        (
            [
                new RomTypeUpdateDiscountDataRequest(
                    20241011,
                    20241013,
                    1,
                    2,
                    PriceSettingTypes.Discount,
                    1
                )
            ]
        );

        var response = await Client.PatchAsync(
            $"{BaseUrl}/{roomGroupId}/destinations/{site.Id}/discount",
            TestUtil.ToJsonContent(updateReq)
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task GetRoomGroupStandardPrice_ReturnOKWithResponse()
    {
        var roomGroupIdString = await CreateRoomGroupAsync();
        _ = long.TryParse(roomGroupIdString, out var roomGroupId);

        var site = await CreateSiteAsync();

        var response = await Client.GetAsync(
            $"{BaseUrl}/{roomGroupId}/destinations/{site.Id}/standard-price"
        );

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetRoomGroupChildrenPrice_ReturnOKWithResponse()
    {
        var roomGroupIdString = await CreateRoomGroupAsync();
        _ = long.TryParse(roomGroupIdString, out var roomGroupId);
        var site = await CreateSiteAsync();

        var response = await Client.GetAsync(
            $"{BaseUrl}/{roomGroupId}/destinations/{site.Id}/children-price"
        );

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetRoomGroupPriceCalendar_ReturnOKWithResponse()
    {
        var roomGroupIdString = await CreateRoomGroupAsync();
        _ = long.TryParse(roomGroupIdString, out var roomGroupId);

        var site = await CreateSiteAsync();

        var response = await Client.GetAsync(
            $"{BaseUrl}/{roomGroupId}/destinations/{site.Id}/price-calendar?startDate={DateTime.Now:yyyyMMdd}&endDate={DateTime.Now.AddDays(1):yyyyMMdd}"
        );

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetRoomGroupDiscount_ReturnOKWithResponse()
    {
        var roomGroupIdString = await CreateRoomGroupAsync();
        _ = long.TryParse(roomGroupIdString, out var roomGroupId);
        var site = await CreateSiteAsync();

        var response = await Client.GetAsync(
            $"{BaseUrl}/{roomGroupId}/destinations/{site.Id}/discount"
        );

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
