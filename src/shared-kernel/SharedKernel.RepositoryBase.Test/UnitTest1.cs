using Microsoft.EntityFrameworkCore;
using SharedKernel.Entity;
using SharedKernel.RepositoryBase.Implementations;
using SharedKernel.Specification;

namespace SharedKernel.RepositoryBase.Test;

public class UnitTest1
{
    [Fact]
    public async Task GenericRepository_AddAsyncAndGetPagedAsync_WorkAsExpected()
    {
        await using var dbContext = CreateContext();
        var repository = new GenericRepository<TestEntity>(dbContext);

        await repository.AddRangeAsync(
        [
            new TestEntity { Name = "alpha", DisplayOrder = 1 },
            new TestEntity { Name = "beta", DisplayOrder = 2 },
            new TestEntity { Name = "gamma", DisplayOrder = 3 }
        ], autoSave: true);

        var secondPage = await repository.GetPagedAsync(2, 1);

        Assert.Single(secondPage);
        Assert.Equal("beta", secondPage[0].Name);
    }

    [Fact]
    public async Task FluentRepository_GetQueryable_OrdersByDisplayOrderDescending()
    {
        await using var dbContext = CreateContext();
        dbContext.Entities.AddRange(
            new TestEntity { Name = "low", DisplayOrder = 1 },
            new TestEntity { Name = "high", DisplayOrder = 10 });
        await dbContext.SaveChangesAsync();

        var repository = new TestFluentRepository(dbContext);
        var names = await repository.GetQueryable().Select(entity => entity.Name).ToListAsync();

        Assert.Equal(["high", "low"], names);
    }

    [Fact]
    public async Task FluentRepository_GetOneAsync_AppliesSpecificationCriteria()
    {
        await using var dbContext = CreateContext();
        dbContext.Entities.AddRange(
            new TestEntity { Name = "alpha", DisplayOrder = 1 },
            new TestEntity { Name = "beta", DisplayOrder = 2 });
        await dbContext.SaveChangesAsync();

        var repository = new TestFluentRepository(dbContext);
        var entity = await repository.GetOneAsync(new NameSpecification("beta"));

        Assert.NotNull(entity);
        Assert.Equal("beta", entity!.Name);
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

    private sealed class TestFluentRepository(TestDbContext dbContext) : FluentRepository<TestEntity>(dbContext);

    private sealed class NameSpecification(string name) : SpecificationBase<TestEntity>
    {
        public override System.Linq.Expressions.Expression<Func<TestEntity, bool>> Criteria => entity => entity.Name == name;
    }
}
