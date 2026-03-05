using System.Net;
using Liberty.ApplicationShared.Utils;
using Liberty.Entity.ValueObjects;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Employee.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Employee.WebAPI.Application.ExternalServices.Membership.Facility.Repositories;
using Liberty.Reservation.Employee.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Employee.WebAPI.Test.InfrastructureOfTest;
using Liberty.Reservation.Employee.WebAPI.Test.InfrastructureOfTest.Utilities;

namespace Liberty.Reservation.Employee.WebAPI.Test.IntegrationTests;

public class FacilitiesEndpointIntTest : BaseIntegrationTest
{
    private const string BaseUrl = "api/facilities";

    private async Task<long> CreateFacilityAsync(
        long facilityId
    )
    {
        var facilityRepository = Factory.GetRequiredService<IFacilityRepository>();

        var mockData = new Facility
        {
            Id = facilityId,
            Code = EntityUtil.CreateCode()
        };
        var newFacility = await facilityRepository!.AddAsync(mockData, true);

        return newFacility.Id;
    }

    private async Task<long> CreateMembershipFacilityAsync()
    {
        long facilityId = new Random().Next(2, 1000);
        var facilityRepository = Factory.GetRequiredService<IFacilityExternalRepository>();

        var mockData = new Application.ExternalServices.Membership.Facility.Models.Facility
        {
            Id = facilityId,
            Code = EntityUtil.CreateCode()
        };
        var newFacility = await facilityRepository!.AddAsync(mockData, true);

        return newFacility.Id;
    }

    private async Task<Site> CreateSiteAsync()
    {
        var siteRepo = Factory.GetRequiredService<ISiteRepository>();
        var mockData = new Site
        {
            Code = EntityUtil.CreateCode(),
            Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Test" } },
            Description = "Test"
        };

        var newSite = await siteRepo!.AddAsync(mockData, true);

        return newSite;
    }

    [Fact]
    public async Task UpdateFacilityExits_ReturnNoContent()
    {
        var membershipFacilityId = await CreateMembershipFacilityAsync();
        var facilityId = await CreateFacilityAsync(membershipFacilityId);

        var updateReq = new FacilityUpdateRequest(
            [],
            false,
            "test@gmail.com",
            "test"
        ) { Id = facilityId };

        var response = await Client.PutAsync(
            $"{BaseUrl}/{facilityId}",
            TestUtil.ToJsonContent(updateReq)
        );
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task EnableFacilityExits_ReturnNoContent()
    {
        var membershipFacilityId = await CreateMembershipFacilityAsync();
        var facilityId = await CreateFacilityAsync(membershipFacilityId);

        var enableReq = new FacilityEnabledRequest(
            true
        );

        var response = await Client.PatchAsync(
            $"{BaseUrl}/{facilityId}/enable",
            TestUtil.ToJsonContent(enableReq)
        );
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task EnableFacilityNotExits_ReturnNoContent()
    {
        var facilityId = await CreateMembershipFacilityAsync();

        var enableReq = new FacilityEnabledRequest(
            true
        );

        var response = await Client.PatchAsync(
            $"{BaseUrl}/{facilityId}/enable",
            TestUtil.ToJsonContent(enableReq)
        );
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task GetAllFacility_ReturnOK_WithFacilities()
    {
        var membershipFacilityId = await CreateMembershipFacilityAsync();
        await CreateFacilityAsync(membershipFacilityId);

        var response = await Client.GetAsync(
            $"{BaseUrl}"
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();
        Assert.NotEmpty(responseString);
    }

    [Fact]
    public async Task GetFacility_ReturnOK_WithFacility()
    {
        var membershipFacilityId = await CreateMembershipFacilityAsync();
        var facilityId = await CreateFacilityAsync(membershipFacilityId);

        var response = await Client.GetAsync(
            $"{BaseUrl}/{facilityId}"
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();
        Assert.NotEmpty(responseString);
    }

    [Fact]
    public async Task GetAllDestinationsOfFacility_ReturnOK_WithListDestinationResponse()
    {
        var membershipFacilityId = await CreateMembershipFacilityAsync();
        var facilityId = await CreateFacilityAsync(membershipFacilityId);
        var site = await CreateSiteAsync();

        var facilitySiteRepo = Factory.GetRequiredService<IFacilitySiteRepository>();
        var facilitySite = new FacilitySite
        {
            FacilityId = facilityId,
            SiteId = site.Id
        };
        _ = await facilitySiteRepo!.AddAsync(facilitySite, true);

        var response = await Client.GetAsync(
            $"{BaseUrl}/{facilityId}/destinations"
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();
        Assert.NotEmpty(responseString);
    }
}
