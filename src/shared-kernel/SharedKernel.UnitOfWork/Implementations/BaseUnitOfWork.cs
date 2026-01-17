using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;
using System.Linq.Expressions;
using SharedKernel.UnitOfWork.Abstractions;

namespace SharedKernel.UnitOfWork.Implementations;

public class BaseUnitOfWork : IUnitOfWork
{
    private readonly Func<IDbConnection>? _connFactory;
    private IDbConnection? _dbConnection;
    private IDbContextTransaction? _transaction;
    private readonly DbContext? _dbContext;
    private bool _isDisposed;

    protected BaseUnitOfWork(
        DbContext context
    )
    {
        _dbContext = context ?? throw new ArgumentNullException(nameof(context));
    }

    public BaseUnitOfWork(
        Func<IDbConnection> connFactory
    )
    {
        _connFactory = connFactory;
    }

    public DbSet<T> Set<T>() where T : class
    {
        return _dbContext!.Set<T>();
    }

    public IDbConnection? Connection
    {
        get
        {
            if (_dbConnection is null && _connFactory is not null)
            {
                _dbConnection = _connFactory();
            }

            return _dbConnection;
        }
    }

    public async Task BeginTransactionAsync(
        IsolationLevel isolationLevel = IsolationLevel.Unspecified,
        CancellationToken cancellationToken = default
    )
    {
        if (_dbContext is null)
        {
            _transaction = await _dbContext!.Database.BeginTransactionAsync(
                isolationLevel,
                cancellationToken
            );
        }
    }

    public async Task CommitAsync(
        CancellationToken cancellationToken = default
    )
    {
        if (_dbContext != null)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
            if (_transaction != null)
            {
                await _transaction.CommitAsync(cancellationToken);
                await _transaction.DisposeAsync();
            }
        }
    }

    public async Task RollbackAsync(
        CancellationToken cancellationToken = default
    )
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();
        }
    }

    public int SaveChanges()
    {
        if (_dbContext != null)
        {
            return _dbContext.SaveChanges();
        }

        throw new InvalidOperationException("DbContext is null");
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(
        bool disposing
    )
    {
        if (!_isDisposed && disposing)
        {
            _transaction?.Dispose();
            _dbContext?.Dispose();
        }

        _isDisposed = true;
    }

    public async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default
    )
    {
        if (_dbContext != null)
        {
            return await _dbContext.SaveChangesAsync(cancellationToken);
        }

        throw new InvalidOperationException("DbContext is null");
    }

    public void UpdateState<TEntity>(
        TEntity? entity,
        EntityState state
    )
    {
        if (entity is not null && _dbContext?.Entry(entity) is not null)
        {
            _dbContext.Entry(entity).State = state;
        }
    }

    public void SetEntityStateModified<TEntity, TProperty>(
        TEntity? entity,
        Expression<Func<TEntity, TProperty>>? propertyExpression
    ) where TEntity : class where TProperty : class
    {
        if (entity is not null && _dbContext?.Entry(entity) is not null && propertyExpression is not null)
        {
            _dbContext.Entry(entity).Reference(propertyExpression!).IsModified = true;
        }
    }

    public DbContext GetDbContext()
    {
        var dbContext = _dbContext ?? throw new InvalidOperationException("Db context is not null");
        return dbContext;
    }
}
