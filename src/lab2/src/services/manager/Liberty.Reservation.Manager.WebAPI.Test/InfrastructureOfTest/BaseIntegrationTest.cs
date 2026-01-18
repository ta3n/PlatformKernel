using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Manager.Application.Contexts;
using Liberty.Reservation.Manager.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Manager.WebAPI.Application.ExternalServices.Membership.Facility.Repositories;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest.Utilities;
using DataSeeder = Liberty.Reservation.Manager.WebAPI.Test.Configuration.DataSeeder;

namespace Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest;

[Collection("Sequential")]
public class BaseIntegrationTest : IDisposable
{
    protected readonly AppWebApplicationFactory<TestStartup> Factory;
    protected readonly HttpClient Client;
    protected readonly Facility FacilityInfo;
    private const string MockFacilityCode = "01JBHTGN78M8T6S8A0689VXGWS";

    protected BaseIntegrationTest()
    {
        Factory = new AppWebApplicationFactory<TestStartup>().WithMockIdentity(MockFacilityCode);
        Client = Factory.CreateClient();

        ResetDatabase();

        FacilityInfo = MockFacilityProfile(MockFacilityCode);
    }

    private void ResetDatabase()
    {
        var dbContext = Factory.GetRequiredService<ManagerDataContext>();
        if (dbContext is null)
        {
            return;
        }

        dbContext.TruncateAllTablesAsync().GetAwaiter().GetResult();
        var projectDir = GetProjectRootPath(AppContext.BaseDirectory);
        var seedDataPath = Path.Combine(projectDir, "..", "Liberty.Reservation.Manager.WebAPI", "wwwroot", "SeedData");
        DataSeeder.SeedingAsync(seedDataPath, dbContext).GetAwaiter().GetResult();
    }

    private Facility MockFacilityProfile(
        string mockFacilityCode
    )
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
            Code = mockFacilityCode,
            State = Application.ExternalServices.Membership.Facility.Models.FacilityStates.Public,
            Name = "Test",
            Kana = "Kana Test",
            Email = "Test"
        };

        try
        {
            facilityExternalRepo!.Add(externalFacility, true);
        }
        catch
        {
            // ignored
        }

        var facility = new Facility
        {
            Id = externalFacility.Id,
            Code = externalFacility.Code,
            IsEnabled = true,
            CanOnLinePayment = true,
            CanAddRoomOnModify = true,
            UseDailyPerson = true,
            IsOnLinePayment = true,
            IsOnSidePayment = true,
            IsExtendedStayOnModify = true
        };

        var newFacility = facilityRepo.Add(facility, true);
        return newFacility;
    }

    /// <summary>
    /// Walks up directories from <paramref name="startPath"/> until a directory containing
    /// <paramref name="markerFolder"/> or a .csproj file is found. Returns the found directory
    /// full path, or the original start path if nothing is found.
    /// </summary>
    private static string GetProjectRootPath(
        string startPath,
        string markerFolder = "SeedDataJson"
    )
    {
        var dir = new DirectoryInfo(startPath);
        while (dir is not null)
        {
            if (dir.EnumerateDirectories(markerFolder).Any() || dir.EnumerateFiles("*.csproj").Any())
            {
                return dir.FullName;
            }

            dir = dir.Parent;
        }

        return startPath;
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
