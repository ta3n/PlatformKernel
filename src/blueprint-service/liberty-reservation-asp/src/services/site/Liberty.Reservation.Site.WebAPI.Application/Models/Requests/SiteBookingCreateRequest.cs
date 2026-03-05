using Liberty.Reservation.Application.Models.Requests;
using Newtonsoft.Json;

namespace Liberty.Reservation.Site.WebAPI.Application.Models.Requests;

public record SiteBookingCreateRequest(
    [property: JsonRequired] long CheckInDate,
    string? CheckInTime,
    string? CheckOutTime,
    [property: JsonRequired] PaymentTypes PaymentType,
    BookingAdjustRequest Adjust,
    IEnumerable<QuestionOfBookingCreateRequest>? PlanQuestions,
    IEnumerable<QuestionOfBookingCreateRequest>? OptionsQuestions
)
{
    public DateTime BookingDate { get; set; } = DateTime.UtcNow;
}
