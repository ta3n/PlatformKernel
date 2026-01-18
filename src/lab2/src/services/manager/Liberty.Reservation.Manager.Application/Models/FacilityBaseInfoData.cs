using Liberty.Entity.ValueObjects;

namespace Liberty.Reservation.Manager.Application.Models;

public record FacilityBaseInfoData(
    long Id,
    MultilingualText? Name,
    string? Code,
    string? PhoneNumber,
    MultilingualText? Address1,
    MultilingualText? Address2,
    MultilingualText? Address3,
    string? Url,
    MultilingualText? Heading1
);
