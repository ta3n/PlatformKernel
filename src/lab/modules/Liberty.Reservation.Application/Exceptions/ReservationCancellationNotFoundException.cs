using Liberty.Reservation.Application.Constants;

namespace Liberty.Reservation.Application.Exceptions;

public class ReservationCancellationNotFoundException() : ReservationException("予約情報のキャンセル規定が見つかりません")
{
    public override ErrorCode ErrorCode => ErrorCode.X00099;

    public override string Title => "予約情報エラー";

    public override string ApproachMessage => "";

    public override bool IsNotFound => true;
}
