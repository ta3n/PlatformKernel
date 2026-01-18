namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;

public record PlanUpdatePaymentMethodRequest(
    bool? IsOnSidePayment,
    bool? IsOnLinePayment
);
