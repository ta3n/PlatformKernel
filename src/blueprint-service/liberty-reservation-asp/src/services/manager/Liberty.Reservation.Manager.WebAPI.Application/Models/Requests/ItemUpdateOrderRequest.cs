namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;

public record ItemUpdateOrderRequest(
    List<long> Ids
);
