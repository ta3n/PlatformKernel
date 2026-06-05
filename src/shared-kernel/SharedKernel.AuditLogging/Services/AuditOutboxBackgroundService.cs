using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharedKernel.AuditLogging.Models;
using SharedKernel.AuditLogging.Options;

namespace SharedKernel.AuditLogging.Services;

public sealed class AuditOutboxBackgroundService<TDbContext>(
    IServiceScopeFactory scopeFactory,
    IOptions<AuditLoggingOptions> options,
    ILogger<AuditOutboxBackgroundService<TDbContext>> logger
) : BackgroundService where TDbContext : DbContext
{
    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken
    )
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                return;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Audit outbox processing failed.");
            }

            await Task.Delay(options.Value.OutboxPollingInterval, stoppingToken);
        }
    }

    private async Task ProcessAsync(
        CancellationToken stoppingToken
    )
    {
        if (!options.Value.Enabled || options.Value.Mode != AuditLoggingMode.Outbox)
        {
            return;
        }

        using var scope = scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TDbContext>();
        var processor = scope.ServiceProvider.GetRequiredService<AuditOutboxProcessor>();

        await processor.ProcessBatchAsync(dbContext, stoppingToken);
    }
}
