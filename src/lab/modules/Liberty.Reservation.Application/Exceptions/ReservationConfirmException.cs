using Liberty.Reservation.Application.Constants;

namespace Liberty.Reservation.Application.Exceptions;

public class ReservationConfirmException : ReservationException
{
    public override ErrorCode ErrorCode => ErrorCode.X00107;

    public override string Title => "予約情報エラー";

    public override string ApproachMessage => "";

    public ReservationConfirmException() : base("予約認証にエラーがあります")
    {
    }

    public ReservationConfirmException(
        ReservationStatus reservationState
    ) : base("予約認証にエラーがあります")
    {
    }
}
