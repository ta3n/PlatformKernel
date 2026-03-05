using Liberty.ApplicationShared.Utils;
using Liberty.Reservation.Site.Application.Contexts;
using Liberty.Reservation.Site.File.WebAPI.Application.ExternalServices.Membership.Facility.Models;
using Liberty.Reservation.Site.File.WebAPI.Application.ExternalServices.Membership.Facility.Repositories;
using Liberty.Reservation.Site.File.WebAPI.Test.InfrastructureOfTest.Utilities;

namespace Liberty.Reservation.Site.File.WebAPI.Test.InfrastructureOfTest;

[Collection("Sequential")]
public class BaseIntegrationTest : IDisposable
{
    protected readonly AppWebApplicationFactory<TestStartup> Factory;
    protected readonly HttpClient Client;
    protected readonly Facility FacilityInfo;

    protected BaseIntegrationTest()
    {
        Factory = new AppWebApplicationFactory<TestStartup>();
        Client = Factory.CreateClient();

        ResetDatabase();

        FacilityInfo = MockExternalFacility();
    }

    private void ResetDatabase()
    {
        var dbContext = Factory.GetRequiredService<SiteDataContext>();
        dbContext?.TruncateAllTablesAsync().GetAwaiter().GetResult();
    }

    private Facility MockExternalFacility()
    {
        var facilityExternalRepo = Factory.GetRequiredService<IFacilityExternalRepository>();

        var facility = facilityExternalRepo!
            .GetQueryableWithAsNoTracking()
            .SingleOrDefault(x => x.Id == 1);
        if (facility is not null)
        {
            return facility;
        }

        var mockData = new Facility
        {
            Id = 1,
            Code = EntityUtil.CreateCode(),
            State = FacilityStates.Public
        };

        try
        {
            facilityExternalRepo.Add(mockData, true);
        }
        catch
        {
            // ignore
        }

        return mockData;
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
