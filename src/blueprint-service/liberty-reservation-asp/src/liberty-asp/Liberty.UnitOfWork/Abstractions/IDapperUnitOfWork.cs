using System.Data;

namespace Liberty.UnitOfWork.Abstractions;

public interface IDapperUnitOfWork :  IDisposable
{
    IDbConnection? Connection { get; }
    IDbTransaction? Transaction { get; }
    Task BeginTransactionAsync(
        IsolationLevel isolationLevel = IsolationLevel.Unspecified,
        CancellationToken cancellationToken = default
    );
    Task CommitAsync(
        CancellationToken cancellationToken = default
    );

    Task RollbackAsync(
        CancellationToken cancellationToken = default
    );
}
