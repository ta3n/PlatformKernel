namespace PlatformKernel.Service.WebApi.Initializations;

public class InitializationService(
    ILogger<InitializationService> logger
)
    : IHostedService
{
    public Task StartAsync(
        CancellationToken cancellationToken
    )
    {
        logger.LogInformation("Initializing...");

        logger.LogInformation("Initialized");
        return Task.CompletedTask;
    }

    public Task StopAsync(
        CancellationToken cancellationToken
    )
    {
        return Task.CompletedTask;
    }
}
