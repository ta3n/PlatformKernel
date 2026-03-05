namespace Liberty.Reservation.Employee.WebAPI.Application.Models.Requests;

public record FacilityCreateRequest(
    long Id
)
{
    public string? Memo { get; set; }
    public string? SystemEMail { get; set; }
    public bool? IsEnabled { get; set; }
    public bool? CanOnLinePayment { get; set; }
    public bool UseFax { get; set; }
    public string? Fax { get; set; }
};
