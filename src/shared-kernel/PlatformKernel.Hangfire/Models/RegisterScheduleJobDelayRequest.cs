namespace PlatformKernel.Hangfire.Models;

/// <summary>
/// Represents a request to register a scheduled job with a specified delay.
/// Inherits from <see cref="BaseJobRequest"/> to provide common job-related properties.
/// </summary>
/// <remarks>
/// This class is used to schedule a job with a delay for execution.
/// It includes details about the event, job name, serialized job data in JSON format,
/// and the time delay before execution.
/// </remarks>
public record RegisterScheduleJobDelayRequest(
    string EventName,
    string JobName,
    string JsonData,
    TimeSpan Delay
) : BaseJobRequest(EventName, JobName, JsonData);
