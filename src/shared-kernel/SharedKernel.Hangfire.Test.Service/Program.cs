using Hangfire;
using Microsoft.Extensions.Options;
using SharedKernel.Hangfire;
using SharedKernel.Hangfire.Test.Service.Models;
using SharedKernel.Hangfire.Test.Service.Options;
using SharedKernel.Hangfire.Test.Service.Services;
using SharedKernel.ServiceDefaults;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.Configure<TestHangfireServiceOptions>(
    builder.Configuration.GetSection(TestHangfireServiceOptions.SectionName)
);
builder.Services.AddSingleton<IConnectionMultiplexer>(
    _ => RedisConnectionFactory.Create(builder.Configuration)
);
builder.Services.AddSingleton<ScenarioStateTracker>();
builder.Services.AddSingleton<TestHangfireJobs>();

builder.Services.AddHangfireCustom(builder.Configuration);

if (!builder.Configuration.GetValue("HangfireServer:Enabled", true))
{
    builder.Services.AddHangfireServer(
        options =>
        {
            options.ServerName = InstanceIdentity.Resolve(
                builder.Configuration[$"{TestHangfireServiceOptions.SectionName}:InstanceId"]
            );
            options.WorkerCount = 1;
            options.SchedulePollingInterval = TimeSpan.FromSeconds(1);
        }
    );
}

var app = builder.Build();

app.MapDefaultEndpoints();

app.MapGet(
    "/",
    (
        IOptions<TestHangfireServiceOptions> options
    ) => Results.Ok(
        new
        {
            instanceId = InstanceIdentity.Resolve(options.Value.InstanceId)
        }
    )
);

var tests = app.MapGroup("/tests/scenarios").WithTags("HangfireTest");

tests.MapGet(
    "/{scenarioId}",
    async (
        string scenarioId,
        ScenarioStateTracker tracker,
        CancellationToken cancellationToken
    ) => Results.Ok(
        await tracker.GetStateAsync(
            scenarioId,
            cancellationToken
        )
    )
);

tests.MapPost(
    "/{scenarioId}/jobs/scheduled",
    (
        string scenarioId,
        ScheduleJobRequest request,
        IBackgroundJobClient backgroundJobClient
    ) =>
    {
        if (request.DelayMilliseconds <= 0)
        {
            return Results.ValidationProblem(
                new Dictionary<string, string[]>
                {
                    [nameof(request.DelayMilliseconds)] = ["DelayMilliseconds must be greater than zero."]
                }
            );
        }

        var jobId = backgroundJobClient.Schedule<TestHangfireJobs>(
            job => job.ExecuteScheduledAsync(scenarioId),
            TimeSpan.FromMilliseconds(request.DelayMilliseconds)
        );

        return Results.Accepted(
            $"/tests/scenarios/{scenarioId}",
            new ScheduledJobResponse(jobId, request.DelayMilliseconds)
        );
    }
);

tests.MapPost(
    "/{scenarioId}/jobs/recurring/next-minute",
    (
        string scenarioId,
        IRecurringJobManager recurringJobManager
    ) =>
    {
        var registration = RecurringJobRegistrationResponse.Create(scenarioId);
        recurringJobManager.AddOrUpdate<TestHangfireJobs>(
            registration.JobId,
            job => job.ExecuteRecurringAsync(scenarioId),
            registration.CronExpression,
            new RecurringJobOptions
            {
                TimeZone = TimeZoneInfo.Utc
            }
        );

        return Results.Accepted(
            $"/tests/scenarios/{scenarioId}",
            registration
        );
    }
);

tests.MapPost(
    "/{scenarioId}/bootstrap/run",
    async (
        string scenarioId,
        BootstrapLockRequest request,
        IServiceProvider serviceProvider,
        ScenarioStateTracker tracker,
        CancellationToken cancellationToken
    ) =>
    {
        if (request.HoldMilliseconds <= 0)
        {
            return Results.ValidationProblem(
                new Dictionary<string, string[]>
                {
                    [nameof(request.HoldMilliseconds)] = ["HoldMilliseconds must be greater than zero."]
                }
            );
        }

        await serviceProvider.UseHangfireBootstrapLockAsync(
            async _ =>
            {
                await tracker.RecordBootstrapEntryAsync(
                    scenarioId,
                    TimeSpan.FromMilliseconds(request.HoldMilliseconds),
                    cancellationToken
                );
            },
            cancellationToken
        );

        return Results.Ok(
            await tracker.GetStateAsync(
                scenarioId,
                cancellationToken
            )
        );
    }
);

await app.RunAsync();

public partial class Program
{
    protected Program()
    {
    }
}
