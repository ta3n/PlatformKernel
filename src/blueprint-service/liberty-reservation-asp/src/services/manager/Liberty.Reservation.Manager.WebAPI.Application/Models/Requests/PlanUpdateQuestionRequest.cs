namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;

public record PlanUpdateQuestionRequest(
    List<long>? QuestionIds
);
