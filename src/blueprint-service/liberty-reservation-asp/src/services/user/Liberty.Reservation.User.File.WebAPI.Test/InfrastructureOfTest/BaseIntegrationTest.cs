using Liberty.Reservation.User.Application.Contexts;
using Liberty.Reservation.User.Application.Contexts.SeedData;
using Liberty.Reservation.User.File.WebAPI.Test.InfrastructureOfTest.Utilities;
using Microsoft.AspNetCore.Hosting;

namespace Liberty.Reservation.User.File.WebAPI.Test.InfrastructureOfTest;

[Collection("Sequential")]
public class BaseIntegrationTest : IDisposable
{
    protected readonly AppWebApplicationFactory<TestStartup> Factory;
    protected readonly HttpClient Client;
    protected const int MockUserId = 1;
    private const string MockFacilityId = "1";

    protected BaseIntegrationTest()
    {
        Factory = new AppWebApplicationFactory<TestStartup>().WithMockIdentity(MockFacilityId);
        Client = Factory.CreateClient();

        ResetDatabase();
    }

    private void ResetDatabase()
    {
        var dbContext = Factory.GetRequiredService<UserDataContext>();
        if (dbContext is null)
        {
            return;
        }

        var env = Factory.GetRequiredService<IWebHostEnvironment>()!;

        dbContext.TruncateAllTablesAsync().GetAwaiter().GetResult();
        DataSeeder.SeedingAsync(env.WebRootPath, dbContext).GetAwaiter().GetResult();
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
