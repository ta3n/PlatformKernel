namespace SharedKernel.Hangfire.Test.Service.Models;

public sealed record ScheduleJobRequest(
    int DelayMilliseconds = 1500
);
