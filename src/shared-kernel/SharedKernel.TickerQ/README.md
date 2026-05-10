# SharedKernel.TickerQ

A SharedKernel module providing job scheduling capabilities using [TickerQ](https://tickerq.net/), a modern,
lightweight, and production-ready scheduling library for .NET applications.

## Version Compatibility

This module uses **TickerQ 2.1.0** which targets **.NET Standard 2.1** and is compatible with .NET 8.0+.

Package versions:

- TickerQ: 2.1.0
- TickerQ.Utilities: 2.1.0
- TickerQ.EntityFrameworkCore: 2.0.0

## Features

- **Time-based & Cron scheduling** - One-off and recurring jobs
- **Source-generated** - Compile-time function registration with zero reflection
- **Dual persistence** - Entity Framework Core (PostgreSQL, SQL Server, SQLite, MySQL) or Redis
- **Live dashboard** - Real-time monitoring UI with SignalR
- **Retry policies** - Configurable retry mechanisms with backoff
- **High-level abstractions** - Simplified job scheduling interfaces
- **Multi-storage support** - Easy switching between EF Core and Redis storage

## Installation

Add the project reference to your service:

```xml
<ItemGroup>
  <ProjectReference Include="path/to/SharedKernel.TickerQ/SharedKernel.TickerQ.csproj" />
</ItemGroup>
```

## Configuration

### appsettings.json

```json
{
  "ConnectionStrings": {
    "TickerQConnection": "Host=localhost;Port=5432;Database=myapp;Username=postgres;Password=postgres"
  },
  "TickerQStorage": {
    "Provider": "EntityFramework",
    "ConnectionStringName": "TickerQConnection",
    "CancelMissedTickersOnRestart": true,
    "UseModelCustomizerForMigrations": true,
    "Redis": {
      "ConnectionString": "localhost:6379",
      "Database": 1,
      "Prefix": "tickerq:"
    }
  },
  "TickerQServer": {
    "Enabled": true,
    "MaxConcurrency": 4,
    "DefaultRetries": 3,
    "DefaultRetryIntervalsInSeconds": [60, 120, 300],
    "BootstrapLockName": "tickerq:bootstrap",
    "BootstrapLockTimeoutInSeconds": 180
  },
  "TickerQDashboard": {
    "Enabled": true,
    "DashboardUrl": "tickerq-dashboard",
    "EnableBasicAuth": true,
    "Username": "admin",
    "Password": "admin",
    "IsReadOnly": false
  }
}
```

### Program.cs

```csharp
using SharedKernel.TickerQ;
using TickerQ; // Native TickerQ namespace

var builder = WebApplication.CreateBuilder(args);

// 1. Register SharedKernel TickerQ abstractions
builder.Services.AddTickerQCustom(builder.Configuration);
builder.Services.ValidateTickerQOptions();

// 2. Register native TickerQ services
// Note: You need to configure TickerQ directly based on your storage provider
builder.Services.AddTickerQ(/* native TickerQ configuration */);

var app = builder.Build();

// 3. Use native TickerQ middleware
// app.UseTickerQ();

app.Run();
```

**Important**: The `AddTickerQCustom()` method only registers the SharedKernel abstractions (`ITickerQSchedulerEngine`,
`IJobScheduler`) and configuration options. You must separately register the native TickerQ services using the
framework's own registration methods.

## Defining Jobs

Define job functions using the `[TickerFunction]` attribute:

```csharp
using TickerQ.Utilities.Base;

public class MyJobs
{
    [TickerFunction("SendWelcomeEmail")]
    public async Task SendWelcomeEmail(
        TickerFunctionContext context,
        CancellationToken cancellationToken)
    {
        var request = context.GetRequest<WelcomeEmailRequest>();

        // Send email logic
        await _emailService.SendWelcomeEmailAsync(
            request.UserId,
            request.Email,
            cancellationToken
        );
    }

    [TickerFunction("CleanupLogs", "0 2 * * *")] // Runs daily at 2 AM
    public async Task CleanupLogs(
        TickerFunctionContext context,
        CancellationToken cancellationToken)
    {
        await _logService.CleanupOldLogsAsync(cancellationToken);
    }
}

public record WelcomeEmailRequest(string UserId, string Email);
```

## Scheduling Jobs

### Using IJobScheduler (High-level API)

```csharp
public class UserService
{
    private readonly IJobScheduler _jobScheduler;

    public UserService(IJobScheduler jobScheduler)
    {
        _jobScheduler = jobScheduler;
    }

    public async Task RegisterUserAsync(User user, CancellationToken ct)
    {
        // Save user to database
        await _repository.SaveAsync(user, ct);

        // Schedule welcome email to be sent in 5 minutes
        await _jobScheduler.ScheduleTimeJobAsync(
            function: "SendWelcomeEmail",
            runAt: DateTime.UtcNow.AddMinutes(5),
            request: new WelcomeEmailRequest(user.Id, user.Email),
            retries: 3,
            cancellationToken: ct
        );
    }

    public async Task SetupRecurringReportsAsync(CancellationToken ct)
    {
        // Schedule daily report generation at 8 AM
        await _jobScheduler.ScheduleCronJobAsync(
            function: "GenerateDailyReport",
            cronExpression: "0 8 * * *",
            request: new { ReportType = "Daily" },
            cancellationToken: ct
        );
    }
}
```

### Using ITickerQSchedulerEngine (Low-level API)

```csharp
public class AdvancedSchedulerService
{
    private readonly ITickerQSchedulerEngine _engine;

    public AdvancedSchedulerService(ITickerQSchedulerEngine engine)
    {
        _engine = engine;
    }

    public async Task ScheduleComplexJobAsync(CancellationToken ct)
    {
        var definition = new SchedulerJobDefinition
        {
            JobId = Guid.NewGuid().ToString("N"),
            Function = "ProcessPayment",
            JobType = SchedulerJobType.Time,
            RunAt = DateTime.UtcNow.AddHours(1),
            Request = new { PaymentId = "PAY-123", Amount = 99.99m },
            Retries = 5,
            RetryIntervalsInSeconds = [30, 60, 120, 300, 600],
            Description = "Process payment for order #123",
            Tags = ["payment", "order", "critical"]
        };

        var result = await _engine.ScheduleAsync(definition, ct);

        if (!result.Success)
        {
            throw new InvalidOperationException($"Failed to schedule job: {result.ErrorMessage}");
        }
    }

    public async Task CancelJobAsync(string jobId, CancellationToken ct)
    {
        var result = await _engine.CancelAsync(jobId, ct);

        if (!result.Success)
        {
            _logger.LogError("Failed to cancel job {JobId}: {Error}", jobId, result.ErrorMessage);
        }
    }

    public async Task<SchedulerJobStatus?> GetJobStatusAsync(string jobId, CancellationToken ct)
    {
        return await _engine.GetStatusAsync(jobId, ct);
    }
}
```

## Storage Providers

### Entity Framework Core

Configure EF Core storage in your `DbContext`:

```csharp
using Microsoft.EntityFrameworkCore;
using TickerQ.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Apply TickerQ configurations
        builder.ApplyConfiguration(new TimeTickerConfigurations());
        builder.ApplyConfiguration(new CronTickerConfigurations());
        builder.ApplyConfiguration(new CronTickerOccurrenceConfigurations());
    }
}
```

Run migrations:

```bash
dotnet ef migrations add AddTickerQTables
dotnet ef database update
```

### Redis

Set the provider to `"Redis"` in configuration:

```json
{
  "TickerQStorage": {
    "Provider": "Redis",
    "Redis": {
      "ConnectionString": "localhost:6379",
      "Database": 1,
      "Prefix": "tickerq:"
    }
  }
}
```

## Dashboard

Access the dashboard at `https://localhost:5001/tickerq-dashboard` (configured in `DashboardUrl`).

Default credentials:

- Username: `admin`
- Password: `admin`

The dashboard provides:

- Real-time job monitoring
- Job execution history
- Manual job triggering
- Job status and error details
- Cron expression validation

## Configuration Options

### TickerQStorageOptions

| Property                        | Type                       | Default             | Description                                    |
|---------------------------------|----------------------------|---------------------|------------------------------------------------|
| Provider                        | string                     | "EntityFramework"   | Storage provider: "EntityFramework" or "Redis" |
| ConnectionStringName            | string                     | "TickerQConnection" | Connection string name for EF Core             |
| CancelMissedTickersOnRestart    | bool                       | true                | Cancel missed jobs on restart                  |
| UseModelCustomizerForMigrations | bool                       | true                | Use model customizer for migrations            |
| Redis                           | TickerQRedisStorageOptions | -                   | Redis storage options                          |

### TickerQServerOptions

| Property                       | Type   | Default             | Description                       |
|--------------------------------|--------|---------------------|-----------------------------------|
| Enabled                        | bool   | true                | Enable TickerQ server             |
| MaxConcurrency                 | int    | 4                   | Maximum concurrent job executions |
| DefaultRetries                 | int    | 3                   | Default retry attempts            |
| DefaultRetryIntervalsInSeconds | int[]  | [60, 120, 300]      | Default retry intervals           |
| PollingIntervalInSeconds       | int?   | null                | Job polling interval              |
| BootstrapLockName              | string | "tickerq:bootstrap" | Bootstrap lock name               |
| BootstrapLockTimeoutInSeconds  | int    | 180                 | Bootstrap lock timeout            |

### TickerQDashboardOptions

| Property        | Type    | Default             | Description                 |
|-----------------|---------|---------------------|-----------------------------|
| Enabled         | bool    | false               | Enable dashboard            |
| DashboardUrl    | string  | "tickerq-dashboard" | Dashboard base path         |
| EnableBasicAuth | bool    | true                | Enable basic authentication |
| Username        | string? | null                | Dashboard username          |
| Password        | string? | null                | Dashboard password          |
| IsReadOnly      | bool    | false               | Read-only mode              |

## Best Practices

1. **Concurrency**: Set `MaxConcurrency` based on your workload and server capacity
2. **Retries**: Configure meaningful `RetryIntervalsInSeconds` to avoid aggressive retrying
3. **Job Idempotency**: Ensure job functions are idempotent to handle retries safely
4. **Error Handling**: Implement proper error handling and logging in job functions
5. **Storage**: Use EF Core for transactional guarantees, Redis for distributed scenarios
6. **Missed Jobs**: Enable `CancelMissedTickersOnRestart` for failover protection
7. **Security**: Change default dashboard credentials in production

## Examples

See the test projects for complete examples:

- `SharedKernel.TickerQ.Test` - Unit tests
- `SharedKernel.TickerQ.Test.Service` - Integration tests with ASP.NET host
- `SharedKernel.TickerQ.Test.AppHost` - .NET Aspire orchestration example

## Related Modules

- **SharedKernel.Hangfire** - Alternative job scheduling with Hangfire
- **SharedKernel.MassTransit** - Message-based background processing
- **SharedKernel.Rebus** - Lightweight service bus for background jobs

## References

- [TickerQ Official Website](https://tickerq.net/)
- [TickerQ GitHub Repository](https://github.com/Arcenox-co/TickerQ)
- [TickerQ NuGet Package](https://www.nuget.org/packages/TickerQ/)
