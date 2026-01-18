using Liberty.ApplicationShared.Utils;
using Liberty.Entity.ValueObjects;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Site.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Site.WebAPI.Application.ExternalServices.Membership.Facility.Models;
using Liberty.Reservation.Site.WebAPI.Application.ExternalServices.Membership.Facility.Repositories;
using Liberty.Reservation.Site.WebAPI.Test.InfrastructureOfTest.Utilities;
using Microsoft.EntityFrameworkCore;
using Facility = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.Facility;

namespace Liberty.Reservation.Site.WebAPI.Test.InfrastructureOfTest;

public static class MockConst
{
    public const string MockFacilityCode = "11JBHTGN78M8T6S8A0689VXGWS";
    public const string MockSiteCodeOfFacility = "sitetest";
}

[Collection("Sequential")]
public class BaseIntegrationTest : IDisposable
{
    protected readonly AppWebApplicationFactory<TestStartup> Factory;
    protected readonly HttpClient Client;
    protected readonly Facility FacilityInfo;
    protected readonly Reservation.Application.Contexts.DataContexts.Entities.Data.Site SiteInfo;

    protected BaseIntegrationTest() : this(MockConst.MockFacilityCode, MockConst.MockSiteCodeOfFacility)
    {
    }

    protected BaseIntegrationTest(
        string mockFacilityCode,
        string mockSiteCodeOfFacility
    )
    {
        Factory = new AppWebApplicationFactory<TestStartup>().WithMockIdentity(mockFacilityCode, mockSiteCodeOfFacility);
        Client = Factory.CreateClient();

        FacilityInfo = MockFacilityProfile(MockConst.MockFacilityCode);
        SiteInfo = MockSiteOfFacility(FacilityInfo.Id, MockConst.MockSiteCodeOfFacility);
    }

    private Facility MockFacilityProfile(
        string mockFacilityCode
    )
    {
        var facilityRepo = Factory.GetRequiredService<IFacilityRepository>();
        var facilityExternalRepo = Factory.GetRequiredService<IFacilityExternalRepository>();
        var existingFacilities = facilityRepo!.GetQueryableWithAsNoTracking().ToList();

        var existingFacility = facilityRepo
            .GetQueryableWithAsNoTracking()
            .SingleOrDefault(x => x.Code == mockFacilityCode);
        if (existingFacility is not null)
        {
            return existingFacility;
        }

        var externalFacility = new Application.ExternalServices.Membership.Facility.Models.Facility
        {
            Id = 1,
            Code = mockFacilityCode,
            State = FacilityStates.Public,
            Name = "Test",
            Address = new Address
            {
                Id = 2,
                Code = EntityUtil.CreateCode(),
                Address1 = "abc",
                Address2 = "abc",
                Address3 = "abc",
                Address4 = "abc"
            }
        };

        try
        {
            facilityExternalRepo!.Add(externalFacility, true);
        }
        catch
        {
            // ignored
        }

        if (existingFacilities is { Count: > 0 })
        {
            var facilityExist = existingFacilities[0];
            facilityExist.Code = mockFacilityCode;
            facilityExist.IsEnabled = true;
            facilityExist.CanOnLinePayment = true;
            facilityExist.IsOnLinePayment = true;
            facilityExist.IsOnSidePayment = true;

            try
            {
                facilityRepo.Update(facilityExist, true);
            }
            catch
            {
                // ignored
            }

            return facilityExist;
        }

        var facility = new Facility
        {
            Id = externalFacility.Id,
            Code = externalFacility.Code,
            IsEnabled = true,
            CanOnLinePayment = true,
            IsOnLinePayment = true,
            IsOnSidePayment = true
        };

        try
        {
            facilityRepo.Add(facility, true);
        }
        catch
        {
            // ignored
        }

        return facility;
    }

    private Reservation.Application.Contexts.DataContexts.Entities.Data.Site MockSiteOfFacility(
        long facilityId,
        string mockSiteCode
    )
    {
        var facilitySiteRepo = Factory.GetRequiredService<IFacilitySiteRepository>()!;
        var siteRepo = Factory.GetRequiredService<ISiteRepository>()!;

        var existingSiteOfFacility = facilitySiteRepo
            .GetQueryableWithAsNoTracking()
            .Include(x => x.Site)
            .SingleOrDefault(x => x.FacilityId == facilityId);
        if (existingSiteOfFacility?.Site is not null)
        {
            return existingSiteOfFacility.Site;
        }

        var existingSites = siteRepo
            .GetQueryableWithAsNoTracking()
            .ToList();
        if (existingSites is { Count: > 0 })
        {
            var site = existingSites[0] ?? throw new InvalidOperationException("Existing facility site has a null Site property.");
            return site;
        }

        var siteOfFacility = new FacilitySite
        {
            FacilityId = facilityId,
            Site = new()
            {
                Code = mockSiteCode,
                Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Test Site" } },
                IsEnabled = true
            },
            IsEnabled = true
        };

        try
        {
            facilitySiteRepo.Add(siteOfFacility, true);
        }
        catch
        {
            // ignored
        }

        return siteOfFacility.Site;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(
        bool disposing
    )
    {
        if (!disposing)
        {
            return;
        }

        Client.Dispose();
        Factory.Dispose();
    }
}

[CollectionDefinition("Sequential")]
public class SequentialCollection
    : ICollectionFixture<AppWebApplicationFactory<TestStartup>>;
