namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;

public record RoomGroupUpdateQuestionRequest(
    List<long>? QuestionIds
) : PlanUpdateQuestionRequest(QuestionIds);
