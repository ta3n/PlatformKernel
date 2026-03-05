namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;

public record FacilityDetailAccessResponse
{
    public string? Code { get; init; }
    public float Latitude { get; init; }
    public float Longitude { get; init; }
    public string? AccessInfoComment { get; init; }
    public bool? ExistsParking { get; init; }
    public string? ParkingInfoComment { get; init; }
    public bool? CanTransfer { get; init; }
    public string? TransferComment { get; init; }
    public string? NearStationInfoComment { get; init; }
}
