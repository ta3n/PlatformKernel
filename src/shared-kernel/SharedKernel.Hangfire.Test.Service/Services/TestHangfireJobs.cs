namespace SharedKernel.Hangfire.Test.Service.Services;

public sealed class TestHangfireJobs
{
    private readonly ScenarioStateTracker _tracker;

    public TestHangfireJobs(
        ScenarioStateTracker tracker
    )
    {
        _tracker = tracker;
    }

    public Task ExecuteScheduledAsync(
        string scenarioId
    )
    {
        return _tracker.RecordScheduledExecutionAsync(scenarioId);
    }

    public Task ExecuteRecurringAsync(
        string scenarioId
    )
    {
        return _tracker.RecordRecurringExecutionAsync(scenarioId);
    }
}
