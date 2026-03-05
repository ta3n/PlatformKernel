using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;

namespace Liberty.Application.Domains.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        Task<IDisposable> BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, CancellationToken cancellationToken = default);

        Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    }
    public class UnitOfWork : DbContext, IUnitOfWork
    {
        private IDbContextTransaction _dbContextTransaction;
        private bool _isDispose = false;
        public UnitOfWork(IDbContextTransaction dbContextTransaction, DbContextOptions<DbContext> options) : base(options)
        {
            _dbContextTransaction = dbContextTransaction;
        }
        public async Task<IDisposable> BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, CancellationToken cancellationToken = default)
        {
            _dbContextTransaction = await Database.BeginTransactionAsync(isolationLevel, cancellationToken);
            return _dbContextTransaction;
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            await _dbContextTransaction.CommitAsync(cancellationToken);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_isDispose)
            {
                if (disposing)
                {
                    _dbContextTransaction.Dispose();
                }
            }
            _isDispose = true;
        }

        public override void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
