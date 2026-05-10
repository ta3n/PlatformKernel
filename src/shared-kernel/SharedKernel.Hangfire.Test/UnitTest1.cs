using Hangfire;
using Hangfire.Common;
using Hangfire.MemoryStorage;
using Hangfire.Redis.StackExchange;
using Hangfire.States;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
using SharedKernel.Hangfire;
using SharedKernel.Hangfire.Abstractions;
using SharedKernel.Hangfire.Exceptions;
using SharedKernel.Hangfire.Infrastructure;
using SharedKernel.Hangfire.Options;
using SharedKernel.Hangfire.Models;
using SharedKernel.Hangfire.Utils;
using System.Reflection;

namespace SharedKernel.Hangfire.Test;

public class UnitTest1
{
    [Fact]
    public void GetDefaultRecurringJobOptions_UsesTokyoTimeZone()
    {
        var options = JobUtil.GetDefaultRecurringJobOptions();

        Assert.IsType<RecurringJobOptions>(options);
        Assert.Equal(JobUtil.DefaultTimeZone, options.TimeZone.Id);
    }

    [Fact]
    public void GetRecurringJobOptions_UsesProvidedTimeZone()
    {
        var options = JobUtil.GetRecurringJobOptions("UTC");

        Assert.Equal("UTC", options.TimeZone.Id);
    }

    [Fact]
    public void UseHangfireDashboardCustom_ReturnsSameBuilderWhenDisabled()
    {
        var services = new ServiceCollection().BuildServiceProvider();
        var app = new ApplicationBuilder(services);
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(
            [
                new KeyValuePair<string, string?>("HangfireDashboard:Enabled", "false"),
                new KeyValuePair<string, string?>("HangfireDashboard:DashboardUrl", "hangfire"),
                new KeyValuePair<string, string?>("HangfireDashboard:Username", "user"),
                new KeyValuePair<string, string?>("HangfireDashboard:Password", "pass"),
                new KeyValuePair<string, string?>("HangfireDashboard:IsReadOnly", "false")
            ])
            .Build();

        var result = app.UseHangfireDashboardCustom(configuration);

        Assert.Same(app, result);
    }

    [Fact]
    public void UseHangfireDashboardCustom_ThrowsWhenEnabledWithoutCredentials()
    {
        var services = new ServiceCollection().BuildServiceProvider();
        var app = new ApplicationBuilder(services);
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(
            [
                new KeyValuePair<string, string?>("HangfireDashboard:Enabled", "true"),
                new KeyValuePair<string, string?>("HangfireDashboard:DashboardUrl", "hangfire"),
                new KeyValuePair<string, string?>("HangfireDashboard:IsReadOnly", "false")
            ])
            .Build();

        var exception = Assert.Throws<InvalidOperationException>(
            () => app.UseHangfireDashboardCustom(configuration)
        );

        Assert.Contains(
            "HangfireDashboard:Username",
            exception.Message,
            StringComparison.Ordinal
        );
    }

    [Fact]
    public void JobRequestRecords_ExposeInheritedValues()
    {
        var recurring = new RegisterRecurringJobRequest("ImageResize", "nightly", "{}", "0 0 * * *");
        var delayed = new RegisterScheduleJobDelayRequest("ImageResize", "delay", "{}", TimeSpan.FromMinutes(5));

        Assert.Equal("ImageResize", recurring.EventName);
        Assert.Equal("nightly", recurring.JobName);
        Assert.Equal("0 0 * * *", recurring.CronExpression);
        Assert.Equal(TimeSpan.FromMinutes(5), delayed.Delay);
    }

    [Fact]
    public void SchedulerJobDefinitionValidator_AcceptsGeneralRegistrationModel()
    {
        var definition = CreateSchedulerJobDefinition();

        SchedulerJobDefinitionValidator.EnsureValidRecurring(definition);

        Assert.Equal("billing.close-daily", definition.JobKey);
        Assert.Equal("0 17 * * *", definition.CronExpression);
        Assert.Equal(SchedulerJobStatus.Active, definition.Status);
    }

    [Fact]
    public void SchedulerJobDefinitionValidator_ThrowsForInvalidDefinition()
    {
        var definition = CreateSchedulerJobDefinition() with
        {
            GrpcMethod = ""
        };

        var exception = Assert.Throws<InvalidSchedulerJobDefinitionException>(
            () => SchedulerJobDefinitionValidator.EnsureValidRecurring(definition)
        );

        Assert.Contains(
            nameof(SchedulerJobDefinition.GrpcMethod),
            exception.Message,
            StringComparison.Ordinal
        );
    }

    [Fact]
    public void SchedulerExecutionContext_CreatesStableIdempotencyKey()
    {
        var scheduledAt = new DateTimeOffset(
            2026,
            5,
            10,
            12,
            30,
            0,
            TimeSpan.FromHours(7)
        );

        var execution = SchedulerExecutionContext.Create(
            "billing.close-daily",
            scheduledAt,
            "execution-1",
            "trace-1"
        );

        Assert.Equal("execution-1", execution.ExecutionId);
        Assert.Equal("trace-1", execution.CorrelationId);
        Assert.Equal("billing.close-daily:2026-05-10T05:30:00.0000000+00:00", execution.IdempotencyKey);
    }

    [Fact]
    public void SchedulerGrpcDispatchRequest_CopiesTargetTenantAndExecutionMetadata()
    {
        var definition = CreateSchedulerJobDefinition();
        var execution = SchedulerExecutionContext.Create(
            definition.JobKey,
            DateTimeOffset.UtcNow,
            "execution-1"
        );

        var request = SchedulerGrpcDispatchRequest.Create(
            definition,
            execution
        );

        Assert.Equal(definition.TargetService, request.TargetService);
        Assert.Equal(definition.GrpcMethod, request.GrpcMethod);
        Assert.Equal(definition.SubSystemId, request.SubSystemId);
        Assert.Equal(execution.IdempotencyKey, request.Execution.IdempotencyKey);
    }

    [Fact]
    public void SchedulerGrpcDispatchRequest_ToGrpcMetadata_MapsTenantAndExecutionHeaders()
    {
        var definition = CreateSchedulerJobDefinition();
        var execution = SchedulerExecutionContext.Create(
            definition.JobKey,
            new DateTimeOffset(
                2026,
                5,
                10,
                12,
                30,
                0,
                TimeSpan.FromHours(7)
            ),
            "execution-1",
            "trace-1"
        );
        var request = SchedulerGrpcDispatchRequest.Create(
            definition,
            execution
        );

        var metadata = request.ToGrpcMetadata();

        Assert.Equal(definition.SubSystemId, metadata[SchedulerGrpcMetadataNames.SubSystemId]);
        Assert.Equal(definition.CompanyId, metadata[SchedulerGrpcMetadataNames.CompanyId]);
        Assert.Equal(definition.ProjectId, metadata[SchedulerGrpcMetadataNames.ProjectId]);
        Assert.Equal(execution.IdempotencyKey, metadata[SchedulerGrpcMetadataNames.IdempotencyKey]);
        Assert.Equal(execution.ExecutionId, metadata[SchedulerGrpcMetadataNames.ExecutionId]);
        Assert.Equal("trace-1", metadata[SchedulerGrpcMetadataNames.CorrelationId]);
    }

    [Fact]
    public void SchedulerRetryPolicy_ClassifiesRetryableAndNonRetryableFailures()
    {
        Assert.True(SchedulerRetryPolicy.ShouldRetry(new RetryableSchedulerJobException("timeout")));
        Assert.True(SchedulerRetryPolicy.ShouldRetry(new TimeoutException("timeout")));
        Assert.False(SchedulerRetryPolicy.ShouldRetry(new NonRetryableSchedulerJobException("invalid")));
        Assert.False(SchedulerRetryPolicy.ShouldRetry(new InvalidSchedulerJobDefinitionException("invalid")));
        Assert.False(SchedulerRetryPolicy.ShouldRetry(new UnauthorizedAccessException("denied")));
    }

    [Fact]
    public void AddHangfireCustom_RegistersSchedulerEngine()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(
            [
                new KeyValuePair<string, string?>("ConnectionStrings:HangfireConnection", "Host=localhost;Database=hangfire;Username=postgres;Password=postgres")
            ])
            .Build();

        services.AddHangfireCustom(configuration);

        Assert.Contains(
            services,
            descriptor => descriptor.ServiceType == typeof(IHangfireSchedulerEngine)
        );
    }

    [Fact]
    public void AddHangfireSchedulerOrchestration_RegistersSchedulerServiceAndExecutor()
    {
        var services = new ServiceCollection();

        services.AddHangfireSchedulerOrchestration();

        Assert.Contains(
            services,
            descriptor => descriptor.ServiceType == typeof(IJobExecutor)
                          && descriptor.ImplementationType == typeof(JobExecutor)
        );
        Assert.Contains(
            services,
            descriptor => descriptor.ServiceType == typeof(IJobScheduler)
                          && descriptor.ImplementationType == typeof(JobSchedulerService)
        );
    }

    [Fact]
    public async Task JobSchedulerService_RegisterRecurringJob_StoresMetadataAndRegistersEngine()
    {
        var store = new InMemoryJobMetadataStore();
        var engine = new RecordingSchedulerEngine();
        var service = new JobSchedulerService(
            store,
            engine,
            NullLogger<JobSchedulerService>.Instance
        );
        var definition = CreateSchedulerJobDefinition();

        var result = await service.RegisterRecurringJobAsync(definition);

        var storedDefinition = await store.GetByJobKeyAsync(definition.JobKey);
        Assert.True(result.Succeeded);
        Assert.Equal(definition.JobKey, result.JobKey);
        Assert.NotNull(storedDefinition);
        Assert.Equal(SchedulerJobStatus.Active, storedDefinition.Status);
        Assert.Equal(definition.JobKey, engine.RegisteredDefinition?.JobKey);
    }

    [Fact]
    public void AddHangfireCustom_UsesRedisStorageAndMapsRedisOptions()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(
            [
                new KeyValuePair<string, string?>("HangfireStorage:Provider", "Redis"),
                new KeyValuePair<string, string?>("HangfireStorage:SucceededJobExpirationInDays", "7"),
                new KeyValuePair<string, string?>("HangfireStorage:Redis:ConnectionString", "localhost:6379,abortConnect=false"),
                new KeyValuePair<string, string?>("HangfireStorage:Redis:Database", "3"),
                new KeyValuePair<string, string?>("HangfireStorage:Redis:Prefix", "kernel:hangfire:"),
                new KeyValuePair<string, string?>("HangfireStorage:Redis:SucceededListSize", "128"),
                new KeyValuePair<string, string?>("HangfireStorage:Redis:DeletedListSize", "64"),
                new KeyValuePair<string, string?>("HangfireStorage:Redis:ExpiryCheckIntervalInSeconds", "300"),
                new KeyValuePair<string, string?>("HangfireStorage:Redis:FetchTimeoutInSeconds", "15"),
                new KeyValuePair<string, string?>("HangfireStorage:Redis:InvisibilityTimeoutInSeconds", "30"),
                new KeyValuePair<string, string?>("HangfireStorage:Redis:UseTransactions", "false")
            ])
            .Build();

        services.AddHangfireCustom(configuration);

        using var provider = services.BuildServiceProvider();
        var storage = Assert.IsType<RedisStorage>(provider.GetRequiredService<JobStorage>());
        var storageOptions = Assert.IsType<RedisStorageOptions>(
            typeof(RedisStorage)
                .GetField("_options", BindingFlags.Instance | BindingFlags.NonPublic)!
                .GetValue(storage)
        );

        Assert.Equal(3, storage.Db);
        Assert.Equal("kernel:hangfire:", storageOptions.Prefix);
        Assert.Equal(128, storageOptions.SucceededListSize);
        Assert.Equal(64, storageOptions.DeletedListSize);
        Assert.Equal(TimeSpan.FromMinutes(5), storageOptions.ExpiryCheckInterval);
        Assert.Equal(TimeSpan.FromSeconds(15), storageOptions.FetchTimeout);
        Assert.Equal(TimeSpan.FromSeconds(30), storageOptions.InvisibilityTimeout);
        Assert.False(storageOptions.UseTransactions);
    }

    [Fact]
    public void AddHangfireCustom_UsesSharedRedisConfigurationAsFallback()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(
            [
                new KeyValuePair<string, string?>("HangfireStorage:Provider", "Redis"),
                new KeyValuePair<string, string?>("Redis:UrlConfiguration", "localhost:6379,abortConnect=false"),
                new KeyValuePair<string, string?>("Redis:DefaultDatabase", "5")
            ])
            .Build();

        services.AddHangfireCustom(configuration);

        using var provider = services.BuildServiceProvider();
        var storage = Assert.IsType<RedisStorage>(provider.GetRequiredService<JobStorage>());

        Assert.Equal(5, storage.Db);
    }

    [Fact]
    public void AddHangfireCustom_RegistersHangfireServerByDefault()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(
            [
                new KeyValuePair<string, string?>("ConnectionStrings:HangfireConnection", "Host=localhost;Database=hangfire;Username=postgres;Password=postgres")
            ])
            .Build();

        services.AddHangfireCustom(configuration);

        Assert.Contains(
            services,
            descriptor => descriptor.ServiceType == typeof(IHostedService)
        );
    }

    [Fact]
    public void AddHangfireCustom_DoesNotRegisterHangfireServerWhenDisabled()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(
            [
                new KeyValuePair<string, string?>("ConnectionStrings:HangfireConnection", "Host=localhost;Database=hangfire;Username=postgres;Password=postgres"),
                new KeyValuePair<string, string?>("HangfireServer:Enabled", "false")
            ])
            .Build();

        services.AddHangfireCustom(configuration);

        Assert.DoesNotContain(
            services,
            descriptor => descriptor.ServiceType == typeof(IHostedService)
        );
    }

    [Fact]
    public void AddHangfireCustom_ThrowsWhenSucceededJobExpirationIsInvalid()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(
            [
                new KeyValuePair<string, string?>("HangfireStorage:SucceededJobExpirationInDays", "0"),
                new KeyValuePair<string, string?>("ConnectionStrings:HangfireConnection", "Host=localhost;Database=hangfire;Username=postgres;Password=postgres")
            ])
            .Build();

        var exception = Assert.Throws<ArgumentOutOfRangeException>(
            () => services.AddHangfireCustom(configuration)
        );

        Assert.Equal("succeededJobExpirationInDays", exception.ParamName);
    }

    [Fact]
    public async Task UseHangfireBootstrapLockAsync_SerializesConcurrentBootstrap()
    {
        var services = new ServiceCollection();
        var syncRoot = new object();
        var currentConcurrency = 0;
        var maxConcurrency = 0;

        services.Configure<HangfireServerOptions>(
            options =>
            {
                options.BootstrapLockName = "hangfire:test-bootstrap";
                options.BootstrapLockTimeoutInSeconds = 5;
            }
        );
        services.AddSingleton<JobStorage>(new MemoryStorage());

        using var provider = services.BuildServiceProvider();

        async Task BootstrapAsync()
        {
            await provider.UseHangfireBootstrapLockAsync(
                async _ =>
                {
                    var current = Interlocked.Increment(ref currentConcurrency);

                    lock (syncRoot)
                    {
                        if (current > maxConcurrency)
                        {
                            maxConcurrency = current;
                        }
                    }

                    await Task.Delay(100);
                    Interlocked.Decrement(ref currentConcurrency);
                }
            );
        }

        await Task.WhenAll(BootstrapAsync(), BootstrapAsync());

        Assert.Equal(1, maxConcurrency);
    }

    [Fact]
    public void UseHangfireBootstrapLock_ThrowsWhenLockTimeoutIsInvalid()
    {
        var services = new ServiceCollection();

        services.Configure<HangfireServerOptions>(
            options => options.BootstrapLockTimeoutInSeconds = 0
        );
        services.AddSingleton<JobStorage>(new MemoryStorage());

        using var provider = services.BuildServiceProvider();

        var exception = Assert.Throws<ArgumentOutOfRangeException>(
            () => provider.UseHangfireBootstrapLock(_ => { })
        );

        Assert.Equal("bootstrapLockTimeoutInSeconds", exception.ParamName);
    }

    [Fact]
    public void SucceededJobExpirationFilterAttribute_SetsExpirationForSucceededJobs()
    {
        var filter = new SucceededJobExpirationFilterAttribute(TimeSpan.FromDays(7));
        var storage = new MemoryStorage();
        using var connection = storage.GetConnection();
        using var transaction = connection.CreateWriteTransaction();
        var context = new ApplyStateContext(
            storage,
            connection,
            transaction,
            new BackgroundJob(
                "job-1",
                Job.FromExpression(() => Console.WriteLine("hangfire")),
                DateTime.UtcNow
            ),
            new SucceededState(null, 0, 0),
            EnqueuedState.StateName
        );

        filter.OnStateApplied(context, transaction);

        Assert.Equal(TimeSpan.FromDays(7), context.JobExpirationTimeout);
    }

    [Fact]
    public void SucceededJobExpirationFilterAttribute_DoesNotChangeExpirationForFailedJobs()
    {
        var filter = new SucceededJobExpirationFilterAttribute(TimeSpan.FromDays(7));
        var storage = new MemoryStorage();
        using var connection = storage.GetConnection();
        using var transaction = connection.CreateWriteTransaction();
        var context = new ApplyStateContext(
            storage,
            connection,
            transaction,
            new BackgroundJob(
                "job-2",
                Job.FromExpression(() => Console.WriteLine("hangfire")),
                DateTime.UtcNow
            ),
            new FailedState(new InvalidOperationException("boom")),
            EnqueuedState.StateName
        );
        var originalExpirationTimeout = context.JobExpirationTimeout;

        filter.OnStateApplied(context, transaction);

        Assert.Equal(originalExpirationTimeout, context.JobExpirationTimeout);
    }

    private sealed class InMemoryJobMetadataStore : IJobMetadataStore
    {
        private readonly Dictionary<string, SchedulerJobDefinition> _definitions = [];

        public Task UpsertAsync(
            SchedulerJobDefinition definition,
            CancellationToken cancellationToken = default
        )
        {
            _definitions[definition.JobKey] = definition;

            return Task.CompletedTask;
        }

        public Task<SchedulerJobDefinition?> GetByJobKeyAsync(
            string jobKey,
            CancellationToken cancellationToken = default
        )
        {
            _definitions.TryGetValue(
                jobKey,
                out var definition
            );

            return Task.FromResult(definition);
        }

        public Task SetStatusAsync(
            string jobKey,
            SchedulerJobStatus status,
            CancellationToken cancellationToken = default
        )
        {
            if (_definitions.TryGetValue(
                    jobKey,
                    out var definition
                ))
            {
                _definitions[jobKey] = definition with
                {
                    Status = status
                };
            }

            return Task.CompletedTask;
        }

        public Task DeleteAsync(
            string jobKey,
            CancellationToken cancellationToken = default
        )
        {
            _definitions.Remove(jobKey);

            return Task.CompletedTask;
        }
    }

    private sealed class RecordingSchedulerEngine : IHangfireSchedulerEngine
    {
        public SchedulerJobDefinition? RegisteredDefinition { get; private set; }

        public void RegisterRecurringJob(
            SchedulerJobDefinition definition
        )
        {
            RegisteredDefinition = definition;
        }

        public void RemoveRecurringJob(
            string jobKey
        )
        {
        }

        public void PauseRecurringJob(
            string jobKey
        )
        {
        }

        public void ResumeRecurringJob(
            SchedulerJobDefinition definition
        )
        {
            RegisteredDefinition = definition;
        }

        public string TriggerJobNow(
            string jobKey,
            DateTimeOffset? scheduledAt = null,
            string? executionId = null,
            string? correlationId = null
        )
        {
            return "hangfire-job-1";
        }

        public string ScheduleJobAt(
            SchedulerJobDefinition definition,
            DateTimeOffset scheduledAt
        )
        {
            return "hangfire-job-2";
        }

        public string ScheduleJobDelay(
            SchedulerJobDefinition definition,
            TimeSpan delay
        )
        {
            return "hangfire-job-3";
        }
    }

    private static SchedulerJobDefinition CreateSchedulerJobDefinition()
    {
        return new SchedulerJobDefinition
        {
            JobKey = "billing.close-daily",
            TargetService = "billing-service",
            GrpcMethod = "billing.v1.InvoiceScheduler/CloseDailyInvoices",
            CronExpression = "0 17 * * *",
            PayloadJson = "{\"kind\":\"daily-close\"}",
            SubSystemId = "core",
            CompanyId = "company-1",
            ProjectId = "project-1",
            RequestedBy = "billing-service",
            TimeZoneId = "UTC"
        };
    }
}
