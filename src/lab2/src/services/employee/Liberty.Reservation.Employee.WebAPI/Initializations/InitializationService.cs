using Liberty.Reservation.Employee.Application.Contexts;
using Microsoft.EntityFrameworkCore;

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

        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<EmployeeDataContext>();

        await dbContext.Database.OpenConnectionAsync(
            cancellationToken
        );
        logger.LogInformation("Initialized");
    }

    public Task StopAsync(
        CancellationToken cancellationToken
    )
    {
        return Task.CompletedTask;
    }
}
