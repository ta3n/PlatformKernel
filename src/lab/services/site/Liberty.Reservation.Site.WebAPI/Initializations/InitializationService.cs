using Liberty.Reservation.Application.Contexts.DataContexts;

namespace Liberty.Reservation.Employee.WebAPI.Initializations;

public class InitializationService(
    ILogger<InitializationService> logger,
    IServiceProvider serviceProvider
)
    : IHostedService
{
    public async Task StartAsync(
        CancellationToken cancellationToken
    )
    {
        logger.LogInformation("Initializing...");

        // 初期データのロジックをここに記述する
        using (var scope = serviceProvider.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();

            /*

                // DB を作り直し
                dbContext.Database.EnsureDeleted();
                dbContext.Database.EnsureCreated();

                await dbContext.SaveChangesAsync();
            */
        }

        logger.LogInformation("Initialized.");
    }

    public Task StopAsync(
        CancellationToken cancellationToken
    )
    {
        return Task.CompletedTask;
    }
}
