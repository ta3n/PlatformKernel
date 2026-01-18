namespace Liberty.Reservation.Site.WebAPI.Application.Models.Responses;

public record BookingSpecialNoteResponse
{
    public string? Meal { get; set; }
    public string? Other { get; set; }
    public string? Payment { get; set; }

}

