using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

namespace Liberty.Reservation.Application.Models.Requests;

public record BookingExternalInfoRequest(
    string? UserCode,
    IEnumerable<Question>? SelectedQuestions
);
