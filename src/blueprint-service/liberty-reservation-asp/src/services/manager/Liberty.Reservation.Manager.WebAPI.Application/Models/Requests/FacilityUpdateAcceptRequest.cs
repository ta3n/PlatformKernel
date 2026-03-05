namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;

public record FacilityUpdateAcceptRequest(
    bool? IsAcceptChildren,
    string? AcceptChildrenInfoComment,
    bool? IsAcceptPet,
    string? AcceptPetInfoComment,
    bool? IsBarrierFree,
    string? BarrierFreeInfoComment
);
