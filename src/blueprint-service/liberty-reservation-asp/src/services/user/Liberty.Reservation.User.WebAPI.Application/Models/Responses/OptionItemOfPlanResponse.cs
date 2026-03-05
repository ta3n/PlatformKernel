namespace Liberty.Reservation.User.WebAPI.Application.Models.Responses;

public record OptionItemOfPlanResponse(
    long Id,
    string? Name,
    string? Description,
    int? Price
)
{
    public int Number => SellNumber - ReservedNumber;
    public int SellNumber { get; set; }
    public int ReservedNumber { get; set; }
    public bool IsSelected { get; set; }
    public List<FileOfOptionItemResponse>? Files { get; set; }
};

public record FileOfOptionItemResponse(
    string? Code,
    string? ContentType
);
