namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;

//public record PlanImportantNoteResponse(
//    long Id,
//    string? Payment,
//    string? Meal,
//    string? Other
//);
public record PlanImportantNoteResponse
{
    public long Id { get; init; }
    public string? Payment { get; init; }
    public string? Meal { get; init; }
    public string? Other { get; init; }
}
