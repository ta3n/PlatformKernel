using Liberty.Reservation.Application.Constants;

namespace Liberty.Reservation.Application.Models.Responses;

public record BookingAuditLogResponse(
    long? ChangeDateId,
    string? ChangedBy,
    IEnumerable<ChangeOfBookingAuditLogResponse>? ChangeItems
)
{
    public DateTime? ChangeDate { get; init; }
}

public record ChangeOfBookingAuditLogResponse(
    string? ChangeItem,
    object? BeforeValue,
    object? AfterValue
)
{
    public object? ChangeItemData { get; set; }
}

public record AppDateOfBookingAuditLogResponse(
    long AppDateId,
    int RestIndex,
    IEnumerable<RoomOfBookingAuditLogResponse> Rooms
)
{
    public IEnumerable<AppDatePersonFlatOfBookingAuditLogResponse> GetFlatPersons()
    {
        var result = new List<AppDatePersonFlatOfBookingAuditLogResponse>();

        foreach (var room in Rooms)
        {
            if (room.Persons is null)
            {
                continue;
            }

            foreach (var person in room.Persons)
            {
                var hasNumberOfMales = person.NumberOfMales > 0;
                if (hasNumberOfMales)
                {
                    result.Add(
                        new AppDatePersonFlatOfBookingAuditLogResponse(
                            AppDateId,
                            RestIndex,
                            room.RoomIndex,
                            person.PersonAgeTypeName,
                            Genders.Male,
                            person.NumberOfMales,
                            person.NumberOfMales,
                            0,
                            0
                        )
                    );
                }

                var hasNumberOfFemales = person.NumberOfFemales > 0;
                if (hasNumberOfFemales)
                {
                    result.Add(
                        new AppDatePersonFlatOfBookingAuditLogResponse(
                            AppDateId,
                            RestIndex,
                            room.RoomIndex,
                            person.PersonAgeTypeName,
                            Genders.Female,
                            person.NumberOfFemales,
                            0,
                            person.NumberOfFemales,
                            0
                        )
                    );
                }

                if (hasNumberOfFemales || hasNumberOfMales)
                {
                    continue;
                }

                result.Add(
                    new AppDatePersonFlatOfBookingAuditLogResponse(
                        AppDateId,
                        RestIndex,
                        room.RoomIndex,
                        person.PersonAgeTypeName,
                        Genders.None,
                        person.NumberOfPersons,
                        0,
                        0,
                        person.NumberOfNonePersons
                    )
                );
            }
        }

        return result;
    }

    public IEnumerable<AppDateOptionFlatOfBookingAuditLogResponse> GetFlatOptions()
    {
        return Rooms.SelectMany(
            room => room.Options?.Select(
                    option => new AppDateOptionFlatOfBookingAuditLogResponse(
                        AppDateId,
                        RestIndex,
                        room.RoomIndex,
                        option.OptionName,
                        option.NumberOfOptions
                    )
                )
                ?? []
        );
    }

    public IEnumerable<AppDateRoomRepresentativeNameFlatOfBookingAuditLogResponse> GetFlatRoomNameRepresentatives()
    {
        var data = Rooms.Select(
                room => room.RoomRepresentatives != null
                    ? new AppDateRoomRepresentativeNameFlatOfBookingAuditLogResponse(
                        room.RoomIndex,
                        room.RoomRepresentatives.Name
                    )
                    : null
            )
            .Where(x => x != null);

        return data.Where(x => x != null)!;
    }

    public IEnumerable<AppDateRoomRepresentativeKanaFlatOfBookingAuditLogResponse> GetFlatRoomNameKanaRepresentatives()
    {
        var data = Rooms.Select(
                room => room.RoomRepresentatives != null
                    ? new AppDateRoomRepresentativeKanaFlatOfBookingAuditLogResponse(
                        room.RoomIndex,
                        room.RoomRepresentatives.Kana
                    )
                    : null
            )
            .Where(x => x != null);

        return data.Where(x => x != null)!;
    }
}

public record RoomOfBookingAuditLogResponse(
    int RoomIndex,
    IEnumerable<PersonOfBookingAuditLogResponse>? Persons,
    IEnumerable<OptionOfBookingAuditLogResponse>? Options,
    RoomRepresentativeOfBookingAuditLogResponse? RoomRepresentatives
);

public record PersonOfBookingAuditLogResponse(
    string? PersonAgeTypeName,
    int NumberOfPersons,
    int NumberOfMales,
    int NumberOfFemales,
    int NumberOfNonePersons
);

public record OptionOfBookingAuditLogResponse(
    string? OptionName,
    int NumberOfOptions
);

public record RoomRepresentativeOfBookingAuditLogResponse(
    string? Name,
    string? Kana
);
