using System.Text.Json;
using Npgsql;

namespace PlatformKernel.TimescaleDbDemo;

internal static class Program
{
    private static async Task<int> Main(
        string[] args
    )
    {
        var command = args.FirstOrDefault()?.ToLowerInvariant() ?? "demo";
        var connectionString = ResolveConnectionString(args);
        var runner = new TimescaleDemoRunner(connectionString);

        switch (command)
        {
            case "bootstrap":
                await runner.BootstrapAsync();
                await WriteJsonAsync(new { Command = command, Status = "ok", ConnectionString = connectionString });
                return 0;

            case "seed":
                await runner.BootstrapAsync();
                var seeded = await runner.SeedSampleDataAsync();
                await runner.RefreshContinuousAggregateAsync();
                await WriteJsonAsync(new { Command = command, SeededRows = seeded.Count });
                return 0;

            case "legacy":
                var legacy = await runner.RunLegacyMigrationDemoAsync();
                await WriteJsonAsync(legacy);
                return 0;

            case "summary":
                await WriteJsonAsync(await runner.GetSummaryAsync());
                return 0;

            case "tiering":
                await WriteJsonAsync(await runner.ProbeTieringAsync());
                return 0;

            case "demo":
                await runner.BootstrapAsync();
                await runner.SeedSampleDataAsync();
                await runner.RefreshContinuousAggregateAsync();
                var demoResult = new DemoExecutionResult(
                    await runner.GetSummaryAsync(),
                    await runner.RunLegacyMigrationDemoAsync(),
                    await runner.ProbeTieringAsync());
                await WriteJsonAsync(demoResult);
                return 0;

            default:
                await Console.Error.WriteLineAsync($"Unknown command '{command}'. Supported commands: bootstrap, seed, legacy, summary, tiering, demo.");
                return 1;
        }
    }

    private static string ResolveConnectionString(
        IReadOnlyList<string> args
    )
    {
        foreach (var arg in args)
        {
            const string prefix = "--connection=";
            if (arg.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            {
                return arg[prefix.Length..];
            }
        }

        return Environment.GetEnvironmentVariable("TIMESCALE_CONNECTION_STRING")
            ?? BuildDefaultConnectionString();
    }

    private static async Task WriteJsonAsync<T>(
        T payload
    )
    {
        await Console.Out.WriteLineAsync(JsonSerializer.Serialize(payload, new JsonSerializerOptions
        {
            WriteIndented = true
        }));
    }

    private static string BuildDefaultConnectionString()
    {
        var password = Environment.GetEnvironmentVariable("TIMESCALE_DB_PASSWORD")
            ?? throw new InvalidOperationException(
                "Set TIMESCALE_CONNECTION_STRING or TIMESCALE_DB_PASSWORD before running the sample.");

        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = "localhost",
            Port = 55432,
            Database = "timescale_demo",
            Username = "postgres",
            Password = password
        };

        return builder.ConnectionString;
    }
}
