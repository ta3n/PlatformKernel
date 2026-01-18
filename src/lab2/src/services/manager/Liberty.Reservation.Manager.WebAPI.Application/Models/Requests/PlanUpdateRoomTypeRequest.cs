namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;

public record PlanUpdateRoomTypeRequest(
    List<long>? RoomTypeIds
);
