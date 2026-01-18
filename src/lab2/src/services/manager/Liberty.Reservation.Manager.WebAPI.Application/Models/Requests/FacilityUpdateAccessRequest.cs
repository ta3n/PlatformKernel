using Newtonsoft.Json;

namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;

public record FacilityUpdateAccessRequest(
    [property: JsonRequired] float Latitude,
    [property: JsonRequired] float Longitude,
    string? AccessInfoComment,
    [property: JsonRequired] bool ExistsParking,
    string? ParkingInfoComment,
    [property: JsonRequired] bool CanTransfer,
    string? TransferComment,
    string? NearStationInfoComment
);
