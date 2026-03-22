using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using SharedKernel.UnitOfWork;
using SharedKernel.UnitOfWork.Implementations;

namespace SharedKernel.UnitOfWork.Test;

public class UnitTest1
{
    [Fact]
    public void BaseUnitOfWork_LazilyCreatesConnection()
    {
        var factoryCalled = false;
        var unitOfWork = new BaseUnitOfWork(
            () =>
            {
                factoryCalled = true;
                return new FakeDbConnection();
            });

        Assert.False(factoryCalled);
        Assert.NotNull(unitOfWork.Connection);
        Assert.True(factoryCalled);
    }

    [Fact]
    public void BaseUnitOfWork_SaveChangesPersistsEntity()
    {
        using var dbContext = CreateContext();
        var unitOfWork = new TestUnitOfWork(dbContext);
        unitOfWork.Set<TestEntity>().Add(new TestEntity { Name = "kernel" });

        var affected = unitOfWork.SaveChanges();

        Assert.Equal(1, affected);
        Assert.Equal(1, dbContext.Entities.Count());
    }

    [Fact]
    public async Task DbConnectionManager_WaitAsyncExecutesDelegate()
    {
        var manager = new DbConnectionManager(
            new ConnectionPoolOptions
            {
                MaximumNumberOfConcurrentEntries = 1,
                WaitTimeout = 1000
            });
        var executed = false;

        await manager.WaitAsync(
            () =>
            {
                executed = true;
                return Task.CompletedTask;
            });

        Assert.True(executed);
    }

    [Fact]
    public void AddForwardingDbContextFactory_RegistersForwardedFactory()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IDbContextFactory<TestDbContextImpl>>(new StubDbContextFactory());

        services.AddForwardingDbContextFactory<TestDbContextBase, TestDbContextImpl>();

        var provider = services.BuildServiceProvider();
        var factory = provider.GetRequiredService<IDbContextFactory<TestDbContextBase>>();
        var context = factory.CreateDbContext();

        Assert.IsType<TestDbContextImpl>(context);
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

    private sealed class TestEntity
    {
        public int Id { get; set; }

        public string? Name { get; set; }
    }

    private sealed class TestUnitOfWork(DbContext dbContext) : BaseUnitOfWork(dbContext);

    private sealed class FakeDbConnection : DbConnection
    {
        [AllowNull]
        public override string ConnectionString { get; set; } = string.Empty;

        public override string Database => "Fake";

        public override string DataSource => "Fake";

        public override string ServerVersion => "1.0";

        public override ConnectionState State => ConnectionState.Open;

        public override void ChangeDatabase(string databaseName)
        {
        }

        public override void Close()
        {
        }

        public override void Open()
        {
        }

        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
        {
            throw new NotSupportedException();
        }

        protected override DbCommand CreateDbCommand()
        {
            throw new NotSupportedException();
        }
    }

    private abstract class TestDbContextBase : DbContext
    {
        protected TestDbContextBase(DbContextOptions options) : base(options)
        {
        }
    }

    private sealed class TestDbContextImpl : TestDbContextBase
    {
        public TestDbContextImpl() : base(new DbContextOptionsBuilder<TestDbContextImpl>().Options)
        {
        }
    }

    private sealed class StubDbContextFactory : IDbContextFactory<TestDbContextImpl>
    {
        public TestDbContextImpl CreateDbContext()
        {
            return new TestDbContextImpl();
        }

        public Task<TestDbContextImpl> CreateDbContextAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new TestDbContextImpl());
        }
    }
}
