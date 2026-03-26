namespace SharedKernel.Hangfire.Test.Service.Models;

public sealed record ScheduledJobResponse(
    string JobId,
    int DelayMilliseconds
);
