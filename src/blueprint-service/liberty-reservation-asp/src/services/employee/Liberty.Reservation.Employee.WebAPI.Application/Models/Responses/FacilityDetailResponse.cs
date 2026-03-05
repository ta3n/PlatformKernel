namespace Liberty.Reservation.Employee.WebAPI.Application.Models.Responses;

public record FacilityDetailResponse(
    long Id,
    string? Code,
    string? Name,
    string? Kana,
    string? PostCode,
    string? Address1,
    string? Address2,
    string? Address3,
    string? Address4,
    string? Fax,
    string? SystemEMail,
    string? Memo,
    string? RecordCode,
    bool CanOnLinePayment,
    IEnumerable<SiteOfFacilityResponse>? Sites,
    IEnumerable<FaxServiceOfFacilityResponse>? FaxServices
);

public record FacilityReservationResponse(
    long Id,
    string? Fax,
    bool CanOnLinePayment,
    bool IsEnabled,
    string? Email,
    string? Memo,
    IEnumerable<SiteOfFacilityResponse>? Sites,
    IEnumerable<FaxServiceOfFacilityResponse>? FaxServices
);

public record SiteOfFacilityResponse(
    long Id,
    string? Name,
    bool IsEnabled,
    bool IsVisible
);

public record FaxServiceOfFacilityResponse(
    long Id,
    string? Name,
    bool IsEnabled,
    bool IsVisible
);
