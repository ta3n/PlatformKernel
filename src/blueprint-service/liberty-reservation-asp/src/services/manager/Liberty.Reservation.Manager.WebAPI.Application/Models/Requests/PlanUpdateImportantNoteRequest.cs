namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;

public record PlanUpdateImportantNoteRequest
{
    public string? Payment { get; init; }
    public string? Meal { get; init; }
    public string? Other { get; init; }
}
