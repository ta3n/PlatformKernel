using Liberty.Reservation.Application.Constants;

namespace Liberty.Reservation.Application.Exceptions;

public class ReservationQuestionNotFoundException : ReservationException
{
    public override ErrorCode ErrorCode => ErrorCode.X01024;

    public override string Title => "予約情報エラー";

    public override string ApproachMessage => "";

    public override bool IsNotFound => true;

    public ReservationQuestionNotFoundException() : base("予約質問が見つかりません")
    {
    }

    public ReservationQuestionNotFoundException(
        long reservationId,
        long questionId
    ) : base($"予約質問が見つかりません at reservationID:{reservationId},  questionID:{questionId}")
    {
    }
}
