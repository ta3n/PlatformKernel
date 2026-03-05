namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;

public record RoomGroupUpdatePaymentMethodRequest(
    bool? IsOnSidePayment,
    bool? IsOnLinePayment
) : PlanUpdatePaymentMethodRequest(
    IsOnSidePayment,
    IsOnLinePayment
);
