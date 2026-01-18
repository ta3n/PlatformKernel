using Newtonsoft.Json;

namespace Liberty.Reservation.Employee.WebAPI.Application.Models.Responses;

public record BookingAggregateFaxAuditLogSearchResponse(
    long Id,
    string? FaxNumber,
    string? Subject,
    string? Result,
    bool? IsSuccess,
    string? ReservationCode,
    string? Body
)
{
    [JsonIgnore]
    public long CreatedAtId { get; set; }

    public DateTime? CreatedAt => AppDate.ConvertLongToDateTime(CreatedAtId, null);
    public string ResultMessage { get; set; } = string.Empty;
};
