namespace Liberty.Hangfire;

/// <summary>
/// Contains constants for API endpoints used in job registration processes.
/// </summary>
public static class RegisterJobEndpoint
{
    /// <summary>
    /// Represents the endpoint URL for registering recurring jobs in the system.
    /// </summary>
    public const string RegisterRecurringJobEndpoint = "api/register/recurring-jobs";

    /// <summary>
    /// Represents the endpoint used to register a scheduled job to execute at a specific time.
    /// </summary>
    /// <remarks>
    /// This constant defines the endpoint used by the application to interact with the BatchSchedulerService
    /// for scheduling jobs that execute at a specific time. It is commonly utilized in methods responsible
    /// for calling the external API service to schedule tasks.
    /// </remarks>
    public const string RegisterScheduleJobAtEndpoint = "api/register/schedule-jobs/at";

    /// <summary>
    /// Represents the endpoint URL for scheduling delayed jobs in the batch job scheduler service.
    /// This constant is used for sending requests to schedule jobs with a delay through
    /// the external batch scheduler API.
    /// </summary>
    public const string RegisterScheduleJobDelayEndpoint = "api/register/schedule-jobs/delay";
}
