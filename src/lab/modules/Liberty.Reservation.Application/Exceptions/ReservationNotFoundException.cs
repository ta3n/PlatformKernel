using Liberty.Reservation.Application.Constants;

namespace Liberty.Reservation.Application.Exceptions;

public class ReservationNotFoundException : ReservationException
{
    public override ErrorCode ErrorCode => ErrorCode.X00109;

    public override string Title => "予約情報エラー";

    public override string ApproachMessage => "";

    public override bool IsNotFound => true;

    public ReservationNotFoundException() : base("予約情報が見つかりません")
    {
    }

    public ReservationNotFoundException(
        long reservationId
    ) : base($"予約情報が見つかりません at reservationID:{reservationId}")
    {
    }

    public ReservationNotFoundException(
        long reservationId,
        string code
    ) : base($"予約情報が見つかりません at reservationID:{reservationId},  code:{code}")
    {
    }
}
