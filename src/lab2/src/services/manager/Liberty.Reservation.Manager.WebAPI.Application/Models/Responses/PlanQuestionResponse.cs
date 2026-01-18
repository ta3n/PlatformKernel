namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;

public record PlanQuestionResponse
{
    public long Id { get; init; }
    public IEnumerable<QuestionOfPlanQuestionResponse>? Questions { get; init; }
}

public record QuestionOfPlanQuestionResponse(
    long Id,
    string? Name,
    bool IsEnabled
);
