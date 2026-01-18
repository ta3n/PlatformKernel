using Liberty.Reservation.User.Application.Contexts;
using Liberty.Reservation.User.Application.Contexts.SeedData;
using Liberty.Reservation.User.WebAPI.Test.InfrastructureOfTest.Utilities;
using Microsoft.AspNetCore.Hosting;

namespace Liberty.Reservation.User.WebAPI.Test.InfrastructureOfTest;

[Collection("Sequential")]
public abstract class BaseIntegrationTest : IDisposable
{
    protected readonly AppWebApplicationFactory<TestStartup> Factory;
    protected readonly HttpClient Client;
    protected const string MockUserCode = "01JBHRFGSPRZ3HCV9RS8HVVBXR";

    protected BaseIntegrationTest()
    {
        Factory = new AppWebApplicationFactory<TestStartup>().WithMockIdentity(MockUserCode);
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
