using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Linq.Expressions;

namespace Liberty.UnitOfWork.Abstractions;

public interface IUnitOfWork : IDisposable
{
    DbSet<T> Set<T>() where T : class;

    IDbConnection? Connection { get; }

    Task BeginTransactionAsync(
        CancellationToken cancellationToken,
        IsolationLevel isolationLevel = IsolationLevel.Unspecified
    );

    Task CommitAsync(
        CancellationToken cancellationToken = default
    );

    Task RollbackAsync(
        CancellationToken cancellationToken = default
    );

    int SaveChanges();

    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default
    );

    void UpdateState<TEntity>(
        TEntity? entity,
        EntityState state
    );

    void SetEntityStateModified<TEntity, TProperty>(
        TEntity? entity,
        Expression<Func<TEntity, TProperty>>? propertyExpression
    ) where TEntity : class where TProperty : class;

    DbContext GetDbContext();
}
