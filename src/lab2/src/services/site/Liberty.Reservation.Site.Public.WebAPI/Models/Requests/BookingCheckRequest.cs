using System.Text.Json.Serialization;

namespace Liberty.Reservation.Site.Public.WebAPI.Models.Requests;

public record BookingCheckRequest(
    [property: JsonRequired] long CheckInDate,
    [property: JsonRequired] int RestNumber,
    [property: JsonRequired] int RoomNumber,
    [property: JsonRequired] List<OptionOfAppDateRequest> OptionsOfAppDate,
    [property: JsonRequired] List<NumberPeopleOfAppDateRequest> NumberPeopleOfAppDate
);

public record OptionOfAppDateRequest(
    long AppDateId,
    List<OptionOfRoomResquest> OptionsOfRoom
);

public record OptionOfRoomResquest(
    long RoomIndex,
    List<OptionResquest> Options
);

public record OptionResquest(
    long Id,
    string? Name,
    decimal? Price,
    int? Number
);

public record NumberPeopleOfAppDateRequest(
    long AppDateId,
    List<NumberOfPeopleRequest> People
);

public record NumberOfPeopleRequest(
    int? Price,
    int? SpaTax,
    int? TotalSpaTax,
    int? TotalPrice,
    int? MalePersons,
    int? FemalePersons,
    int? NonePersons,
    long PersonAgeTypeId,
    string? Name
);
