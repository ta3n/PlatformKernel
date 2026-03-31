using Npgsql;
using Testcontainers.PostgreSql;

namespace PlatformKernel.TimescaleDbDemo.Tests;

public sealed class TimescaleContainerFixture : IAsyncLifetime
{
    private static readonly string TestPassword = Guid.NewGuid().ToString("N");

    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder()
        .WithImage("timescale/timescaledb-ha:pg17")
        .WithDatabase("postgres")
        .WithUsername("postgres")
        .WithPassword(TestPassword)
        .Build();

    public async Task InitializeAsync()
    {
        await _container.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await _container.DisposeAsync();
    }

    public async Task<string> CreateIsolatedConnectionStringAsync()
    {
        var databaseName = $"timescale_demo_{Guid.NewGuid():N}";

        await using var adminConnection = new NpgsqlConnection(_container.GetConnectionString());
        await adminConnection.OpenAsync();

        await using var command = new NpgsqlCommand($"CREATE DATABASE {databaseName};", adminConnection);
        await command.ExecuteNonQueryAsync();

        var builder = new NpgsqlConnectionStringBuilder(_container.GetConnectionString()) { Database = databaseName };

        return builder.ConnectionString;
    }
}
