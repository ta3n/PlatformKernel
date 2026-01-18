using Liberty.Reservation.Application.Constants;

namespace Liberty.Reservation.Application.Exceptions;

public class ReservationUsedPointOverTotalPriceException : ReservationException
{
    public override ErrorCode ErrorCode => ErrorCode.X00102;
    public override string Title => "予約情報エラー";

    public override string ApproachMessage => "";

    public ReservationUsedPointOverTotalPriceException() : base("指定のポイント数は使用できません")
    {
    }

    public ReservationUsedPointOverTotalPriceException(
        int usedPoint,
        int totalPrice
    ) : base("指定のポイント数は使用できません")
    {
    }
}
