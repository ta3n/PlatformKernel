namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;

public record QuestionDetailResponse(
    long Id,
    string? Name,
    string? Description,
    string? FormData,
    bool IsEnabled,
    bool HasContent
)
{
    public QuestionTypes Type { get; init; }
}
