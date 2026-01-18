namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;

public class RoomGroupWithAppDatesResponse
{
    public long RoomGroupId { get; set; }
    public string RoomGroupName { get; set; } = string.Empty;
    public string? GroupName { get; set; }
    public int BaseNumber { get; set; }
    public List<RoomGroupAppDateDetailResponse> AppDates { get; set; } = [];
}

public class RoomGroupAppDateDetailResponse
{
    public long AppDateId { get; set; }
    public int SellNumber { get; set; }
    public bool IsNotSold { get; set; }
    public int ReservedNumber { get; set; }
    public int RemainNumber { get; set; }
}
