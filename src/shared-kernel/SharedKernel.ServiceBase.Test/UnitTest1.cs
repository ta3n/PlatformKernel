using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using SharedKernel.Entity;
using SharedKernel.RepositoryBase;
using SharedKernel.RepositoryBase.Implementations;
using SharedKernel.ServiceBase.Implementations;

namespace SharedKernel.ServiceBase.Test;

public class UnitTest1
{
    [Fact]
    public async Task CreateAsync_PersistsEntity()
    {
        await using var dbContext = CreateContext();
        var repository = new GenericRepository<TestEntity>(dbContext);
        var service = new TestFluentService(repository);

        var entity = await service.CreateAsync(new TestEntity { Name = "kernel", DisplayOrder = 1 });

        Assert.True(entity.Id > 0);
        Assert.Equal(1, await dbContext.Entities.CountAsync());
    }

    [Fact]
    public async Task EnableAsync_UpdatesEnabledState()
    {
        await using var dbContext = CreateContext();
        var entity = new TestEntity { Name = "kernel", DisplayOrder = 1, IsEnabled = false };
        dbContext.Entities.Add(entity);
        await dbContext.SaveChangesAsync();
        dbContext.Entry(entity).State = EntityState.Detached;

        var repository = new GenericRepository<TestEntity>(dbContext);
        var service = new TestFluentService(repository);

        var updated = await service.EnableAsync(entity.Id, true);

        Assert.True(updated.IsEnabled);
    }

    [Fact]
    public void ExposeCacheKey_ReturnsStableHashedKeyWithEntityName()
    {
        using var dbContext = CreateContext();
        var repository = new GenericRepository<TestEntity>(dbContext);
        var service = new TestFluentService(repository);

        var cacheKey = service.ExposeCacheKey("Find", "1");

        Assert.StartsWith("TestEntity:", cacheKey, StringComparison.Ordinal);
        Assert.NotEqual("TestEntity:Find:1", cacheKey);
    }

    private static TestDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new TestDbContext(options);
    }

    private sealed class TestDbContext(DbContextOptions<TestDbContext> options) : DbContext(options)
    {
        public DbSet<TestEntity> Entities => Set<TestEntity>();
    }

    private sealed class TestEntity : EntityData
    {
        public string? Name { get; set; }
    }

    private sealed class TestFluentService(IGenericRepository<TestEntity> repository)
        : FluentService<TestEntity>(NullLogger.Instance, repository, new InvalidOperationException("not found"))
    {
        public string ExposeCacheKey(string methodName, params string[] keys)
        {
            return GetCacheKey(methodName, keys);
        }
    }
}
