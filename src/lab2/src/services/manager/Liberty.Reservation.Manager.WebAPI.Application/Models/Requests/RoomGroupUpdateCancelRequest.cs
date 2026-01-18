namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;

public record RoomGroupUpdateCancelRequest(
    bool? IsCancelSameAccept,
    int? CancelDayLimit,
    TimeSpan? CancelLimit,
    long? CancellationId
) : PlanUpdateCancelRequest(
    IsCancelSameAccept,
    CancelDayLimit,
    CancelLimit,
    CancellationId
);
