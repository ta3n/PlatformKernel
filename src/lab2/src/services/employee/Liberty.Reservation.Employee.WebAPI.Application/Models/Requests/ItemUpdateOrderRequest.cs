namespace Liberty.Reservation.Employee.WebAPI.Application.Models.Requests;

public record ItemUpdateOrderRequest(
    List<long> Ids
);
