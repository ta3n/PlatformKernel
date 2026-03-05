namespace Liberty.Reservation.Manager.Distribution.WebAPI.Application.Models.Requests;

public record GetPriceDataByPlanRoomRequest(
    string? ScAgtPlanCode,
    string? ScAgtRoomCode,
    string? ScAgtSiteCode,
    string? AppointedDate,
    string? AcquireDayNums
);
