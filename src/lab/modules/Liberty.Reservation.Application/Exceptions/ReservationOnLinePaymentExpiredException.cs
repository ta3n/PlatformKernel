using Liberty.Reservation.Application.Constants;

namespace Liberty.Reservation.Application.Exceptions;

public class ReservationOnLinePaymentExpiredException : ReservationException
{
    public override ErrorCode ErrorCode => ErrorCode.X00100;

    public override string Title => "予約情報エラー";

    public override string ApproachMessage => "";

    public ReservationOnLinePaymentExpiredException() : base("オンライン決済が可能な期間を超えています")
    {
    }

    public ReservationOnLinePaymentExpiredException(
        long reservationId
    ) : base($"オンライン決済が可能な期間を超えています")
    {
    }
}
