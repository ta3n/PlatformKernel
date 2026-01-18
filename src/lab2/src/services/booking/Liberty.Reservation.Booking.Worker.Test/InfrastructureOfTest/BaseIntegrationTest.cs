using Liberty.Reservation.Booking.Worker.Test.Configration;
using Liberty.Reservation.Booking.Worker.Test.InfrastructureOfTest.Utilities;
using Liberty.Reservation.Manager.Application.Contexts;

namespace Liberty.Reservation.Booking.Worker.Test.InfrastructureOfTest;

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
        var dbContext = Factory.GetRequiredService<ManagerDataContext>();
        if (dbContext is null)
        {
            return;
        }

        dbContext.TruncateAllTablesAsync().GetAwaiter().GetResult();
        var projectDir = AppContext.BaseDirectory;
        var seedDataPath = Path.Combine(projectDir, "SeedDataJson");
        DataSeeder.SeedingAsync(seedDataPath, dbContext).GetAwaiter().GetResult();
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
