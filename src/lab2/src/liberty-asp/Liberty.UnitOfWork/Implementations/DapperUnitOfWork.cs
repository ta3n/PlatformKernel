using Liberty.UnitOfWork.Abstractions;

namespace Liberty.UnitOfWork.Implementations;

using System.Data;
using System.Data.Common;

public sealed class DapperUnitOfWork(
    IDbConnection connection
) : IDapperUnitOfWork
{
    private readonly IDbConnection _connection = connection ?? throw new ArgumentNullException(nameof(connection));
    private bool _disposed;

    public IDbConnection Connection => _connection;

    public IDbTransaction? Transaction { get; private set; }

    public bool HasActiveTransaction => Transaction is not null;

    public async Task BeginTransactionAsync(
        IsolationLevel isolationLevel = IsolationLevel.Unspecified,
        CancellationToken cancellationToken = default
    )
    {
        ThrowIfDisposed();

        if (Transaction is not null)
        {
            throw new InvalidOperationException("Transaction already started");
        }

        await OpenIfNeededAsync(_connection, cancellationToken).ConfigureAwait(false);

        Transaction = isolationLevel == IsolationLevel.Unspecified
            ? _connection.BeginTransaction()
            : _connection.BeginTransaction(isolationLevel);
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();

        if (Transaction is null)
        {
            throw new InvalidOperationException("No active transaction to commit");
        }

        try
        {
            if (Transaction is DbTransaction dbTx)
            {
                await dbTx.CommitAsync(cancellationToken).ConfigureAwait(false);
            }
            else
            {
                Transaction.Commit();
            }
        }
        finally
        {
            DisposeTransaction();
            CloseIfNeeded(_connection);
        }
    }

    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();

        if (Transaction is null)
        {
            throw new InvalidOperationException("No active transaction to rollback.");
        }

        try
        {
            if (Transaction is DbTransaction dbTx)
            {
                await dbTx.RollbackAsync(cancellationToken).ConfigureAwait(false);
            }
            else
            {
                Transaction.Rollback();
            }
        }
        finally
        {
            DisposeTransaction();
            CloseIfNeeded(_connection);
        }
    }

    public void Dispose()
    {
        if (_disposed) return;

        try
        {
            if (Transaction is not null)
            {
                try { Transaction.Rollback(); } catch { /* swallow to avoid masking */ }
            }

            DisposeTransaction();

            CloseIfNeeded(_connection);

            _connection.Dispose();
        }
        finally
        {
            _disposed = true;
        }
    }

    private void DisposeTransaction()
    {
        Transaction?.Dispose();
        Transaction = null;
    }

    private static async Task OpenIfNeededAsync(IDbConnection connection, CancellationToken cancellationToken)
    {
        if (connection.State == ConnectionState.Open) return;

        if (connection is DbConnection dbConnection)
        {
            await dbConnection.OpenAsync(cancellationToken).ConfigureAwait(false);
            return;
        }

        // Fallback sync
        connection.Open();
    }

    private static void CloseIfNeeded(IDbConnection connection)
    {
        if (connection.State != ConnectionState.Open) return;
        connection.Close();
    }

    private void ThrowIfDisposed()
    {
        if (_disposed) throw new ObjectDisposedException(nameof(DapperUnitOfWork));
    }
}

