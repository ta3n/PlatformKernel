using Liberty.Reservation.Employee.Application.Contexts;
using Liberty.Reservation.Employee.Application.Contexts.SeedData;
using Liberty.Reservation.Employee.WebAPI.Test.InfrastructureOfTest.Utilities;
using Microsoft.AspNetCore.Hosting;

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
