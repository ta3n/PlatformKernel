using Liberty.Reservation.Application.Constants;

namespace Liberty.Reservation.Application.Exceptions;

public class ReservationHasNoAdultPersonAgeTypesException() : ReservationInvalidException("予約情報に「大人」区分がありません")
{
    public override ErrorCode ErrorCode => ErrorCode.X00104;

    public override string Title => "予約情報エラー";

    public override string ApproachMessage => "予約情報に「大人」区分がありません";
}
