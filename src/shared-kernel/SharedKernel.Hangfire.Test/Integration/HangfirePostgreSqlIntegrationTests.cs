using Hangfire;
using Hangfire.States;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Hangfire.Test.Integration;

namespace SharedKernel.Hangfire.Test;

[Collection(HangfirePostgreSqlCollection.Name)]
public sealed class HangfirePostgreSqlIntegrationTests
{
    private readonly HangfirePostgreSqlContainerFixture _fixture;

    public HangfirePostgreSqlIntegrationTests(
        HangfirePostgreSqlContainerFixture fixture
    )
    {
        _fixture = fixture;
    }

    [Fact]
    public void AddHangfireCustom_UsesPostgreSqlContainerStorage()
    {
        using var provider = CreateProvider();
        var jobClient = provider.GetRequiredService<IBackgroundJobClient>();
        var jobStorage = provider.GetRequiredService<JobStorage>();
        var testId = Guid.NewGuid().ToString("N");

        var jobId = jobClient.Enqueue(
            () => PostgreSqlProbeJob.Execute(testId)
        );

        using var connection = jobStorage.GetConnection();
        var jobData = connection.GetJobData(jobId);

        Assert.Equal("PostgreSqlStorage", jobStorage.GetType().Name);
        Assert.NotNull(jobData);
        Assert.Equal(EnqueuedState.StateName, jobData.State);
        Assert.Equal(typeof(PostgreSqlProbeJob), jobData.Job.Type);
        Assert.Equal(nameof(PostgreSqlProbeJob.Execute), jobData.Job.Method.Name);
        Assert.Equal(testId, Assert.Single(jobData.Job.Args));
    }

    [Fact]
    public async Task UseHangfireBootstrapLockAsync_SerializesConcurrentBootstrapWithPostgreSqlStorage()
    {
        var lockName = $"hangfire:postgresql:{Guid.NewGuid():N}:bootstrap";
        using var provider = CreateProvider(lockName);
        var currentConcurrency = 0;
        var maxConcurrency = 0;

        async Task RunBootstrapAsync()
        {
            await provider.UseHangfireBootstrapLockAsync(
                async _ =>
                {
                    var current = Interlocked.Increment(ref currentConcurrency);
                    try
                    {
                        UpdateMaxConcurrency(
                            ref maxConcurrency,
                            current
                        );
                        await Task.Delay(200);
                    }
                    finally
                    {
                        Interlocked.Decrement(ref currentConcurrency);
                    }
                }
            );
        }

        await Task.WhenAll(
            RunBootstrapAsync(),
            RunBootstrapAsync(),
            RunBootstrapAsync()
        );

        Assert.Equal(1, maxConcurrency);
    }

    private ServiceProvider CreateProvider(
        string? bootstrapLockName = null
    )
    {
        var services = new ServiceCollection();
        services.AddLogging();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(
            [
                new KeyValuePair<string, string?>("ConnectionStrings:HangfireConnection", _fixture.ConnectionString),
                new KeyValuePair<string, string?>("HangfireStorage:Provider", "PostgreSql"),
                new KeyValuePair<string, string?>("HangfireStorage:SucceededJobExpirationInDays", "7"),
                new KeyValuePair<string, string?>("HangfireServer:Enabled", "false"),
                new KeyValuePair<string, string?>("HangfireServer:BootstrapLockName", bootstrapLockName ?? $"hangfire:postgresql:{Guid.NewGuid():N}:bootstrap"),
                new KeyValuePair<string, string?>("HangfireServer:BootstrapLockTimeoutInSeconds", "10")
            ])
            .Build();

        services.AddHangfireCustom(configuration);

        return services.BuildServiceProvider();
    }

    private static void UpdateMaxConcurrency(
        ref int maxConcurrency,
        int current
    )
    {
        while (true)
        {
            var snapshot = Volatile.Read(ref maxConcurrency);
            if (current <= snapshot)
            {
                return;
            }

            if (Interlocked.CompareExchange(
                    ref maxConcurrency,
                    current,
                    snapshot
                ) == snapshot)
            {
                return;
            }
        }
    }

    public static class PostgreSqlProbeJob
    {
        public static void Execute(
            string testId
        )
        {
            GC.KeepAlive(testId);
        }
    }
}
