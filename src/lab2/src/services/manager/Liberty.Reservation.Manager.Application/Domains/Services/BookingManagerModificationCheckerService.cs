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
        ReservationStatus reservationState,
        TimeSpan? timeZone
    )
    {
        var checkOutDateTime = AppDate.GetDateTime(checkOutDate);

        var offset = timeZone ?? DefaultValues.DefaultTimeZoneOffset;

        var dateNow = DateTime.UtcNow
            .Add(offset)
            .Date;
        var modifyLimit = checkOutDateTime
            .AddDays(_managerModifyOptions.AllowModifyDaysAfterCheckOut)
            .Date;

        var validDateModify = dateNow <= modifyLimit;
        var validNoShow = !isNoShow;

        return validDateModify && validNoShow;
    }

    public bool CanBookingChangeModify(
        long checkOutDate,
        bool isNoShow,
        ReservationStatus reservationState,
        TimeSpan? timeZone
    )
    {
        var checkOutDateTime = AppDate.GetDateTime(checkOutDate);

        var offset = timeZone ?? DefaultValues.DefaultTimeZoneOffset;

        var dateNow = DateTime.UtcNow
            .Add(offset)
            .Date;
        var modifyLimit = checkOutDateTime.AddDays(_managerModifyOptions.AllowModifyDaysAfterCheckOut).Date;

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
        ReservationStatus reservationState,
        TimeSpan? timeZone
    )
    {
        var offset = timeZone ?? DefaultValues.DefaultTimeZoneOffset;

        var checkOutDateTime = AppDate.GetDateTime(checkOutDate);

        var checkTime = checkInTime.TotalDays < 1
            ? checkInTime
            : new TimeSpan(23, 59, 59);
        var checkInDateTime = AppDate
            .GetDateTime(checkInDate)
            .Add(checkTime);

        var dateNow = DateTime.UtcNow
            .Add(offset);

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
