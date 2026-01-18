using Liberty.Reservation.Application.Constants;

namespace Liberty.Reservation.Application.Exceptions;

/// <summary>在庫数を超えていた場合の例外</summary>
public class ReservationOperationNumberOverRemainException : ReservationInvalidException
{
    public override ErrorCode ErrorCode => ErrorCode.X00103;

    public override string Title => "予約情報エラー";

    public override string ApproachMessage => "";

    public ReservationOperationNumberOverRemainException() : base("在庫数を超えています")
    {
    }

    public ReservationOperationNumberOverRemainException(
        int? number,
        int optionNumber
    ) : base("在庫数を超えています")
    {
    }
}
