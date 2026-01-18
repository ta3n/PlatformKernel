using Liberty.Reservation.Application.Constants;

namespace Liberty.Reservation.Application.Exceptions;

public class ReservationNoMatchPersonsException() : ReservationInvalidException("予約情報の人数が一致しません")
{
    public override ErrorCode ErrorCode => ErrorCode.X00105;

    public override string Title => "予約情報エラー";

    public override string ApproachMessage => "予約情報の人数が一致しません";
}
