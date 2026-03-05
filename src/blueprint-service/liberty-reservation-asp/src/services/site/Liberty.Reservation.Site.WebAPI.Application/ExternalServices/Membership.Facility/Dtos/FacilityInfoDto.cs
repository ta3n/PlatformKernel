namespace Liberty.Reservation.Site.WebAPI.Application.ExternalServices.Membership.Facility.Dtos;

public record FacilityInfoDto
{
    public long Id { get; set; }
    public string? Code { get; set; }
    public string? RecordCode { get; set; }

    public string? Name { get; set; }
    // public string? Kana { get; set; }
    // public string? Email { get; set; }
    // public string? Tel { get; set; }
    // public string? Mobile { get; set; }
    // public string? Postcode { get; set; }
    // public string? CountryCode { get; set; }
    // public string? Address1 { get; set; }
    // public string? Address2 { get; set; }
    // public string? Address3 { get; set; }
    // public string? Address4 { get; set; }
}
