using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Manager.Application.Contexts;
using Liberty.Reservation.Manager.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Manager.File.WebAPI.Application.ExternalServices.Membership.Facility.Repositories;
using Liberty.Reservation.Manager.File.WebAPI.Test.InfrastructureOfTest.Utilities;

namespace Liberty.Reservation.Manager.File.WebAPI.Test.InfrastructureOfTest;

[Collection("Sequential")]
public class BaseIntegrationTest : IDisposable
{
    protected readonly AppWebApplicationFactory<TestStartup> Factory;
    protected readonly HttpClient Client;
    protected readonly Facility FacilityInfo;
    private const string MockFacilityCode = "01JBHTDSKFWB9Q5A8CX397QGET";

    protected BaseIntegrationTest()
    {
        Factory = new AppWebApplicationFactory<TestStartup>().WithMockIdentity(MockFacilityCode);
        Client = Factory.CreateClient();

        ResetDatabase();

        FacilityInfo = MockFacilityProfile();
    }

    private void ResetDatabase()
    {
        var dbContext = Factory.GetRequiredService<ManagerDataContext>();
        dbContext?.TruncateAllTablesAsync().GetAwaiter().GetResult();
    }

    private Facility MockFacilityProfile()
    {
        var facilityRepo = Factory.GetRequiredService<IFacilityRepository>();
        var facilityExternalRepo = Factory.GetRequiredService<IFacilityExternalRepository>();

        var existingFacilities = facilityRepo!.GetQueryableWithAsNoTracking().ToList();
        if (existingFacilities is { Count: > 0 })
        {
            return existingFacilities[0];
        }

        var externalFacility = new Application.ExternalServices.Membership.Facility.Models.Facility
        {
            Id = 1,
            Code = MockFacilityCode,
            State = Application.ExternalServices.Membership.Facility.Models.FacilityStates.Public,
            Name = "Test"
        };

        try
        {
            facilityExternalRepo!.Add(externalFacility, true);
        }
        catch
        {
            // ignore
        }

        var facility = new Facility
        {
            Id = externalFacility.Id,
            Code = externalFacility.Code,
            IsEnabled = true,
            CanOnLinePayment = true
        };

        try
        {
            facilityRepo.Add(facility, true);
        }
        catch
        {
            // ignore
        }

        return facility;
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
