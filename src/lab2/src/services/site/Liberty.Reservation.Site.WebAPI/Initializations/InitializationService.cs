using Liberty.Reservation.Site.Application.Contexts;
using Liberty.Reservation.Site.Application.Contexts.SeedData;

namespace Liberty.Reservation.Site.WebAPI.Initializations;

public class InitializationService(
    ILogger<InitializationService> logger,
    IServiceProvider serviceProvider,
    IWebHostEnvironment env
)
    : IHostedService
{
    public async Task StartAsync(
        CancellationToken cancellationToken
    )
    {
        logger.LogInformation("Initializing...");

        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SiteDataContext>();

        await DataSeeder.SeedingAsync(env.WebRootPath, dbContext);

        logger.LogInformation("Initialized");
    }

    public Task StopAsync(
        CancellationToken cancellationToken
    )
    {
        return Task.CompletedTask;
    }
}
