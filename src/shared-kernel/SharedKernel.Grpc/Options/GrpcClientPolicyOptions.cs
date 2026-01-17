namespace SharedKernel.Grpc.Options;

public class GrpcClientPolicyOptions
{
    public static readonly string Name = "GrpcClientPolicy";

    public bool Enabled { get; set; } = false;

    public int Timeout { get; set; } = 20; // 20 seconds

    public int Retry { get; set; } = 5; // 5 times

    public int ExceptionsAllowedBeforeBreaking { get; set; } = 3; // 3 exceptions

    public int BreakDuration { get; set; } = 20; // 20 seconds
}
