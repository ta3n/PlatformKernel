namespace SharedKernel.WorkflowEngineElsa.Booking;

/// <summary>
/// Canonical input contract for a booking workflow that reserves inventory through Redis before committing durable state.
/// </summary>
public sealed record BookingWorkflowInput
{
    public required string BookingId { get; init; }

    public required string ReservationId { get; init; }

    public required string InventoryId { get; init; }

    public required IReadOnlyCollection<DateOnly> Dates { get; init; }

    public long Quantity { get; init; } = 1;

    public TimeSpan HoldTtl { get; init; } = TimeSpan.FromMinutes(10);

    public TimeSpan ReservationDataTtl { get; init; } = TimeSpan.FromHours(12);

    public string? CorrelationId { get; init; }
}
