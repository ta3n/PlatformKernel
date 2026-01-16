namespace PlatformKernel.Hangfire.Models;

/// <summary>
/// Represents a request to register a recurring job in the system.
/// </summary>
/// <remarks>
/// This record extends the <see cref="BaseJobRequest"/> and includes additional
/// properties required for configuring recurring jobs, such as the Cron expression.
/// Recurring jobs are scheduled to execute at specific intervals based on the
/// provided Cron expression, enabling automated task execution.
/// </remarks>
/// <param name="EventName">
/// The name of the event or task associated with the job.
/// </param>
/// <param name="JobName">
/// The name of the recurring job being registered.
/// </param>
/// <param name="JsonData">
/// JSON-formatted string containing additional data or parameters for the job.
/// </param>
/// <param name="CronExpression">
/// Cron expression specifying the schedule at which the recurring job should execute.
/// The Cron expression determines the frequency and timing of the job.
/// </param>
public record RegisterRecurringJobRequest(
    string EventName,
    string JobName,
    string JsonData,
    string CronExpression
) : BaseJobRequest(EventName, JobName, JsonData);
