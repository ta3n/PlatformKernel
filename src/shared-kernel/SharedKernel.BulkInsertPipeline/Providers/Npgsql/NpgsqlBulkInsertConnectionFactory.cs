using Microsoft.Extensions.Logging;
using Npgsql;
using SharedKernel.BulkInsertPipeline.Mapping;

namespace SharedKernel.BulkInsertPipeline.Providers.Npgsql;

public sealed class NpgsqlBulkInsertConnectionFactory<T> :
    IAsyncDisposable
{
    private readonly BulkInsertEntityDescriptor<T> _descriptor;
    private readonly ILogger<NpgsqlBulkInsertConnectionFactory<T>> _logger;
    private readonly NpgsqlDataSource? _bulkDataSource;
    private readonly NpgsqlDataSource? _fallbackDataSource;

    public NpgsqlBulkInsertConnectionFactory(
        BulkInsertEntityDescriptor<T> descriptor,
        ILogger<NpgsqlBulkInsertConnectionFactory<T>> logger
    )
    {
        _descriptor = descriptor;
        _logger = logger;

        if (!string.IsNullOrWhiteSpace(_descriptor.Connection.DirectConnectionString))
        {
            _bulkDataSource = BuildDataSource(_descriptor.Connection.DirectConnectionString);
        }

        if (!string.IsNullOrWhiteSpace(_descriptor.Connection.OltpConnectionString))
        {
            _fallbackDataSource = BuildDataSource(_descriptor.Connection.OltpConnectionString);
        }
    }

    public ValueTask<NpgsqlConnection> OpenConnectionAsync(
        CancellationToken ct = default
    )
    {
        var dataSource = _bulkDataSource ?? _fallbackDataSource;
        if (dataSource is null)
        {
            throw new InvalidOperationException(
                $"Entity '{typeof(T).FullName}' does not have a database connection configured. Register a direct PostgreSQL connection string for COPY or an OLTP connection string for fallback providers."
            );
        }

        return dataSource.OpenConnectionAsync(ct);
    }

    public ValueTask<NpgsqlConnection> OpenDirectConnectionAsync(
        CancellationToken ct = default
    )
    {
        if (_bulkDataSource is null)
        {
            throw new InvalidOperationException(
                $"Entity '{typeof(T).FullName}' requires a direct PostgreSQL connection for binary COPY. Do not send COPY traffic through PgBouncer."
            );
        }

        return _bulkDataSource.OpenConnectionAsync(ct);
    }

    public async ValueTask DisposeAsync()
    {
        if (_bulkDataSource is not null)
        {
            await _bulkDataSource.DisposeAsync().ConfigureAwait(false);
        }

        if (_fallbackDataSource is not null && !ReferenceEquals(_fallbackDataSource, _bulkDataSource))
        {
            await _fallbackDataSource.DisposeAsync().ConfigureAwait(false);
        }
    }

    private NpgsqlDataSource BuildDataSource(
        string connectionString
    )
    {
        var builder = new NpgsqlDataSourceBuilder(connectionString);
        var dataSource = builder.Build();

        _logger.LogInformation(
            "Configured bulk insert data source for entity {EntityName}. DirectConnection={DirectConnectionConfigured} OltpConnection={OltpConnectionConfigured}",
            typeof(T).Name,
            !string.IsNullOrWhiteSpace(_descriptor.Connection.DirectConnectionString),
            !string.IsNullOrWhiteSpace(_descriptor.Connection.OltpConnectionString)
        );

        return dataSource;
    }
}
