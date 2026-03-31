using Microsoft.EntityFrameworkCore;
using SharedKernel.BulkInsert.Abstractions;
using SharedKernel.BulkInsert.Configuration;
using SharedKernel.BulkInsert.Models;

namespace SharedKernel.BulkInsert.Services;

internal sealed class FluentBulkInsertService<TEntity>(
    IPostgreSqlBulkInsertService bulkInsertService,
    FluentBulkInsertEntityOptions<TEntity> options,
    IServiceProvider serviceProvider
)
    :
        IFluentBulkInsertService<TEntity>
    where TEntity : class
{
    private readonly IPostgreSqlBulkInsertService _bulkInsertService =
        bulkInsertService ?? throw new ArgumentNullException(nameof(bulkInsertService));

    private readonly FluentBulkInsertEntityOptions<TEntity> _options = options ?? throw new ArgumentNullException(nameof(options));
    private readonly IServiceProvider _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

    public async Task<int> BulkInsertAsync(
        IEnumerable<TEntity> entities,
        PostgreSqlBulkInsertProvider? provider = null,
        PostgreSqlBulkInsertOptions? options = null,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(entities);

        if (_options.DbContextFactory is null)
        {
            throw new InvalidOperationException(
                $"Entity '{typeof(TEntity).FullName}' does not have a DbContext factory configured. Register one via AddEntity(...).UseDbContextFactory(...) or call the overload that accepts an existing DbContext."
            );
        }

        await using var dbContext = _options.DbContextFactory.Invoke(_serviceProvider);
        return await BulkInsertAsync(
                dbContext,
                entities,
                provider,
                options,
                cancellationToken
            )
            .ConfigureAwait(false);
    }

    public Task<int> BulkInsertAsync(
        DbContext dbContext,
        IEnumerable<TEntity> entities,
        PostgreSqlBulkInsertProvider? provider = null,
        PostgreSqlBulkInsertOptions? options = null,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(dbContext);
        ArgumentNullException.ThrowIfNull(entities);

        return _bulkInsertService.BulkInsertAsync(
            dbContext,
            entities,
            provider ?? _options.Provider,
            MergeOptions(options),
            cancellationToken
        );
    }

    private PostgreSqlBulkInsertOptions? MergeOptions(
        PostgreSqlBulkInsertOptions? options
    )
    {
        var batchSize = options?.BatchSize ?? _options.BatchSize;
        var timeoutSeconds = options?.TimeoutSeconds ?? _options.TimeoutSeconds;

        if (batchSize is null && timeoutSeconds is null)
        {
            return options;
        }

        return new PostgreSqlBulkInsertOptions
        {
            BatchSize = batchSize,
            TimeoutSeconds = timeoutSeconds
        };
    }
}
