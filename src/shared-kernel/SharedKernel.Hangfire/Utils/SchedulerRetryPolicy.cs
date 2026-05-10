using System.Reflection;
using SharedKernel.Hangfire.Exceptions;

namespace SharedKernel.Hangfire.Utils;

/// <summary>
/// Classifies scheduler job exceptions for Hangfire retry behavior.
/// </summary>
public static class SchedulerRetryPolicy
{
    /// <summary>
    /// Determines whether a job failure should be retried.
    /// </summary>
    /// <param name="exception">The failure exception.</param>
    /// <returns>True when Hangfire should retry the job.</returns>
    public static bool ShouldRetry(
        Exception exception
    )
    {
        ArgumentNullException.ThrowIfNull(exception);

        var unwrapped = Unwrap(exception);

        return unwrapped switch
        {
            SchedulerJobException schedulerException => schedulerException.Retryable,
            ArgumentException => false,
            UnauthorizedAccessException => false,
            TimeoutException => true,
            OperationCanceledException => true,
            _ => true
        };
    }

    private static Exception Unwrap(
        Exception exception
    )
    {
        if (exception is TargetInvocationException { InnerException: not null } targetInvocationException)
        {
            return Unwrap(targetInvocationException.InnerException!);
        }

        if (exception is AggregateException { InnerExceptions.Count: 1 } aggregateException)
        {
            return Unwrap(aggregateException.InnerExceptions[0]);
        }

        return exception;
    }
}
