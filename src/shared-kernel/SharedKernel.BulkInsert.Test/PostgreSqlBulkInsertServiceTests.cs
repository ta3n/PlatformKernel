using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.BulkInsert;
using SharedKernel.BulkInsert.Abstractions;
using SharedKernel.BulkInsert.Models;

namespace SharedKernel.BulkInsert.Test;

public sealed class PostgreSqlBulkInsertServiceTests(
    PostgreSqlBulkInsertContainerFixture fixture
) : IClassFixture<PostgreSqlBulkInsertContainerFixture>
{
    public static TheoryData<PostgreSqlBulkInsertProvider> Providers =>
    [
        PostgreSqlBulkInsertProvider.EfCore,
        PostgreSqlBulkInsertProvider.Dapper,
        PostgreSqlBulkInsertProvider.RepoDb
    ];

    [Theory]
    [MemberData(nameof(Providers))]
    public async Task BulkInsertAsync_InsertsRows_ForAllProviders(
        PostgreSqlBulkInsertProvider provider
    )
    {
        await using var dbContext = await CreateDbContextAsync();
        var service = CreateService();
        var createdAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);
        var orders = Enumerable.Range(1, 5)
            .Select(
                index => new BulkInsertTestOrder
                {
                    ExternalId = $"order-{index}",
                    Quantity = index * 10,
                    CreatedAt = createdAt.AddMinutes(index)
                }
            )
            .ToArray();

        var insertedRows = await service.BulkInsertAsync(
            dbContext,
            orders,
            provider,
            new PostgreSqlBulkInsertOptions
            {
                BatchSize = 2,
                TimeoutSeconds = 60
            }
        );

        dbContext.ChangeTracker.Clear();

        var persistedRows = await dbContext.Orders
            .AsNoTracking()
            .OrderBy(order => order.Id)
            .ToListAsync();

        Assert.Equal(orders.Length, insertedRows);
        Assert.Equal(orders.Length, persistedRows.Count);
        Assert.Equal(
            orders.Select(order => order.ExternalId),
            persistedRows.Select(order => order.ExternalId)
        );
        Assert.Equal(
            orders.Select(order => order.Quantity),
            persistedRows.Select(order => order.Quantity)
        );
        Assert.All(persistedRows, row => Assert.True(row.Id > 0));
    }

    [Theory]
    [MemberData(nameof(Providers))]
    public async Task BulkInsertAsync_UsesExistingEfCoreTransaction(
        PostgreSqlBulkInsertProvider provider
    )
    {
        await using var dbContext = await CreateDbContextAsync();
        var service = CreateService();
        var orders = Enumerable.Range(1, 3)
            .Select(
                index => new BulkInsertTestOrder
                {
                    ExternalId = $"tx-order-{index}",
                    Quantity = index,
                    CreatedAt = DateTime.UtcNow.AddSeconds(index)
                }
            )
            .ToArray();

        await using var transaction = await dbContext.Database.BeginTransactionAsync();

        var insertedRows = await service.BulkInsertAsync(
            dbContext,
            orders,
            provider,
            new PostgreSqlBulkInsertOptions
            {
                BatchSize = 2
            }
        );

        Assert.Equal(orders.Length, insertedRows);

        await transaction.RollbackAsync();
        dbContext.ChangeTracker.Clear();

        var rowCount = await dbContext.Orders.CountAsync();

        Assert.Equal(0, rowCount);
    }

    private static IPostgreSqlBulkInsertService CreateService()
    {
        var serviceProvider = new ServiceCollection()
            .AddPostgreSqlBulkInsert()
            .BuildServiceProvider();

        return serviceProvider.GetRequiredService<IPostgreSqlBulkInsertService>();
    }

    private async Task<BulkInsertTestDbContext> CreateDbContextAsync()
    {
        var connectionString = await fixture.CreateIsolatedConnectionStringAsync();
        var options = new DbContextOptionsBuilder<BulkInsertTestDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        var dbContext = new BulkInsertTestDbContext(options);
        await dbContext.Database.ExecuteSqlRawAsync("CREATE SCHEMA IF NOT EXISTS bulk_insert;");
        await dbContext.Database.EnsureCreatedAsync();

        return dbContext;
    }
}
