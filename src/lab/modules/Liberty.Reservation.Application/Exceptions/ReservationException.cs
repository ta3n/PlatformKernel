using Liberty.Reservation.Application.Constants;

namespace Liberty.Reservation.Application.Exceptions;

public abstract class ReservationException : ModelException
{
    public override ErrorCode ErrorCode => ErrorCode.X00110;

    public override string Title => "予約情報エラー";

    public override string ApproachMessage => "";

    protected ReservationException() : base("予約情報にエラーがあります")
    {
    }

    protected ReservationException(
        string message
    ) : base(message)
    {
    }

    protected ReservationException(
        string message,
        Exception ex
    ) : base(message, ex)
    {
    }
}
