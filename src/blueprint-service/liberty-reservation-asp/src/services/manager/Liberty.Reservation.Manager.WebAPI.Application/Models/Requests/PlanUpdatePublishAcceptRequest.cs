namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;

public record PlanUpdatePublishAcceptRequest(
    bool? UseBookingReception,
    long? BookingReceptionStart,
    long? BookingReceptionEnd,
    bool? UseDisplayDate,
    long? DisplayDateStart,
    long? DisplayDateEnd,
    bool? UseAcceptDate,
    long? AcceptDateStart,
    long? AcceptDateEnd,
    int? AcceptDays,
    int? AcceptMonths,
    PlanAcceptEndLimitTypes? AcceptEndLimitType,
    int? ReceptionDayLimit,
    TimeSpan? ReceptionLimit,
    List<long>? Sites
);
