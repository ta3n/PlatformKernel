namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;

public record FacilityUpdatePublicationInformationRequest(
    string? Heading1,
    string? PrPointComment,
    string? EquipmentInfoComment,
    string? RoomInfoComment,
    string? AmenityInfoComment,
    string? LeisureInfoComment,
    string? FaqInfoComment,
    string? OtherInfoComment,
    string? MapUrl
);
