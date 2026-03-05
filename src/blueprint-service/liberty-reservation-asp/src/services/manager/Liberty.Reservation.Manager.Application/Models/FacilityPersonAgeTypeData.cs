namespace Liberty.Reservation.Manager.Application.Models;

public record FacilityPersonAgeTypeData(
    long FacilityId,
    IEnumerable<PersonAgeType> PersonAgeType
);
