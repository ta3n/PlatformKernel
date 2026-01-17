namespace Blueprint.Service.Base.Application.Constants;

public static class ReservationStatusFilter
{
    public static readonly ReservationStatus[] AllowedStatuses =
    [
        ReservationStatus.Confirmed,
        ReservationStatus.Reserved,
        ReservationStatus.UserCanceled,
        ReservationStatus.GuestCanceled,
        ReservationStatus.ManagerCanceled,
        ReservationStatus.Modified
    ];
}
