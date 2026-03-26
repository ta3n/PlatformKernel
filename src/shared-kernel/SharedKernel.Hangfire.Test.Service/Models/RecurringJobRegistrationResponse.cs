namespace SharedKernel.Hangfire.Test.Service.Models;

public sealed record RecurringJobRegistrationResponse(
    string JobId,
    string CronExpression,
    DateTimeOffset ScheduledForUtc
)
{
    public static RecurringJobRegistrationResponse Create(
        string scenarioId
    )
    {
        var scheduledForUtc = DateTimeOffset.UtcNow
            .AddMinutes(1);
        scheduledForUtc = new DateTimeOffset(
            scheduledForUtc.Year,
            scheduledForUtc.Month,
            scheduledForUtc.Day,
            scheduledForUtc.Hour,
            scheduledForUtc.Minute,
            0,
            TimeSpan.Zero
        );

        return new RecurringJobRegistrationResponse(
            JobId: $"recurring:{scenarioId}",
            CronExpression: $"{scheduledForUtc.Minute} {scheduledForUtc.Hour} {scheduledForUtc.Day} {scheduledForUtc.Month} *",
            ScheduledForUtc: scheduledForUtc
        );
    }
}
