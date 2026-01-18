using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Data.Common;

namespace Liberty.UnitOfWork;

public sealed class CustomDbConnectionInterceptor(
    IDbConnectionManager dbConnectionManager
) : DbConnectionInterceptor
{
    public override async ValueTask<InterceptionResult> ConnectionOpeningAsync(
        DbConnection connection,
        ConnectionEventData eventData,
        InterceptionResult result,
        CancellationToken cancellationToken = default
    )
    {
        if (dbConnectionManager.Enabled)
        {
            await dbConnectionManager.WaitAsync();
        }

        return await base.ConnectionOpeningAsync(
            connection,
            eventData,
            result,
            cancellationToken
        );
    }

    public override InterceptionResult ConnectionOpening(
        DbConnection connection,
        ConnectionEventData eventData,
        InterceptionResult result
    )
    {
        if (dbConnectionManager.Enabled)
        {
            dbConnectionManager.Wait();
        }

        return base.ConnectionOpening(
            connection,
            eventData,
            result
        );
    }

    public override async Task ConnectionFailedAsync(
        DbConnection connection,
        ConnectionErrorEventData eventData,
        CancellationToken cancellationToken = new()
    )
    {
        if (dbConnectionManager.Enabled)
        {
            dbConnectionManager.Release();
        }

        await base.ConnectionFailedAsync(
            connection,
            eventData,
            cancellationToken
        );
    }

    public override void ConnectionFailed(
        DbConnection connection,
        ConnectionErrorEventData eventData
    )
    {
        if (dbConnectionManager.Enabled)
        {
            dbConnectionManager.Release();
        }

        base.ConnectionFailed(connection, eventData);
    }

    public override async Task ConnectionClosedAsync(
        DbConnection connection,
        ConnectionEndEventData eventData
    )
    {
        if (dbConnectionManager.Enabled)
        {
            dbConnectionManager.Release();
        }

        await base.ConnectionClosedAsync(connection, eventData);
    }

    public override void ConnectionClosed(
        DbConnection connection,
        ConnectionEndEventData eventData
    )
    {
        if (dbConnectionManager.Enabled)
        {
            dbConnectionManager.Release();
        }

        base.ConnectionClosed(connection, eventData);
    }
}
