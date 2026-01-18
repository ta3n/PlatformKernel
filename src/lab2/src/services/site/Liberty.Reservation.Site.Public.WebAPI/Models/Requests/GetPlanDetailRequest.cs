namespace Liberty.Reservation.Site.Public.WebAPI.Models.Requests;

public record GetPlanDetailRequest(
    string PlanCode,
    string RoomGroupCode
);
