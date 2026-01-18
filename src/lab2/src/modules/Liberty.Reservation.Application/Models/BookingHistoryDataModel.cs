using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Metas;

namespace Liberty.Reservation.Application.Models;

public class BookingHistoryDataModel
{
    public long Id { get; set; }
    public string? Code { get; set; }
    public string? Serial { get; set; }
    public int? UpdatedCount { get; set; }
    public long? LastModifiedDateTime { get; set; }
    public string? LastModifiedBy { get; set; }
    public BasicOfOfBookingDataModel? Basic { get; set; }
    public CustomerOfBookingDataModel? Reserver { get; set; }
    public GuestOfBookingDataModel? MainUser { get; set; }
    public BookingData? BookingData { get; set; }
}

public class BasicOfOfBookingDataModel
{
    public TimeSpan? CheckInTime { get; set; }
    public int NumberOfNights { get; set; }
    public int NumberOfRooms { get; set; }
    public string? Memo { get; set; }
}

public class CustomerOfBookingDataModel
{
    public string? Name { get; set; }
    public string? Kana { get; set; }
    public string? EMail { get; set; }
    public string? CountryCode { get; set; }
    public string? PostCode { get; set; }
    public string? Address1 { get; set; }
    public string? Address2 { get; set; }
    public string? Address3 { get; set; }
    public string? Phone { get; set; }
    public Genders? Gender { get; set; }
}

public class GuestOfBookingDataModel
{
    public string? Name { get; set; }
    public string? Kana { get; set; }
    public string? CountryCode { get; set; }
    public string? PostCode { get; set; }
    public string? Address1 { get; set; }
    public string? Address2 { get; set; }
    public string? Address3 { get; set; }
    public string? Phone { get; set; }
    public Genders? Gender { get; set; }
}

public class BookingTreeModel
{
    public long Id { get; set; }
    public long? ParentId { get; set; }
}
