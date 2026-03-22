using Microsoft.EntityFrameworkCore;
using SharedKernel.MassTransit.Test.Data;

namespace SharedKernel.MassTransit.Test.Infrastructure;

public static class DatabaseInitializer
{
    private const long SchemaInitializationLockKey = 20260322_01;

    public static async Task InitializeDatabaseAsync(
        this WebApplication app
    )
    {
        const int maxAttempts = 10;

        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                await using var scope = app.Services.CreateAsyncScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<MassTransitTestDbContext>();
                var logger = scope.ServiceProvider
                    .GetRequiredService<ILoggerFactory>()
                    .CreateLogger("DatabaseInitializer");

                await dbContext.Database.OpenConnectionAsync();

                try
                {
                    await dbContext.Database.ExecuteSqlRawAsync(
                        $"SELECT pg_advisory_lock({SchemaInitializationLockKey});"
                    );

                    await dbContext.Database.EnsureCreatedAsync();
                }
                finally
                {
                    await dbContext.Database.ExecuteSqlRawAsync(
                        $"SELECT pg_advisory_unlock({SchemaInitializationLockKey});"
                    );
                    await dbContext.Database.CloseConnectionAsync();
                }

                logger.LogInformation("Database is ready for SharedKernel.MassTransit.Test.");
                return;
            }
            catch (Exception exception) when (attempt < maxAttempts)
            {
                await using var scope = app.Services.CreateAsyncScope();
                var logger = scope.ServiceProvider
                    .GetRequiredService<ILoggerFactory>()
                    .CreateLogger("DatabaseInitializer");

                logger.LogWarning(
                    exception,
                    "Database initialization attempt {Attempt}/{MaxAttempts} failed. Retrying.",
                    attempt,
                    maxAttempts
                );

                await Task.Delay(TimeSpan.FromSeconds(2));
            }
        }

        await using (var finalScope = app.Services.CreateAsyncScope())
        {
            var dbContext = finalScope.ServiceProvider.GetRequiredService<MassTransitTestDbContext>();
            await dbContext.Database.OpenConnectionAsync();

            try
            {
                await dbContext.Database.ExecuteSqlRawAsync(
                    $"SELECT pg_advisory_lock({SchemaInitializationLockKey});"
                );

                await dbContext.Database.EnsureCreatedAsync();
            }
            finally
            {
                await dbContext.Database.ExecuteSqlRawAsync(
                    $"SELECT pg_advisory_unlock({SchemaInitializationLockKey});"
                );
                await dbContext.Database.CloseConnectionAsync();
            }
        }
    }
}
