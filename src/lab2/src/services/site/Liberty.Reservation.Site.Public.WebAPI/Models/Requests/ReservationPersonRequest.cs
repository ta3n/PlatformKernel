namespace Liberty.Reservation.Site.Public.WebAPI.Models.Requests;

public record ReservationPersonRequest
{
    public long AppDateId { get; init; }
    public int RestIndex { get; init; }

    public int RoomGroupIndex { get; init; }
    public PersonAgeTypeRequest? PersonAgeType { get; init; }
    public int? Persons { get; init; }

    public int? MalePersons { get; init; }
    public int? FemalePersons { get; init; }
}
