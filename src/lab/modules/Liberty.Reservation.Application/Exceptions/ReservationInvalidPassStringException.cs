using Liberty.Reservation.Application.Constants;

namespace Liberty.Reservation.Application.Exceptions;

public class ReservationInvalidPassStringException : ReservationException
{
    public override ErrorCode ErrorCode => ErrorCode.X00108;

    public override string Title => "予約情報エラー";

    public override string ApproachMessage => "";

    public ReservationInvalidPassStringException() : base("パスワード文字列にエラーがあります")
    {
    }

    public ReservationInvalidPassStringException(
        string passString
    ) : base($"パスワード文字列にエラーがあります")
    {
    }

    public ReservationInvalidPassStringException(
        string except1,
        string except2
    ) : base("")
    {
    }
}
