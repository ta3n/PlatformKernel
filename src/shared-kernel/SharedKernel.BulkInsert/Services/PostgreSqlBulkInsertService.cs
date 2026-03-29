using Microsoft.EntityFrameworkCore;
using SharedKernel.BulkInsert.Abstractions;
using SharedKernel.BulkInsert.Internal;
using SharedKernel.BulkInsert.Models;

namespace SharedKernel.BulkInsert.Services;

internal sealed class PostgreSqlBulkInsertService : IPostgreSqlBulkInsertService
{
    private readonly IReadOnlyDictionary<PostgreSqlBulkInsertProvider, IPostgreSqlBulkInsertStrategy> _strategies;
    private readonly IPostgreSqlBulkInsertMetadataResolver _metadataResolver;

    public PostgreSqlBulkInsertService(
        IEnumerable<IPostgreSqlBulkInsertStrategy> strategies,
        IPostgreSqlBulkInsertMetadataResolver metadataResolver
    )
    {
        ArgumentNullException.ThrowIfNull(strategies);

        _strategies = strategies.ToDictionary(
            strategy => strategy.Provider,
            strategy => strategy
        );
        _metadataResolver = metadataResolver ?? throw new ArgumentNullException(nameof(metadataResolver));
    }

    public Task<int> BulkInsertAsync<TEntity>(
        DbContext dbContext,
        IEnumerable<TEntity> entities,
        PostgreSqlBulkInsertProvider provider = PostgreSqlBulkInsertProvider.RepoDb,
        PostgreSqlBulkInsertOptions? options = null,
        CancellationToken cancellationToken = default
    )
        where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(dbContext);
        ArgumentNullException.ThrowIfNull(entities);

        if (dbContext.Database.GetDbConnection() is not Npgsql.NpgsqlConnection)
        {
            throw new NotSupportedException("SharedKernel.BulkInsert only supports PostgreSQL via Npgsql.");
        }

        if (!_strategies.TryGetValue(provider, out var strategy))
        {
            throw new NotSupportedException(
                $"Bulk insert provider '{provider}' has not been registered."
            );
        }

        var data = entities as IReadOnlyCollection<TEntity> ?? [.. entities];
        if (data.Count == 0)
        {
            return Task.FromResult(0);
        }

        options ??= new PostgreSqlBulkInsertOptions();
        if (options.TimeoutSeconds is <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(options),
                "TimeoutSeconds must be greater than zero."
            );
        }

        var metadata = _metadataResolver.Resolve<TEntity>(dbContext);

        return strategy.BulkInsertAsync(
            dbContext,
            data,
            metadata,
            options,
            cancellationToken
        );
    }
}
