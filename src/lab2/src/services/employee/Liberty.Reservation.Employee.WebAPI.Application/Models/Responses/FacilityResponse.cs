namespace Liberty.Reservation.Employee.WebAPI.Application.Models.Responses;

public record FacilityResponse(
    long Id,
    string Code,
    long DisplayOrder,
    bool IsEnabled,
    string? Memo
)
{
    public string? Name { get; set; }
    public string? Kana { get; set; }
    public string? PostCode { get; set; }
    public string? RecordCode { get; set; }
}
