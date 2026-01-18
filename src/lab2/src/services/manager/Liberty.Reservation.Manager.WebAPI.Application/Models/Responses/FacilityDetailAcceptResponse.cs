namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;

public record FacilityDetailAcceptResponse
{
    public string? Code { get; init; }
    public bool IsAcceptChildren { get; init; }
    public string? AcceptChildrenInfoComment { get; init; }
    public bool IsAcceptPet { get; init; }
    public string? AcceptPetInfoComment { get; init; }
    public bool IsBarrierFree { get; init; }
    public string? BarrierFreeInfoComment { get; init; }
}
