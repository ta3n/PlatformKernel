using SharedKernel.Hangfire.Exceptions;
using SharedKernel.Hangfire.Models;

namespace SharedKernel.Hangfire.Utils;

/// <summary>
/// Validates scheduler job definitions before they are stored or scheduled.
/// </summary>
public static class SchedulerJobDefinitionValidator
{
    /// <summary>
    /// Ensures a job key is present.
    /// </summary>
    /// <param name="jobKey">The job key.</param>
    /// <returns>The validated job key.</returns>
    public static string EnsureJobKey(
        string jobKey
    )
    {
        return !string.IsNullOrWhiteSpace(jobKey)
            ? jobKey
            : throw new InvalidSchedulerJobDefinitionException("JobKey must not be empty.");
    }

    /// <summary>
    /// Ensures a recurring job definition is valid.
    /// </summary>
    /// <param name="definition">The job definition.</param>
    public static void EnsureValidRecurring(
        SchedulerJobDefinition definition
    )
    {
        EnsureValidTarget(definition);

        if (string.IsNullOrWhiteSpace(definition.CronExpression))
        {
            throw new InvalidSchedulerJobDefinitionException("CronExpression must not be empty.");
        }

        if (definition.Status == SchedulerJobStatus.Removed)
        {
            throw new InvalidSchedulerJobDefinitionException("Removed jobs cannot be registered.");
        }
    }

    /// <summary>
    /// Ensures target dispatch metadata is valid.
    /// </summary>
    /// <param name="definition">The job definition.</param>
    public static void EnsureValidTarget(
        SchedulerJobDefinition definition
    )
    {
        ArgumentNullException.ThrowIfNull(definition);

        EnsureJobKey(definition.JobKey);
        EnsureRequired(definition.TargetService, nameof(definition.TargetService));
        EnsureRequired(definition.GrpcMethod, nameof(definition.GrpcMethod));
        EnsureRequired(definition.PayloadJson, nameof(definition.PayloadJson));
        EnsureRequired(definition.SubSystemId, nameof(definition.SubSystemId));
        EnsureRequired(definition.CompanyId, nameof(definition.CompanyId));
        EnsureRequired(definition.ProjectId, nameof(definition.ProjectId));
        EnsureRequired(definition.RequestedBy, nameof(definition.RequestedBy));
        EnsureRequired(definition.TimeZoneId, nameof(definition.TimeZoneId));
    }

    private static void EnsureRequired(
        string value,
        string name
    )
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidSchedulerJobDefinitionException($"{name} must not be empty.");
        }
    }
}
