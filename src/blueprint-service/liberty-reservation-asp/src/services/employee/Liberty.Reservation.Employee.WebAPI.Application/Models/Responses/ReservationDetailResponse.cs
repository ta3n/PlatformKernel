namespace Liberty.Reservation.Employee.WebAPI.Application.Models.Responses;

public record ReservationDetailResponse
{
    public long Id { get; init; }
    public string? Code { get; init; }
    public ReservationStatus State { get; init; }
    public string? FacilityName { get; init; }
    public string? PlanName { get; init; }
    public string? SiteName { get; init; }
    public TimeSpan? CheckInStart { get; init; }
    public TimeSpan? CheckInEnd { get; init; }
    public string? LanguageCode { get; init; }
    public string? RoomGroupName { get; init; }
    public long CheckInDate { get; init; }

    public long CheckOutDate
    {
        get
        {
            var checkOutDate = AppDate.GetDateTime(CheckInDate).AddDays(NumberOfNights);
            return AppDate.GetId(checkOutDate);
        }
    }

    public TimeSpan? CheckInTime { get; init; }
    public int NumberOfNights { get; init; }
    public int NumberOfRooms { get; init; }
    public PaymentTypes PaymentType { get; init; }
    public decimal AccommodationFee { get; init; }
    public decimal OptionalFee { get; init; }
    public decimal TaxFee { get; init; }
    public decimal TotalFee { get; init; }
    public string? FreeInput { get; init; }
    public bool IsSameMainUser { get; init; }
    public ImportantNotesOfReservationDetailResponse? ImportantNotes { get; init; }
    public CancellationOfReservationResponse? Cancellation { get; init; }
    public IEnumerable<ReservationPersonDataResponse>? NumberOfPeople { get; set; }
    public ReserverResponse? Reserver { get; init; }
    public GuestResponse? Customer { get; init; }
    public IEnumerable<ReservationQuestionResponse>? PlanQuestions { get; init; }
    public IEnumerable<ReservationQuestionResponse>? OptionQuestions { get; init; }
    public IEnumerable<BookingAppDateResponse>? AppDates { get; set; }
    public IEnumerable<PersonAgeTypesResponse>? PersonAgeTypes { get; set; }
    public IEnumerable<RoomRepresentativeResponse>? RoomRepresentatives { get; set; }
    public decimal CancellationPrice { get; set; }
    public bool? IsBarrierFree { get; set; }
    public string? BarrierFreeInfoComment { get; set; }
    public bool? UseSpaTax { get; set; }
    public string? SpaTaxComment { get; set; }
    public string? SpaTaxTable { get; set; }
    public PlanTypes PlanType { get; set; }
    public int? CapacityMax { get; set; }
    public bool IsNoShow { get; init; }
    public DateTime? NoShowDateTime { get; init; }
    public string? NoShowReason { get; init; }
    public bool DayUse { get; set; }
    public DateTime? UpdatedAt { get; init; }
    public int UpdateCount { get; init; }
}

public record ReservationPersonDataResponse(
    long AppDateId,
    IEnumerable<RoomDataOfReservationResponse> Rooms
);

public record RoomDataOfReservationResponse(
    int RoomIndex,
    IEnumerable<PeoplePerRoomDataOfReservationResponse> People
);

public record PeoplePerRoomDataOfReservationResponse(
    string? PersonAgeType,
    int NumberOfPeople,
    int NumberOfMales,
    int NumberOfFemales,
    int NumberOfChildren
);

public record ImportantNotesOfReservationDetailResponse(
    string? PaymentInfo,
    string? MealInfo,
    string? OtherInfo
);

public record ReserverResponse(
    string? FullName,
    string? Kana,
    string? Email,
    string? Address1,
    string? Address2,
    string? Address3,
    string? CountryCode,
    string? PostCode,
    string? Phone,
    Genders Gender
);

public record GuestResponse(
    string? FullName,
    string? Kana,
    string? Address1,
    string? Address2,
    string? Address3,
    string? CountryCode,
    string? PostCode,
    long? BirthDay,
    Genders Gender,
    string? Phone
);

public record ReservationQuestionResponse(
    long? Id,
    string? Name,
    string? Description,
    string? FormData,
    string? AnswerData
);

public record CancellationOfReservationResponse(
    string? Name,
    string? Description,
    IEnumerable<CancellationDataOfReservationResponse?> CancellationData
);

public record CancellationDataOfReservationResponse(
    int? DayStart,
    int? DayEnd,
    float? Rate
);

public record BasicFeeOfReservationResponse(
    long AppDateId,
    IEnumerable<RoomsFeeOfReservationResponse> RoomsFee,
    decimal TotalPrice
);

public record RoomsFeeOfReservationResponse(
    int RoomIndex,
    IEnumerable<PeopleFeePerRoomOfReservationResponse> PeopleFee
);

public record PeopleFeePerRoomOfReservationResponse(
    decimal UnitPrice,
    int NumberOfPeople,
    decimal TotalPrice
);

public record BookingAppDateResponse(
    long AppDateId,
    int RestIndex,
    decimal Price,
    decimal SpaTax,
    decimal OptionPrice,
    decimal TotalPrice,
    IEnumerable<BookingRoomOfAppDate> Rooms
);

public record BookingRoomOfAppDate(
    int RoomIndex,
    decimal RoomPrice,
    IEnumerable<PeoplePriceOfRoomResponse> PricePeoples,
    IEnumerable<OptionItemOfRoomResponse>? OptionItems,
    decimal SpaTax,
    decimal TotalOptionPrice,
    int Persons
);

public record PeoplePriceOfRoomResponse(
    decimal RoomPrice,
    decimal SpaTax,
    int Persons,
    long? PersonAgeTypeId,
    string? PersonAgeTypeName,
    bool? IsMain,
    int? MalePersons,
    int? FemalePersons,
    int? NonePersons,
    int? OtherPersons,
    decimal TotalPrice
);

public record OptionItemOfRoomResponse(
    long Id,
    string? Name,
    decimal? Price,
    int Number
)
{
    public decimal TotalPrice => (Price ?? 0) * Number;
}

public record RoomRepresentativeResponse(
    long RoomIndex,
    string? Name,
    string? Kana
);

public record PersonAgeTypesResponse(
    long Id,
    string? Name,
    bool IsMain,
    int? AgeMax,
    int? AgeMin
);
