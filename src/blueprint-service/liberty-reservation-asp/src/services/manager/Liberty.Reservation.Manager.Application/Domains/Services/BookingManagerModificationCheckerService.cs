using Liberty.Reservation.Manager.Application.Options;
using Microsoft.Extensions.Options;

namespace Liberty.Reservation.Manager.Application.Domains.Services;

public class BookingManagerModificationCheckerService(
    IOptions<ManagerModifyOptions> managerModifySetting
) : IBookingManagerModificationCheckerService
{
    private readonly ManagerModifyOptions _managerModifyOptions = managerModifySetting.Value;

    public bool CanCancelModify(
        long checkOutDate,
        bool isNoShow,
        ReservationStatus reservationState
    )
    {
        var checkOutDateTime = AppDate.GetDateTime(checkOutDate);
        var dateNow = DateTime.UtcNow
            .AddHours(DefaultValues.TimeZoneOffset)
            .Date;
        var modifyLimit = checkOutDateTime
            .AddDays(_managerModifyOptions.AllowModifyDaysAfterCheckOut)
            .Date;

        var validDateModify = dateNow <= modifyLimit;
        var validNoShow = !isNoShow;

        return validDateModify && validNoShow;
    }

    public bool CanBookingChangeModify(
        long checkInDate,
        bool isNoShow,
        ReservationStatus reservationState
    )
    {
        var checkInDateTime = AppDate.GetDateTime(checkInDate);
        var dateNow = DateTime.UtcNow
            .AddHours(DefaultValues.TimeZoneOffset)
            .Date;
        var modifyLimit = checkInDateTime.Date;

        var validDateModify = dateNow <= modifyLimit;
        var validReservationStatus = reservationState is ReservationStatus.Reserved
            or ReservationStatus.Confirmed
            or ReservationStatus.Modified;
        var validNoShow = !isNoShow;

        return validDateModify && validReservationStatus && validNoShow;
    }

    public bool CanNoShowModify(
        TimeSpan checkInTime,
        long checkInDate,
        long checkOutDate,
        bool isNoShow,
        ReservationStatus reservationState
    )
    {
        var checkOutDateTime = AppDate.GetDateTime(checkOutDate);

        var checkTime = checkInTime.TotalDays < 1
            ? checkInTime
            : new TimeSpan(23, 59, 59);
        var checkInDateTime = AppDate
            .GetDateTime(checkInDate)
            .Add(checkTime);

        var dateNow = DateTime.UtcNow
            .AddHours(DefaultValues.TimeZoneOffset);

        var modifyLimit = checkOutDateTime
            .AddDays(_managerModifyOptions.AllowModifyDaysAfterCheckOut)
            .Date;

        var validDateModify = checkInDateTime < dateNow && dateNow.Date <= modifyLimit;
        var validReservationStatus = reservationState is ReservationStatus.Reserved
            or ReservationStatus.Confirmed
            or ReservationStatus.Modified;
        var validNoShow = !isNoShow;

        return validDateModify && validReservationStatus && validNoShow;
    }
}
