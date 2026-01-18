using Liberty.Reservation.Application.Constants;

namespace Liberty.Reservation.Application.Exceptions;

public class ReservationUserInfoNotFoundException : ReservationException
{
    public override ErrorCode ErrorCode => ErrorCode.X01025;

    public override string Title => "予約情報エラー";

    public override string ApproachMessage => "";

    public override bool IsNotFound => true;

    public ReservationUserInfoNotFoundException() : base("予約者情報が見つかりません")
    {
    }

    public ReservationUserInfoNotFoundException(
        long reservationId,
        long appDateId,
        long roomGroupIndex
    ) : base($"予約者情報が見つかりません at reservationID:{reservationId},  appDateID:{appDateId}, roomGroupIndex:{roomGroupIndex}")
    {
    }
}
