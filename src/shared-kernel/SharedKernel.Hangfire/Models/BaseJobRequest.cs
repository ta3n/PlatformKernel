namespace SharedKernel.Hangfire.Models;

/// <summary>
/// Represents the base for job request records in the system.
/// </summary>
/// <remarks>
/// This record serves as a foundation for creating job requests with common fields
/// required for executing or registering jobs. It defines shared properties such as
/// the event name, job name, and job-specific data in JSON format.
/// </remarks>
/// <param name="EventName">
/// The name of the event or task associated with the job.
/// </param>
/// <param name="JobName">
/// The name of the job being registered or executed.
/// </param>
/// <param name="JsonData">
/// JSON-formatted string containing additional data or parameters required for the job.
/// </param>
public record BaseJobRequest(
    string EventName,
    string JobName,
    string JsonData
);
