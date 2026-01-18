using Liberty.Reservation.Application.Constants;

namespace Liberty.Reservation.Application.Exceptions;

public class ReservationInvalidException : ReservationException
{
    public override ErrorCode ErrorCode => ErrorCode.X00106;

    public override string Title => "予約情報エラー";

    public override string ApproachMessage => "";

    public ReservationInvalidException() : base("不正な予約情報です")
    {
    }

    public ReservationInvalidException(
        string message
    ) : base("不正な予約情報です")
    {
    }
}
