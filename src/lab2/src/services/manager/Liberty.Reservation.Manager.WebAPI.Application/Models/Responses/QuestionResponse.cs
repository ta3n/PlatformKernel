namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;

public record QuestionResponse(
    long Id,
    string? Name,
    string? Description,
    bool IsEnabled,
    bool IsUseInPlan,
    bool IsUseInOption,
    bool HasContent,
    bool IsRequired
);
