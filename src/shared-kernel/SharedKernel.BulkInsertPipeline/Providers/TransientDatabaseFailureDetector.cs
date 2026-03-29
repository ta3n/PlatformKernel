using Npgsql;

namespace SharedKernel.BulkInsertPipeline.Providers;

internal static class TransientDatabaseFailureDetector
{
    public static bool IsTransient(
        Exception exception
    )
    {
        return exception switch
        {
            TimeoutException => true,
            OperationCanceledException => false,
            PostgresException postgresException => postgresException.SqlState is "40001" or "40P01" or "53300" or "57P03"
                || postgresException.IsTransient,
            NpgsqlException npgsqlException => npgsqlException.IsTransient,
            _ when exception.InnerException is not null => IsTransient(exception.InnerException),
            _ => false
        };
    }
}
