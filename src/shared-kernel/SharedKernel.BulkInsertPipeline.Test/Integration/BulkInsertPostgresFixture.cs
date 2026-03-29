using Npgsql;
using Testcontainers.PostgreSql;

namespace SharedKernel.BulkInsertPipeline.Test.Integration;

public sealed class BulkInsertPostgresFixture : IAsyncLifetime
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

    public async Task<string> CreateInitializedDatabaseAsync()
    {
        var databaseName = $"bulk_insert_{Guid.NewGuid():N}";

        await using var adminConnection = new NpgsqlConnection(_container.GetConnectionString());
        await adminConnection.OpenAsync();

        await using (var createDb = new NpgsqlCommand($"CREATE DATABASE {databaseName};", adminConnection))
        {
            await createDb.ExecuteNonQueryAsync();
        }

        var builder = new NpgsqlConnectionStringBuilder(_container.GetConnectionString())
        {
            Database = databaseName
        };

        var connectionString = builder.ConnectionString;

        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(
            """
            CREATE EXTENSION IF NOT EXISTS timescaledb;

            CREATE TABLE metric_buffer
            (
                device_id text NOT NULL,
                occurred_at timestamptz NOT NULL,
                value double precision NOT NULL,
                quality integer NULL
            );

            SELECT create_hypertable('metric_buffer', 'occurred_at', if_not_exists => TRUE);

            CREATE TABLE "MetricFallback"
            (
                "DeviceId" text NOT NULL,
                "OccurredAt" timestamptz NOT NULL,
                "Value" double precision NOT NULL,
                "Quality" integer NULL
            );
            """,
            connection
        );

        await command.ExecuteNonQueryAsync();

        return connectionString;
    }

    public static async Task<int> CountRowsAsync(string connectionString, string tableName)
    {
        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand($"SELECT COUNT(*) FROM \"{tableName}\";", connection);
        return Convert.ToInt32(await command.ExecuteScalarAsync());
    }
}
