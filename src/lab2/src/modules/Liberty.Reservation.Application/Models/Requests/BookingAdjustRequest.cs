using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Models.Responses;

namespace Liberty.Reservation.Application.Models.Requests;

public record BookingAdjustRequest(
    bool IsAgree,
    string CheckInTime,
    int NumberOfNights,
    int NumberOfRooms,
    string? FreeInput,
    ReserverOfReservationAdjustRequest Reserver,
    GuestOfReservationAdjustRequest? MainUser,
    IEnumerable<NightPeopleOfReservationAdjustRequest>? NightPeoples,
    IEnumerable<NightOptionOfReservationAdjustRequest>? NightOptions,
    IEnumerable<RoomRepresentativeOfReservationAdjustRequest>? RoomRepresentatives,
    IEnumerable<QuestionOfBookingCreateRequest>? PlanQuestions,
    IEnumerable<QuestionOfBookingCreateRequest>? OptionsQuestions
)
{
    public long Id { get; set; }
    public long CheckInDateId { get; set; }

    /// <summary>
    /// Calculates and retrieves the ID of the check-out date based on the check-in date and the number of nights.
    /// </summary>
    /// <remarks>
    /// This method uses the `CheckInDateId` and adds the `NumberOfNights` to determine the check-out date.
    /// The resulting date is then converted to its corresponding ID using the <see cref="AppDate.GetId"/> method.
    /// </remarks>
    /// <returns>
    /// The ID of the check-out date as a <see cref="long"/>.
    /// </returns>
    /// <example>
    /// <code>
    /// var checkOutDateId = request.CheckOutDateId();
    /// </code>
    /// </example>
    public long CheckOutDateId()
    {
        var checkOutDate = AppDate.GetId(
            AppDate.GetDateTime(CheckInDateId).AddDays(NumberOfNights)
        );

        return checkOutDate;
    }

    /// <summary>
    /// Calculates and retrieves the ID of the last night of the stay based on the check-in date and the number of nights.
    /// </summary>
    /// <remarks>
    /// This method uses the `CheckInDateId` and subtracts one from the `NumberOfNights` to determine the last night.
    /// The resulting date is then converted to its corresponding ID using the <see cref="AppDate.GetId"/> method.
    /// </remarks>
    /// <returns>
    /// The ID of the last night of the stay as a <see cref="long"/>.
    /// </returns>
    /// <example>
    /// <code>
    /// var dateEndNight = request.GetDateEndNight();
    /// </code>
    /// </example>
    public long GetDateEndNight()
    {
        var dateEndNight = AppDate.GetId(
            AppDate.GetDateTime(CheckInDateId).AddDays(NumberOfNights - 1)
        );

        return dateEndNight;
    }

    /// <summary>
    /// Converts the nested structure of night peoples into a flattened collection of <see cref="FlattenedNightPeople"/> objects.
    /// </summary>
    /// <remarks>
    /// This method processes the `NightPeoples` property, which contains a hierarchical structure of night peoples, rooms,
    /// and their associated details. It flattens this structure into a collection of <see cref="FlattenedNightPeople"/>
    /// objects, where each object represents a single person with their associated room and date information.
    /// </remarks>
    /// <returns>
    /// A collection of <see cref="FlattenedNightPeople"/> objects. If `NightPeoples` is null, the method returns null.
    /// </returns>
    /// <example>
    /// <code>
    /// var flattenedNightPeoples = request.NightPeoplesToTable();
    /// </code>
    /// </example>
    public IEnumerable<FlattenedNightPeople>? NightPeoplesToTable()
    {
        var appDateId = NightPeoples?.Select(x => x.AppDateId).OrderBy(x => x).ToList();
        var nightPeoplesToTable = NightPeoples?
            .SelectMany(
                nightPeople => nightPeople.Rooms,
                (
                    nightPeople,
                    room
                ) => new
                {
                    nightPeople.AppDateId,
                    room.RoomIndex,
                    room.Peoples
                }
            )
            .SelectMany(
                x => x.Peoples,
                (
                    x,
                    people
                ) => new FlattenedNightPeople(
                    x.AppDateId,
                    appDateId?.IndexOf(x.AppDateId) ?? 0,
                    x.RoomIndex,
                    people.PersonAgeTypeId,
                    people.NumberOfPeoples,
                    people.Gender
                )
            );

        return nightPeoplesToTable;
    }

    /// <summary>
    /// Gets the guests per room for search model, grouped by AppDateId, RestIndex, RoomGroupIndex, and PersonAgeTypeId.
    /// </summary>
    /// <returns>A collection of <see cref="PersonOfBookingSearchModel"/> representing guests per room.</returns>
    /// <example>
    /// <code>
    /// var guests = request.GetGuestsPerRoomForSearchModel();
    /// </code>
    /// </example>
    public IEnumerable<PersonOfBookingSearchModel> GetGuestsPerRoomForSearchModel()
    {
        if (NightPeoples is null)
        {
            return [];
        }

        var dict = new Dictionary<
            (long AppDateId, int RestIndex, int RoomGroupIndex, long PersonAgeTypeId),
            PersonOfBookingSearchModel
        >();
        var nightPeoplesList = NightPeoples.ToList();

        for (var nightIndex = 0; nightIndex < nightPeoplesList.Count; nightIndex++)
        {
            var nightPeople = nightPeoplesList[nightIndex];
            AddGuestsForNight(nightPeople, nightIndex, dict);
        }

        return dict.Values;

        static void AddGuestsForNight(
            NightPeopleOfReservationAdjustRequest nightPeople,
            int nightIndex,
            IDictionary<(long AppDateId, int RestIndex, int RoomGroupIndex, long PersonAgeTypeId), PersonOfBookingSearchModel> dict
        )
        {
            var roomsList = nightPeople.Rooms.ToList();
            for (var roomIndex = 0; roomIndex < roomsList.Count; roomIndex++)
            {
                var room = roomsList[roomIndex];
                AddGuestsForRoom(nightPeople.AppDateId, nightIndex, roomIndex, room, dict);
            }
        }

        static void AddGuestsForRoom(
            long appDateId,
            int nightIndex,
            int roomIndex,
            RoomNightOfReservationAdjustRequest room,
            IDictionary<(long AppDateId, int RestIndex, int RoomGroupIndex, long PersonAgeTypeId), PersonOfBookingSearchModel> dict
        )
        {
            foreach (var people in room.Peoples)
            {
                var malePersons = people.Gender is Genders.Male ? people.NumberOfPeoples : 0;
                var femalePersons = people.Gender is Genders.Female ? people.NumberOfPeoples : 0;
                var key = (appDateId, nightIndex, roomIndex, people.PersonAgeTypeId);

                if (!dict.TryGetValue(key, out var existing))
                {
                    dict[key] = new PersonOfBookingSearchModel
                    {
                        AppDateId = appDateId,
                        RestIndex = nightIndex,
                        RoomGroupIndex = roomIndex,
                        PersonAgeTypeId = people.PersonAgeTypeId,
                        MalePersons = malePersons,
                        FemalePersons = femalePersons,
                        Persons = malePersons + femalePersons
                    };
                }
                else
                {
                    dict[key] = existing with
                    {
                        MalePersons = existing.MalePersons + malePersons,
                        FemalePersons = existing.FemalePersons + femalePersons,
                        Persons = existing.Persons + malePersons + femalePersons
                    };
                }
            }
        }
    }

    /// <summary>
    /// Converts the nested structure of night options into a flattened collection of <see cref="FlattenedNightOptionItem"/> objects.
    /// </summary>
    /// <remarks>
    /// This method processes the `NightOptions` property, which contains a hierarchical structure of night options, rooms,
    /// and their associated details. It flattens this structure into a collection of <see cref="FlattenedNightOptionItem"/>
    /// objects, where each object represents a single option item with its associated room and date information.
    /// </remarks>
    /// <returns>
    /// A collection of <see cref="FlattenedNightOptionItem"/> objects. If `NightOptions` is null, the method returns null.
    /// </returns>
    /// <example>
    /// <code>
    /// var flattenedNightOptions = request.NightOptionItemsToTable();
    /// </code>
    /// </example>
    public IEnumerable<FlattenedNightOptionItem>? NightOptionItemsToTable()
    {
        var appDateId = NightOptions?.Select(x => x.AppDateId).OrderBy(x => x).ToList();
        var nightOptionsToTable = NightOptions?
            .SelectMany(
                nightOption => nightOption.Rooms,
                (
                    nightOption,
                    room
                ) => new
                {
                    nightOption.AppDateId,
                    room.RoomIndex,
                    room.OptionItems
                }
            )
            .SelectMany(
                x => x.OptionItems,
                (
                    x,
                    option
                ) => new FlattenedNightOptionItem(
                    x.AppDateId,
                    appDateId?.IndexOf(x.AppDateId) ?? 0,
                    x.RoomIndex,
                    option.OptionItemId,
                    option.Number
                )
            );

        return nightOptionsToTable;
    }

    /// <summary>
    /// Retrieves a collection of option items for the booking search model.
    /// </summary>
    /// <remarks>
    /// This method processes the `NightOptions` property to generate a collection of
    /// <see cref="OptionOfBookingSearchModel"/> objects. Each object represents an option item
    /// associated with a specific room and date.
    /// </remarks>
    /// <returns>
    /// A collection of <see cref="OptionOfBookingSearchModel"/> objects. If `NightOptions` is null,
    /// an empty collection is returned.
    /// </returns>
    /// <example>
    /// <code>
    /// var optionItems = request.GetOptionItemsForSearchModel();
    /// </code>
    /// </example>
    public IEnumerable<OptionOfBookingSearchModel> GetOptionItemsForSearchModel()
    {
        if (NightOptions is null)
        {
            return [];
        }

        return
        [
            .. NightOptionItemsToTable()!
                .Select(
                    x => new OptionOfBookingSearchModel
                    {
                        AppDateId = x.AppDateId,
                        RoomGroupIndex = x.RoomIndex,
                        OptionItemId = x.OptionItemId,
                        Number = x.Number
                    }
                )
        ];
    }

    /// <summary>
    /// Retrieves a distinct array of person age type IDs from the nested structure of night peoples.
    /// </summary>
    /// <remarks>
    /// This method processes the `NightPeoples` property, which contains a hierarchical structure of rooms and peoples.
    /// It extracts and returns a distinct collection of `PersonAgeTypeId` values from all peoples across all rooms.
    /// </remarks>
    /// <returns>
    /// An array of distinct `PersonAgeTypeId` values. If `NightPeoples` is null, an empty array is returned.
    /// </returns>
    /// <example>
    /// <code>
    /// var personAgeTypeIds = request.GetPersonAgeTypeIds();
    /// </code>
    /// </example>
    public long[] GetPersonAgeTypeIds()
    {
        if (NightPeoples is null)
        {
            return [];
        }

        return
        [
            .. NightPeoples
                .SelectMany(x => x.Rooms)
                .SelectMany(x => x.Peoples)
                .Select(x => x.PersonAgeTypeId)
                .Distinct()
        ];
    }

    /// <summary>
    /// Retrieves a distinct array of option item IDs from the nested structure of night options.
    /// </summary>
    /// <remarks>
    /// This method processes the `NightOptions` property, which contains a hierarchical structure of rooms and option items.
    /// It extracts and returns a distinct collection of `OptionItemId` values from all option items across all rooms.
    /// </remarks>
    /// <returns>
    /// An array of distinct `OptionItemId` values. If `NightOptions` is null, an empty array is returned.
    /// </returns>
    /// <example>
    /// <code>
    /// var optionItemIds = request.GetOptionItemIds();
    /// </code>
    /// </example>
    public long[] GetOptionItemIds()
    {
        if (NightOptions is null)
        {
            return [];
        }

        return
        [
            .. NightOptions
                .SelectMany(x => x.Rooms)
                .SelectMany(x => x.OptionItems)
                .Select(x => x.OptionItemId)
                .Distinct()
        ];
    }

    /// <summary>
    /// Retrieves the total number of people for a specific date ID.
    /// </summary>
    /// <remarks>
    /// This method processes the `NightPeoples` property to find the entry matching the specified `appDateId`.
    /// It then aggregates the `NumberOfPeoples` from all rooms associated with that date.
    /// </remarks>
    /// <param name="appDateId">The ID of the date for which the total number of people is to be calculated.</param>
    /// <returns>
    /// The total number of people as an <see cref="int"/>. If `NightPeoples` is null or the date ID is not found, returns null.
    /// </returns>
    /// <example>
    /// <code>
    /// var totalPeople = request.GetNumberOfPeoplesByDateId(12345);
    /// </code>
    /// </example>
    public int? GetNumberOfPeoplesByDateId(
        long appDateId
    )
    {
        return NightPeoples?
            .First(x => x.AppDateId == appDateId)
            .Rooms
            .SelectMany(room => room.Peoples)
            .Sum(people => people.NumberOfPeoples);
    }

    /// <summary>
    /// Retrieves the maximum number of adults for a specific date ID.
    /// </summary>
    /// <remarks>
    /// This method processes the `NightPeoples` property to find the entry matching the specified `appDateId`.
    /// It then calculates the maximum number of adults across all rooms for that date.
    /// </remarks>
    /// <param name="appDateId">The ID of the date for which the maximum number of adults is to be calculated.</param>
    /// <returns>
    /// The maximum number of adults as an <see cref="int"/>. If `NightPeoples` is null, returns null.
    /// </returns>
    /// <example>
    /// <code>
    /// var maxAdults = request.GetNumberOfAdultsByDateId(12345);
    /// </code>
    /// </example>
    public int? GetNumberOfAdultsByDateId(
        long appDateId
    )
    {
        return NightPeoples?
            .First(x => x.AppDateId == appDateId)
            .Rooms
            .Select(room => room.Peoples.Sum(people => people.NumberOfPeoples))
            .Max();
    }

    /// <summary>
    /// Retrieves the total number of adults for a specific date ID and room index, filtered by person age type IDs.
    /// </summary>
    /// <remarks>
    /// This method processes the `NightPeoples` property to find the entry matching the specified `appDateId`.
    /// It then filters the rooms by the specified `roomIndex` and aggregates the `NumberOfPeoples`
    /// for the given `personAgeTypeIds`.
    /// </remarks>
    /// <param name="appDateId">The ID of the date for which the total number of adults is to be calculated.</param>
    /// <param name="roomIndex">The index of the room to filter by.</param>
    /// <param name="personAgeTypeIds">A collection of person age type IDs to filter the peoples.</param>
    /// <returns>
    /// The total number of adults as an <see cref="int"/>. If `NightPeoples` is null, returns null.
    /// </returns>
    /// <example>
    /// <code>
    /// var adultsByRoom = request.GetNumberOfAdultsByDateIdAndRoomIndex(12345, 1, new[] { 1L, 2L });
    /// </code>
    /// </example>
    public int? GetNumberOfAdultsByDateIdAndRoomIndex(
        long appDateId,
        int roomIndex,
        IEnumerable<long> personAgeTypeIds
    )
    {
        return NightPeoples?
            .First(x => x.AppDateId == appDateId)
            .Rooms
            .Where(x => x.RoomIndex == roomIndex)
            .SelectMany(room => room.Peoples)
            .Where(people => personAgeTypeIds.Contains(people.PersonAgeTypeId))
            .Sum(people => people.NumberOfPeoples);
    }

    /// <summary>
    /// Retrieves the maximum number of people for a specific date ID, filtered by person age type IDs.
    /// </summary>
    /// <remarks>
    /// This method processes the `NightPeoples` property to find the entry matching the specified `appDateId`.
    /// It then calculates the maximum number of people across all rooms for the given `personAgeTypeIds`.
    /// </remarks>
    /// <param name="appDateId">The ID of the date for which the maximum number of people is to be calculated.</param>
    /// <param name="personAgeTypeIds">A collection of person age type IDs to filter the peoples.</param>
    /// <returns>
    /// The maximum number of people as an <see cref="int"/>. If `NightPeoples` is null, returns 0.
    /// </returns>
    /// <example>
    /// <code>
    /// var maxPeople = request.GetMaxNumberOfPeoplesByDateId(12345, new[] { 1L, 2L });
    /// </code>
    /// </example>
    public int? GetMaxNumberOfPeoplesByDateId(
        long appDateId,
        IEnumerable<long> personAgeTypeIds
    )
    {
        var roomsByAppDateId = NightPeoples?
            .First(nightPeople => nightPeople.AppDateId == appDateId)
            .Rooms
            .ToList();

        if (roomsByAppDateId is null)
        {
            return 0;
        }

        var temp = new int[roomsByAppDateId.Count];

        for (var i = 0; i < roomsByAppDateId.Count; i++)
        {
            temp[i] = roomsByAppDateId[i]
                .Peoples
                .Where(x => personAgeTypeIds.Contains(x.PersonAgeTypeId))
                .Sum(
                    people => people.NumberOfPeoples
                );
        }

        return temp.Max();
    }

    public CustomerOfBookingDataModel ConvertToCustomerOfBookingData()
    {
        return new CustomerOfBookingDataModel
        {
            Name = Reserver.FullName,
            Address1 = Reserver.Address1,
            Address2 = Reserver.Address2,
            Address3 = Reserver.Address3,
            CountryCode = Reserver.CountryCode,
            EMail = Reserver.Email,
            Gender = Reserver.Gender,
            Kana = Reserver.Kana,
            Phone = Reserver.PhoneNumber,
            PostCode = Reserver.PostCode
        };
    }

    public GuestOfBookingDataModel? ConvertToGuestOfBookingData()
    {
        if (MainUser is null)
        {
            return null;
        }

        return new GuestOfBookingDataModel
        {
            Name = MainUser.FullName,
            Address1 = MainUser.Address1,
            Address2 = MainUser.Address2,
            Address3 = MainUser.Address3,
            CountryCode = MainUser.CountryCode,
            Gender = MainUser.Gender,
            Kana = MainUser.Kana,
            Phone = MainUser.PhoneNumber,
            PostCode = MainUser.PostCode
        };
    }
}

public record ReserverOfReservationAdjustRequest(
    string FullName,
    string? Kana,
    Genders? Gender,
    string Email,
    string PostCode,
    string? CountryCode,
    string Address1,
    string Address2,
    string? Address3,
    string PhoneNumber
);

public record GuestOfReservationAdjustRequest(
    string? FullName,
    string? Kana,
    Genders? Gender,
    long? Birthday,
    string? PostCode,
    string? CountryCode,
    string? Address1,
    string? Address2,
    string? Address3,
    string? PhoneNumber
);

public record NightPeopleOfReservationAdjustRequest(
    long AppDateId,
    IEnumerable<RoomNightOfReservationAdjustRequest> Rooms
);

public record RoomNightOfReservationAdjustRequest(
    int RoomIndex,
    IEnumerable<PeopleOfReservationAdjustRequest> Peoples
);

public record PeopleOfReservationAdjustRequest(
    long PersonAgeTypeId,
    int NumberOfPeoples,
    Genders Gender
);

public record NightOptionOfReservationAdjustRequest(
    long AppDateId,
    IEnumerable<RoomOptionOfReservationAdjustRequest> Rooms
);

public record RoomOptionOfReservationAdjustRequest(
    int RoomIndex,
    IEnumerable<OptionOfReservationAdjustRequest> OptionItems
);

public record OptionOfReservationAdjustRequest(
    long OptionItemId,
    int Number
);

public record RoomRepresentativeOfReservationAdjustRequest(
    int RoomIndex,
    string FullName,
    string? Kana
);

public record FlattenedNightPeople(
    long AppDateId,
    int RestIndex,
    int RoomIndex,
    long PersonAgeTypeId,
    int NumberOfPeoples,
    Genders Gender
);

public record FlattenedNightOptionItem(
    long AppDateId,
    int RestIndex,
    int RoomIndex,
    long OptionItemId,
    int Number
);
