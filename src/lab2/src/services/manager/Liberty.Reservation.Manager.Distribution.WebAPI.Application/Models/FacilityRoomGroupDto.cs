namespace Liberty.Reservation.Manager.Distribution.WebAPI.Application.Models;

public class FacilityRoomGroupDto
{
    public long Id { get; set; }
    public string? Name { get; set; }
    public string? Code { get; set; }
    public string? Url { get; set; }
    public string? Address1 { get; set; }
    public string? Address2 { get; set; }
    public string? Address3 { get; set; }
    public string? Address4 { get; set; }
    public string? Phone { get; set; }
    public string? Heading { get; set; }
    public List<RoomGroupDto>? RoomGroup { get; set; }
}

public record RoomGroupDto(
    string? Code,
    string? Name
);
