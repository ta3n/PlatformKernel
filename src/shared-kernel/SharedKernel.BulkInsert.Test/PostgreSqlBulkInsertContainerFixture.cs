using DotNet.Testcontainers.Configurations;
using Npgsql;
using Testcontainers.PostgreSql;

namespace SharedKernel.BulkInsert.Test;

public sealed class PostgreSqlBulkInsertContainerFixture : IAsyncLifetime
{
    private static readonly string TestPassword = Guid.NewGuid().ToString("N");

    static PostgreSqlBulkInsertContainerFixture()
    {
        TestcontainersSettings.ResourceReaperEnabled = false;
    }

    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder()
        .WithDatabase("postgres")
        .WithCleanUp(false)
        .WithUsername("postgres")
        .WithPassword(TestPassword)
        .Build();

    public Task InitializeAsync()
    {
        return _container.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _container.DisposeAsync().AsTask();
    }

    public async Task<string> CreateIsolatedConnectionStringAsync()
    {
        var databaseName = $"bulk_insert_{Guid.NewGuid():N}";

        await using var adminConnection = new NpgsqlConnection(_container.GetConnectionString());
        await adminConnection.OpenAsync();

        await using var command = new NpgsqlCommand($"CREATE DATABASE {databaseName};", adminConnection);
        await command.ExecuteNonQueryAsync();

        var builder = new NpgsqlConnectionStringBuilder(_container.GetConnectionString())
        {
            Database = databaseName
        };

        return builder.ConnectionString;
    }
}
