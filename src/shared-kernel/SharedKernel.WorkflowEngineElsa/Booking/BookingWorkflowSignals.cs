namespace SharedKernel.WorkflowEngineElsa.Booking;

/// <summary>
/// Shared stimulus names for long-running booking workflows.
/// </summary>
public static class BookingWorkflowSignals
{
    public const string PaymentCompleted = "booking/payment-completed";
    public const string PaymentFailed = "booking/payment-failed";
    public const string BookingCancelled = "booking/cancelled";
    public const string HoldExpired = "booking/hold-expired";
}
