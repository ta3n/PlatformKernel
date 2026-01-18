using Liberty.ApplicationShared.Utils;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Site.Application.Domains.Repositories.Interfaces;
using SiteEntity = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.Site;
using FacilityMembership = Liberty.Reservation.Site.WebAPI.Application.ExternalServices.Membership.Facility.Models.Facility;
using Liberty.Entity.Utils;
using Liberty.Entity.ValueObjects;
using Liberty.Reservation.Site.WebAPI.Test.InfrastructureOfTest.Utilities;

namespace Liberty.Reservation.Site.WebAPI.Test.InfrastructureOfTest;

public class MockInitData(
    AppWebApplicationFactory<TestStartup> factory
)
{
    public async Task<Facility> CreateFacilityAsync(
        FacilityMembership facilityMembership
    )
    {
        var facilityRepo = factory.GetRequiredService<IFacilityRepository>();

        var facility = await facilityRepo!.GetByIdAsync(facilityMembership.Id);
        if (facility is not null)
        {
            facility.Code = facilityMembership.Code;
            facility.IsEnabled = true;
            return await facilityRepo.UpdateAsync(facility, true);
        }

        var mockFacility = new Facility
        {
            Id = 10,
            Description = "Test",
            Code = facilityMembership.Code,
            IsEnabled = true
        };
        var newFacility = await facilityRepo.AddAsync(mockFacility, true);

        newFacility.IsEnabled = true;
        _ = await facilityRepo.UpdateAsync(mockFacility, true);

        return newFacility;
    }

    public async Task<Plan> CreatePlanAsync()
    {
        var planRepo = factory.GetRequiredService<IPlanRepository>();
        var mockPlan = new Plan
        {
            Name = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Name" } },
            Description = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Description" } },
            IsOnSidePayment = true,
            NumberOfStayLimitMax = 10,
            NumberOfStayLimitMin = 1,
            Code = EntityUtil.CreateCode(),
            IsOnLinePayment = true,
            Cancellation = new Cancellation
            {
                IsEnabled = true,
                Code = EntityUtil.CreateCode()
            }
        };
        var newPlan = await planRepo!.AddAsync(mockPlan, true);

        newPlan.IsEnabled = true;
        var plan = await planRepo.UpdateAsync(newPlan, true);

        return plan;
    }

    public async Task<RoomGroup> CreateRoomGroupAsync()
    {
        var roomGroupRepo = factory.GetRequiredService<IRoomGroupRepository>();
        var mockRoomGroup = new RoomGroup
        {
            Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Test" } },
            Description = new MultilingualText { { TestUtil.DefaultLanguageCode, "Test" } },
            BaseNumber = 1,
            RoomGroupSizeUnitType = RoomGroupSizeUnitTypes.M2,
            Code = EntityUtil.CreateCode()
        };
        var newRoomGroup = await roomGroupRepo!.AddAsync(mockRoomGroup, true);

        newRoomGroup.IsEnabled = true;
        var roomGroup = await roomGroupRepo.UpdateAsync(newRoomGroup, true);

        return roomGroup;
    }

    public async Task<SiteEntity> CreateSiteAsync()
    {
        var siteRepo = factory.GetRequiredService<ISiteRepository>();
        var mockSite = new SiteEntity
        {
            Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Test" } },
            Description = "Test",
            Code = EntityUtil.CreateCode()
        };
        var newSite = await siteRepo!.AddAsync(mockSite, true);

        newSite.IsEnabled = true;
        var site = await siteRepo.UpdateAsync(newSite, true);

        return site;
    }

    public async Task CreatePlanRelationsAsync(
        Facility facility,
        SiteEntity site,
        Plan plan,
        RoomGroup roomGroup
    )
    {
        var facilityPlanRepo = factory.GetRequiredService<IFacilityPlanRepository>();
        var facilityPlan = new FacilityPlan
        {
            PlanId = plan.Id,
            FacilityId = facility.Id
        };
        _ = await facilityPlanRepo!.AddAsync(facilityPlan, true);

        var planRoomGroupRepo = factory.GetRequiredService<IPlanRoomGroupRepository>();
        var planRoomGroup = new PlanRoomGroup
        {
            PlanId = plan.Id,
            RoomGroupId = roomGroup.Id
        };
        _ = await planRoomGroupRepo!.AddAsync(planRoomGroup, true);

        var planRoomGroupSiteRepo = factory.GetRequiredService<IPlanRoomGroupSiteRepository>();
        var planRoomGroupSite = new PlanRoomGroupSite
        {
            PlanId = plan.Id,
            RoomGroupId = roomGroup.Id,
            SiteId = site.Id
        };
        _ = await planRoomGroupSiteRepo!.AddAsync(planRoomGroupSite, true);
    }
}
