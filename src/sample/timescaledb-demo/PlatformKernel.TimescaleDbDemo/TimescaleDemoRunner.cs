using Dapper;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace PlatformKernel.TimescaleDbDemo;

public sealed class TimescaleDemoRunner
{
    private readonly string _connectionString;

    public TimescaleDemoRunner(
        string connectionString
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);
        _connectionString = connectionString;
    }

    public async Task BootstrapAsync(
        CancellationToken cancellationToken = default
    )
    {
        await using var connection = await OpenConnectionAsync(cancellationToken);

        await connection.ExecuteAsync(
            new CommandDefinition(
                """
                CREATE EXTENSION IF NOT EXISTS timescaledb;

                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1
                        FROM pg_available_extensions
                        WHERE name = 'timescaledb_toolkit'
                    ) THEN
                        CREATE EXTENSION IF NOT EXISTS timescaledb_toolkit;
                    END IF;
                END $$;

                CREATE TABLE IF NOT EXISTS metrics (
                    time timestamptz NOT NULL,
                    device_id text NOT NULL,
                    cpu double precision NULL,
                    memory double precision NULL,
                    temperature double precision NULL,
                    tags jsonb NULL,
                    CONSTRAINT pk_metrics PRIMARY KEY (time, device_id)
                ) WITH (
                    tsdb.hypertable,
                    tsdb.chunk_interval = '1 day',
                    tsdb.segmentby = 'device_id',
                    tsdb.orderby = 'time DESC'
                );

                CREATE INDEX IF NOT EXISTS ix_metrics_device_time_desc
                ON metrics (device_id, time DESC);

                CREATE MATERIALIZED VIEW IF NOT EXISTS metrics_1h
                WITH (timescaledb.continuous) AS
                SELECT
                    device_id,
                    time_bucket(INTERVAL '1 hour', time) AS bucket,
                    AVG(cpu) AS avg_cpu,
                    MAX(cpu) AS max_cpu,
                    MIN(cpu) AS min_cpu,
                    COUNT(*) AS total_rows
                FROM metrics
                GROUP BY device_id, bucket
                WITH NO DATA;
                """,
                cancellationToken: cancellationToken
            )
        );

        await connection.ExecuteAsync(
            new CommandDefinition(
                """
                ALTER MATERIALIZED VIEW metrics_1h
                SET (
                    timescaledb.enable_columnstore = true,
                    timescaledb.segmentby = 'device_id'
                );
                """,
                cancellationToken: cancellationToken
            )
        );

        await connection.ExecuteAsync(
            new CommandDefinition(
                """
                DO $$
                BEGIN
                    PERFORM add_continuous_aggregate_policy(
                        'metrics_1h',
                        start_offset => INTERVAL '30 days',
                        end_offset => INTERVAL '1 hour',
                        schedule_interval => INTERVAL '15 minutes'
                    );
                EXCEPTION
                    WHEN duplicate_object THEN
                        NULL;
                END $$;

                DO $$
                BEGIN
                    PERFORM add_retention_policy(
                        'metrics',
                        drop_after => INTERVAL '90 days'
                    );
                EXCEPTION
                    WHEN duplicate_object THEN
                        NULL;
                END $$;
                """,
                cancellationToken: cancellationToken
            )
        );

        await connection.ExecuteAsync(
            new CommandDefinition(
                """
                DO $$
                BEGIN
                    CALL add_columnstore_policy(
                        'metrics_1h',
                        after => INTERVAL '45 days'
                    );
                EXCEPTION
                    WHEN duplicate_object THEN
                        NULL;
                END $$;
                """,
                cancellationToken: cancellationToken
            )
        );
    }

    public async Task<IReadOnlyList<MetricPoint>> SeedSampleDataAsync(
        CancellationToken cancellationToken = default
    )
    {
        await using var context = CreateDbContext();

        await context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE metrics CASCADE;", cancellationToken);

        var sampleMetrics = SeedDataFactory.CreateSampleMetrics();
        context.Metrics.AddRange(sampleMetrics);
        await context.SaveChangesAsync(cancellationToken);

        return sampleMetrics;
    }

    public async Task RefreshContinuousAggregateAsync(
        CancellationToken cancellationToken = default
    )
    {
        await using var connection = await OpenConnectionAsync(cancellationToken);

        await connection.ExecuteAsync(
            new CommandDefinition(
                """
                CALL refresh_continuous_aggregate(
                    'metrics_1h',
                    NULL,
                    NULL
                );
                """,
                cancellationToken: cancellationToken
            )
        );
    }

    public async Task<TimescaleSummary> GetSummaryAsync(
        CancellationToken cancellationToken = default
    )
    {
        await using var connection = await OpenConnectionAsync(cancellationToken);

        var timescaleVersion = await connection.ExecuteScalarAsync<string>(
                new CommandDefinition(
                    """
                    SELECT extversion
                    FROM pg_extension
                    WHERE extname = 'timescaledb';
                    """,
                    cancellationToken: cancellationToken
                )
            )
            ?? "unknown";

        var toolkitInstalled = await connection.ExecuteScalarAsync<bool>(
            new CommandDefinition(
                """
                SELECT EXISTS (
                    SELECT 1
                    FROM pg_extension
                    WHERE extname = 'timescaledb_toolkit'
                );
                """,
                cancellationToken: cancellationToken
            )
        );

        var modernHypertableExists = await connection.ExecuteScalarAsync<bool>(
            new CommandDefinition(
                """
                SELECT EXISTS (
                    SELECT 1
                    FROM timescaledb_information.hypertables
                    WHERE hypertable_name = 'metrics'
                );
                """,
                cancellationToken: cancellationToken
            )
        );

        var seededRowCount = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(
                "SELECT COUNT(*) FROM metrics;",
                cancellationToken: cancellationToken
            )
        );

        var chunkCount = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(
                """
                SELECT COUNT(*)
                FROM timescaledb_information.chunks
                WHERE hypertable_name = 'metrics';
                """,
                cancellationToken: cancellationToken
            )
        );

        var hasContinuousAggregatePolicy = await connection.ExecuteScalarAsync<bool>(
            new CommandDefinition(
                """
                SELECT EXISTS (
                    SELECT 1
                    FROM timescaledb_information.jobs
                    WHERE proc_name = 'policy_refresh_continuous_aggregate'
                );
                """,
                cancellationToken: cancellationToken
            )
        );

        var hasRetentionPolicy = await connection.ExecuteScalarAsync<bool>(
            new CommandDefinition(
                """
                SELECT EXISTS (
                    SELECT 1
                    FROM timescaledb_information.jobs
                    WHERE proc_name = 'policy_retention'
                );
                """,
                cancellationToken: cancellationToken
            )
        );

        var hasColumnstorePolicy = await connection.ExecuteScalarAsync<bool>(
            new CommandDefinition(
                """
                SELECT EXISTS (
                    SELECT 1
                    FROM timescaledb_information.jobs
                    WHERE proc_name IN ('policy_compression', 'policy_columnstore')
                );
                """,
                cancellationToken: cancellationToken
            )
        );

        var hypertables = (await connection.QueryAsync<string>(
            new CommandDefinition(
                """
                SELECT hypertable_name
                FROM timescaledb_information.hypertables
                ORDER BY hypertable_name;
                """,
                cancellationToken: cancellationToken
            )
        )).ToArray();

        var bucketedMetrics = (await connection.QueryAsync<BucketedMetric>(
            new CommandDefinition(
                """
                SELECT
                    time_bucket(INTERVAL '5 minutes', time) AS Bucket,
                    device_id AS DeviceId,
                    AVG(cpu) AS AvgCpu,
                    MAX(temperature) AS MaxTemperature,
                    COUNT(*) AS RowCount
                FROM metrics
                GROUP BY Bucket, DeviceId
                ORDER BY Bucket, DeviceId;
                """,
                cancellationToken: cancellationToken
            )
        )).ToArray();

        var hourlyAggregates = (await connection.QueryAsync<HourlyMetricAggregate>(
            new CommandDefinition(
                """
                SELECT
                    bucket AS Bucket,
                    device_id AS DeviceId,
                    avg_cpu AS AvgCpu,
                    max_cpu AS MaxCpu,
                    min_cpu AS MinCpu,
                    total_rows AS TotalRows
                FROM metrics_1h
                ORDER BY Bucket, DeviceId;
                """,
                cancellationToken: cancellationToken
            )
        )).ToArray();

        var jobs = (await connection.QueryAsync<TimescaleJobInfo>(
            new CommandDefinition(
                """
                SELECT
                    job_id AS JobId,
                    proc_name AS ProcName
                FROM timescaledb_information.jobs
                ORDER BY job_id;
                """,
                cancellationToken: cancellationToken
            )
        )).ToArray();

        return new TimescaleSummary(
            timescaleVersion,
            toolkitInstalled,
            modernHypertableExists,
            seededRowCount,
            chunkCount,
            hasContinuousAggregatePolicy,
            hasRetentionPolicy,
            hasColumnstorePolicy,
            hypertables,
            bucketedMetrics,
            hourlyAggregates,
            jobs
        );
    }

    public async Task<LegacyMigrationResult> RunLegacyMigrationDemoAsync(
        CancellationToken cancellationToken = default
    )
    {
        await using var connection = await OpenConnectionAsync(cancellationToken);

        await connection.ExecuteAsync(
            new CommandDefinition(
                """
                DROP TABLE IF EXISTS metrics_legacy CASCADE;

                CREATE TABLE metrics_legacy (
                    time timestamptz NOT NULL,
                    device_id text NOT NULL,
                    cpu double precision NULL,
                    CONSTRAINT pk_metrics_legacy PRIMARY KEY (time, device_id)
                );

                INSERT INTO metrics_legacy (time, device_id, cpu)
                VALUES
                    ('2026-03-01T00:00:00Z', 'device-legacy-01', 0.11),
                    ('2026-03-01T00:10:00Z', 'device-legacy-01', 0.12),
                    ('2026-03-01T01:00:00Z', 'device-legacy-02', 0.33);

                SELECT create_hypertable(
                    'metrics_legacy',
                    by_range('time', INTERVAL '1 day'),
                    migrate_data => TRUE
                );
                """,
                cancellationToken: cancellationToken
            )
        );

        var isHypertable = await connection.ExecuteScalarAsync<bool>(
            new CommandDefinition(
                """
                SELECT EXISTS (
                    SELECT 1
                    FROM timescaledb_information.hypertables
                    WHERE hypertable_name = 'metrics_legacy'
                );
                """,
                cancellationToken: cancellationToken
            )
        );

        var rowCount = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(
                "SELECT COUNT(*) FROM metrics_legacy;",
                cancellationToken: cancellationToken
            )
        );

        return new LegacyMigrationResult(isHypertable, rowCount);
    }

    public async Task<TieringAttemptResult> ProbeTieringAsync(
        CancellationToken cancellationToken = default
    )
    {
        await using var connection = await OpenConnectionAsync(cancellationToken);

        var functionAvailable = await connection.ExecuteScalarAsync<bool>(
            new CommandDefinition(
                """
                SELECT EXISTS (
                    SELECT 1
                    FROM pg_proc
                    WHERE proname = 'add_tiering_policy'
                );
                """,
                cancellationToken: cancellationToken
            )
        );

        if (!functionAvailable)
        {
            return new TieringAttemptResult(false, false, "Current TimescaleDB build does not expose add_tiering_policy().");
        }

        try
        {
            await connection.ExecuteAsync(
                new CommandDefinition(
                    """
                    SELECT add_tiering_policy(
                        'metrics_1h',
                        INTERVAL '180 days'
                    );
                    """,
                    cancellationToken: cancellationToken
                )
            );

            return new TieringAttemptResult(true, true, "Tiering policy was configured successfully.");
        }
        catch (PostgresException ex)
        {
            return new TieringAttemptResult(
                true,
                false,
                $"Tiering is present but not available in this environment: {ex.MessageText}"
            );
        }
    }

    private MetricsDbContext CreateDbContext()
    {
        var optionsBuilder = new DbContextOptionsBuilder<MetricsDbContext>()
            .UseNpgsql(_connectionString);

        return new MetricsDbContext(optionsBuilder.Options);
    }

    private async Task<NpgsqlConnection> OpenConnectionAsync(
        CancellationToken cancellationToken
    )
    {
        var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);
        return connection;
    }
}
