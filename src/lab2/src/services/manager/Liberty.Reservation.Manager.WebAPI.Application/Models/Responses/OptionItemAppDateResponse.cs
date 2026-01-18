namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;

public record OptionItemAppDateResponse(
    long AppDateId,
    long OptionItemId,
    string OptionItemName,
    int SellNumber,
    bool IsNotSold
)
{
    public int RemainNumber { get; set; }
    public int ReservedNumber { get; set; }
}
