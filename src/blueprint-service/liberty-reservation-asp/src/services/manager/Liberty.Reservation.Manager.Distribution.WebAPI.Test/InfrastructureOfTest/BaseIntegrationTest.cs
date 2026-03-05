using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Manager.Application.Domains.Repositories.Interfaces;

namespace Liberty.Reservation.Manager.Distribution.WebAPI.Test.InfrastructureOfTest;

[Collection("Sequential")]
public class BaseIntegrationTest : IDisposable
{
    protected const long MockFacilityId = 1;
    protected const string MockFacilityCode = "01JBHTDSKFWB9Q5A8CX397QGET";
    protected const string MockUserKey = "manager-d407fcf7-85ac-4a74-8fae-678f10b0d77b";

    protected readonly AppWebApplicationFactory<TestStartup> Factory;
    protected readonly HttpClient Client;

    protected BaseIntegrationTest()
    {
        Factory = new AppWebApplicationFactory<TestStartup>().WithMockIdentity(MockFacilityCode, MockUserKey);
        Client = Factory.CreateClient();

        _ = MockFacilityProfile(
            MockFacilityId,
            MockFacilityCode
        );
    }

    private Facility MockFacilityProfile(
        long mockFacilityId,
        string mockFacilityCode
    )
    {
        var facilityRepo = Factory.GetRequiredService<IFacilityRepository>();

        var existingFacilities = facilityRepo!.GetQueryableWithAsNoTracking().ToList();
        if (existingFacilities is { Count: > 0 })
        {
            return existingFacilities[0];
        }

        var facility = new Facility
        {
            Id = mockFacilityId,
            Code = mockFacilityCode,
            IsEnabled = true,
            CanOnLinePayment = true,
            CanAddRoomOnModify = true,
            UseDailyPerson = true
        };

        var newFacility = facilityRepo.Add(facility, true);
        return newFacility;
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
