using Liberty.Reservation.Employee.Application.Contexts;
using Liberty.Reservation.Employee.WebAPI.Test.InfrastructureOfTest.Utilities;
using DataSeeder = Liberty.Reservation.Employee.WebAPI.Test.Configuration.DataSeeder;

namespace Liberty.Reservation.Employee.WebAPI.Test.InfrastructureOfTest;

[Collection("Sequential")]
public class BaseIntegrationTest : IDisposable
{
    protected readonly AppWebApplicationFactory<TestStartup> Factory;
    protected readonly HttpClient Client;

    protected BaseIntegrationTest()
    {
        Factory = new AppWebApplicationFactory<TestStartup>().WithMockIdentity("test");
        Client = Factory.CreateClient();

        ResetDatabase();
    }

    private void ResetDatabase()
    {
        var dbContext = Factory.GetRequiredService<EmployeeDataContext>();
        if (dbContext is null)
        {
            return;
        }

        dbContext.TruncateAllTablesAsync().GetAwaiter().GetResult();
        var projectDir = GetProjectRootPath(AppContext.BaseDirectory);
        var seedDataPath = Path.Combine(projectDir, "..", "Liberty.Reservation.Employee.WebAPI", "wwwroot", "SeedData");
        DataSeeder.SeedingAsync(seedDataPath, dbContext).GetAwaiter().GetResult();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
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
