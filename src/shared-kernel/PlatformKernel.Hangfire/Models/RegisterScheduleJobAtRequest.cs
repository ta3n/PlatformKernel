namespace PlatformKernel.Hangfire.Models;

/// <summary>
/// Represents a request to register a scheduled job to be executed at a specific time.
/// </summary>
/// <remarks>
/// This record is used to encapsulate the details of a job that will be scheduled,
/// including its associated event name, job name, payload data in JSON format,
/// and the specific timestamp at which the job should execute.
/// </remarks>
public record RegisterScheduleJobAtRequest(
    string EventName,
    string JobName,
    string JsonData,
    DateTimeOffset At
) : BaseJobRequest(EventName, JobName, JsonData);
