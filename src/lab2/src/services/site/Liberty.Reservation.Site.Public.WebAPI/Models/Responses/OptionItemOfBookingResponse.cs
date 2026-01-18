namespace Liberty.Reservation.Site.Public.WebAPI.Models.Responses;

public record OptionItemOfBookingResponse(
    long Id,
    string? Name,
    string? Description,
    int? Price
)
{
    public int Number => SellNumber - ReservedNumber;
    public int SellNumber { get; set; }
    public int ReservedNumber { get; set; }
    public IEnumerable<FileOfBookingResponse>? Files { get; set; }
    public List<QuestionOfBookingResponse>? Questions { get; set; }
    public string? LastUpdatedAt { get; set; }
};
