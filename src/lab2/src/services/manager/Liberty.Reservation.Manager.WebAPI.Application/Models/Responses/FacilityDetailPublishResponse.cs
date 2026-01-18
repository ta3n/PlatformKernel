namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;

public record FacilityDetailPublishResponse
{
    public string? Code { get; init; }
    public string? Heading1 { get; init; }
    public string? PrPointComment { get; init; }
    public string? EquipmentInfoComment { get; init; }
    public string? RoomInfoComment { get; init; }
    public string? AmenityInfoComment { get; init; }
    public string? LeisureInfoComment { get; init; }
    public string? FaqInfoComment { get; init; }
    public string? OtherInfoComment { get; init; }
    public string? MapUrl { get; init; }
}
