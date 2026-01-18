namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;

public record PlanUpdateCancelRequest(
    bool? IsCancelSameAccept,
    int? CancelDayLimit,
    TimeSpan? CancelLimit,
    long? CancellationId
);
