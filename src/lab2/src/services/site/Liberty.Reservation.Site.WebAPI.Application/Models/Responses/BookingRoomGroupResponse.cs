using Liberty.Reservation.Site.Application.Models.Responses;

namespace Liberty.Reservation.Site.WebAPI.Application.Models.Responses;

public record BookingRoomGroupResponse
{
    public string? Code { get; set; }
    public string? Name { get; set; }
    public string? Tag { get; set; }
    public string? Description { get; set; }
    public string? Overview { get; set; }
    public int CapacityMin { get; set; }
    public int CapacityMax { get; set; }
    public decimal? Size { get; set; }
    public RoomGroupSizeUnitTypes? RoomGroupSizeUnitType { get; set; }
    public bool IsEnabledSmoking { get; set; }
    public bool IsRoomSizeVisible { get; set; }
    public bool IsBedTypeVisible { get; set; }
    public IEnumerable<string> BedTypes { get; set; } = [];
    public IEnumerable<FileOfBookingResponse> Files { get; set; } = [];
    public IEnumerable<string> Categories { get; init; } = [];
    public IEnumerable<string> Amenities { get; init; } = [];
}


