namespace SharedKernel.Hangfire.Test.Service.Options;

public sealed class TestHangfireServiceOptions
{
    public const string SectionName = "TestService";

    public string? InstanceId { get; set; }

    public int StateDatabase { get; set; } = 1;

    public string StatePrefix { get; set; } = "hangfire:test:state:";
}
