using Microsoft.EntityFrameworkCore;
using Npgsql;
using SharedKernel.BulkInsertPipeline.Test.Service.Domain;

namespace SharedKernel.BulkInsertPipeline.Test.Service.Data;

public sealed class BulkInsertPipelineLabDatabase(
    IConfiguration configuration,
    IDbContextFactory<BulkInsertPipelineLabDbContext> dbContextFactory
)
{
    public const string ConnectionStringName = "BulkInsertPipelineLab";
    public const string Schema = "bulk_insert_pipeline_lab";
    public const string Table = "metric_events";

    private readonly string _connectionString = configuration.GetConnectionString(ConnectionStringName)
        ?? throw new InvalidOperationException(
            $"Connection string '{ConnectionStringName}' was not found."
        );
    private readonly string _databaseName = GetRequiredDatabaseName(configuration.GetConnectionString(ConnectionStringName));
    private readonly IDbContextFactory<BulkInsertPipelineLabDbContext> _dbContextFactory = dbContextFactory;

    public string DatabaseName => _databaseName;

    public async Task EnsureReadyAsync(
        CancellationToken cancellationToken = default
    )
    {
        await EnsureDatabaseExistsAsync(cancellationToken).ConfigureAwait(false);

        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

        await using (var schemaCommand = new NpgsqlCommand(
            $"CREATE SCHEMA IF NOT EXISTS {QuoteIdentifier(Schema)};",
            connection
        ))
        {
            await schemaCommand.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
        }

        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken)
            .ConfigureAwait(false);
        await dbContext.Database.EnsureCreatedAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<long> GetCountAsync(
        CancellationToken cancellationToken = default
    )
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken)
            .ConfigureAwait(false);

        return await dbContext.Metrics.LongCountAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<MetricReading>> GetRecentMetricsAsync(
        int take,
        CancellationToken cancellationToken = default
    )
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken)
            .ConfigureAwait(false);

        return await dbContext.Metrics
            .AsNoTracking()
            .OrderByDescending(metric => metric.Id)
            .Take(take)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task ClearAsync(
        CancellationToken cancellationToken = default
    )
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

        await using var truncateCommand = new NpgsqlCommand(
            $"TRUNCATE TABLE {QualifiedTableName} RESTART IDENTITY;",
            connection
        );
        await truncateCommand.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
    }

    private async Task EnsureDatabaseExistsAsync(
        CancellationToken cancellationToken
    )
    {
        var builder = new NpgsqlConnectionStringBuilder(_connectionString);
        var databaseName = builder.Database;

        if (string.IsNullOrWhiteSpace(databaseName))
        {
            throw new InvalidOperationException("A target database name is required in the connection string.");
        }

        builder.Database = "postgres";

        await using var adminConnection = new NpgsqlConnection(builder.ConnectionString);
        await adminConnection.OpenAsync(cancellationToken).ConfigureAwait(false);

        await using var existsCommand = new NpgsqlCommand(
            "SELECT 1 FROM pg_database WHERE datname = @databaseName;",
            adminConnection
        );
        existsCommand.Parameters.AddWithValue("databaseName", databaseName);

        var exists = await existsCommand.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false) is not null;
        if (exists)
        {
            return;
        }

        await using var createCommand = new NpgsqlCommand(
            $"CREATE DATABASE {QuoteIdentifier(databaseName)};",
            adminConnection
        );
        await createCommand.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
    }

    private static string QuoteIdentifier(
        string identifier
    ) => $"\"{identifier.Replace("\"", "\"\"", StringComparison.Ordinal)}\"";

    private static string QualifiedTableName => $"{QuoteIdentifier(Schema)}.{QuoteIdentifier(Table)}";

    private static string GetRequiredDatabaseName(
        string? connectionString
    )
    {
        var databaseName = new NpgsqlConnectionStringBuilder(connectionString).Database;
        if (string.IsNullOrWhiteSpace(databaseName))
        {
            throw new InvalidOperationException("A target database name is required in the connection string.");
        }

        return databaseName;
    }
}
