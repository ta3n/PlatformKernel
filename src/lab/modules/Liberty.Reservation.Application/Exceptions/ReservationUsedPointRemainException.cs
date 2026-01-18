using Liberty.Reservation.Application.Constants;

namespace Liberty.Reservation.Application.Exceptions;

public class ReservationUsedPointRemainException : ReservationException
{
    public override ErrorCode ErrorCode => ErrorCode.X00101;

    public override string Title => "予約情報エラー";

    public override string ApproachMessage => "";

    public ReservationUsedPointRemainException() : base("ポイント残数にエラーがあります")
    {
    }

    public ReservationUsedPointRemainException(
        int usedPoint,
        int remainPoint
    ) : base("ポイント残数にエラーがあります")
    {
    }
}
