namespace SharedKernel.Hangfire.Test.Service.Models;

public sealed record ScenarioStateResponse(
    string ScenarioId,
    int ScheduledExecutions,
    string[] ScheduledInstances,
    int RecurringExecutions,
    string[] RecurringInstances,
    int BootstrapEntryCount,
    int BootstrapMaxConcurrency,
    string[] BootstrapInstances
);
