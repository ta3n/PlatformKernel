using Liberty.ApplicationShared.Extensions;
using Liberty.SysException;
using Liberty.SysException.Exceptions;

namespace Liberty.Reservation.Application.Exceptions;

public abstract class ReservationException : ModelException
{
    public override ErrorCode ErrorCode => ErrorCode.E2003;

    public override string Title => ErrorCode.GetEnumDescriptions();

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
