using Liberty.Reservation.Application.Models.Responses;

namespace Liberty.Reservation.Application.Models;

public class RoomGroupAppDateAggregationModel
{
    public required List<HotelModel> Hotels { get; set; }

    public string? RoomAdjustmentCode { get; set; }

    public required string UserCode { get; set; }
}
