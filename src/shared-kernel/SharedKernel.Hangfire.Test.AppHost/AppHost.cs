using System.IO;

var builder = DistributedApplication.CreateBuilder(args);

var redis = builder.AddRedis("hangfire-redis");

var service = builder
    .AddProject(
        "hangfire-test-service",
        "../SharedKernel.Hangfire.Test.Service/SharedKernel.Hangfire.Test.Service.csproj"
    )
    .WithReference(redis, connectionName: "HangfireRedisConnection")
    .WithEnvironment("HangfireStorage__Provider", "Redis")
    .WithEnvironment("HangfireStorage__SucceededJobExpirationInDays", "7")
    .WithEnvironment("HangfireStorage__Redis__Database", "0")
    .WithEnvironment("HangfireStorage__Redis__Prefix", "aspire:hangfire:test:")
    .WithEnvironment("HangfireServer__Enabled", "false")
    .WithEnvironment("HangfireServer__BootstrapLockName", "aspire:hangfire:test:bootstrap")
    .WithEnvironment("HangfireServer__BootstrapLockTimeoutInSeconds", "30")
    .WithEnvironment("TestService__StateDatabase", "1")
    .WithEnvironment("TestService__StatePrefix", "aspire:hangfire:state:")
    .WithReplicas(2)
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WaitFor(redis);

var k6Path = Environment.GetEnvironmentVariable("K6_PATH") ?? "k6";
var scriptsDirectory = Path.Combine(AppContext.BaseDirectory, "k6");

builder
    .AddExecutable(
        "k6-scheduled-job-once",
        k6Path,
        AppContext.BaseDirectory,
        "run",
        Path.Combine(scriptsDirectory, "scheduled-job-once.js")
    )
    .WithEnvironment("K6_NO_USAGE_REPORT", "true")
    .WithEnvironment("TARGET_URL", service.GetEndpoint("http"))
    .WithExplicitStart()
    .WaitFor(service);

builder
    .AddExecutable(
        "k6-recurring-job-once",
        k6Path,
        AppContext.BaseDirectory,
        "run",
        Path.Combine(scriptsDirectory, "recurring-job-once.js")
    )
    .WithEnvironment("K6_NO_USAGE_REPORT", "true")
    .WithEnvironment("TARGET_URL", service.GetEndpoint("http"))
    .WithExplicitStart()
    .WaitFor(service);

builder
    .AddExecutable(
        "k6-bootstrap-lock",
        k6Path,
        AppContext.BaseDirectory,
        "run",
        Path.Combine(scriptsDirectory, "bootstrap-lock.js")
    )
    .WithEnvironment("K6_NO_USAGE_REPORT", "true")
    .WithEnvironment("TARGET_URL", service.GetEndpoint("http"))
    .WithExplicitStart()
    .WaitFor(service);

await builder.Build().RunAsync();
