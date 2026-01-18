namespace Liberty.Reservation.Site.Public.WebAPI.Models.Responses;

public record BookingFacilityResponse(
    long Id,
    string Code,
    string? Name,
    string? Description,
    bool IsOnLinePayment,
    bool IsOnSidePayment,
    bool UseDailyPerson,
    bool CanAddRoomOnModify,
    string? Postcode,
    string? Url
)
{
    public FileOfBookingResponse? File { get; init; }
    public SiteOfBookingFacilityResponse? Site { get; set; }
    public List<PersonAgeTypeOfBookingFacilityResponse>? PersonAgeTypes { get; init; }
    public string? Heading { get; init; }
    public bool IsBarrierFree { get; init; }
    public string? BarrierFreeInfoComment { get; init; }
    public string? Email { get; init; }
    public bool? UseSpaTax { get; init; }
    public string? SpaTaxComment { get; init; }
    public string? SpaTaxTable { get; init; }
    public string? Logo { get; init; }
    public string? Address1 { get; init; }
    public string? Address2 { get; init; }
    public string? Address3 { get; init; }
    public string? Address4 { get; init; }
}

public record SiteOfBookingFacilityResponse(
    string? Code,
    string? Name
)
{
    public int MaxPeople { get; set; }
};

public record PersonAgeTypeOfBookingFacilityResponse(
    long Id,
    bool IsMain,
    int? AgeMin,
    int? AgeMax,
    string? Name,
    long? LastUpdatedAt
);
