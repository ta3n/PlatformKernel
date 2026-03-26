namespace SharedKernel.Hangfire.Test.Service.Models;

public sealed record BootstrapLockRequest(
    int HoldMilliseconds = 1000
);
