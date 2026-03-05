using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Utils;

namespace Liberty.Reservation.Application.Models;

public record BookingAuditChangeLog(
    List<ChangeLog<AppDatePersonFlatOfBookingAuditLogResponse>> PersonChanges,
    List<ChangeLog<AppDateOptionFlatOfBookingAuditLogResponse>> OptionChanges,
    List<ChangeLog<AppDateRoomRepresentativeNameFlatOfBookingAuditLogResponse>> RoomRepresentativeNameChanges,
    List<ChangeLog<AppDateRoomRepresentativeKanaFlatOfBookingAuditLogResponse>> RoomRepresentativeKanaChanges
);

public record AppDatePersonFlatOfBookingAuditLogResponse(
    long AppDateId,
    int RestIndex,
    int RoomIndex,
    string? PersonAgeTypeName,
    Genders Gender,
    int NumberOfPersons,
    int NumberOfMales,
    int NumberOfFemales,
    int NumberOfChildren
);

public record AppDateOptionFlatOfBookingAuditLogResponse(
    long AppDateId,
    int RestIndex,
    int RoomIndex,
    string? OptionName,
    int NumberOfOptions
);

public record AppDateRoomRepresentativeNameFlatOfBookingAuditLogResponse(
    int RoomIndex,
    string? Name
);

public record AppDateRoomRepresentativeKanaFlatOfBookingAuditLogResponse(
    int RoomIndex,
    string? Kana
);
