namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;

public record PlanCancelResponse
{
    public long Id { get; init; }
    public bool IsCancelSameAccept { get; init; }
    public int? CancelDayLimit { get; init; }
    public TimeSpan? CancelLimit { get; init; }
    public long? CancellationId { get; init; }
}
