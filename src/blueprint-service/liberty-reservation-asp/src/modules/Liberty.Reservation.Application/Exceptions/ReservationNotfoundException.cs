using Liberty.ApplicationShared.Extensions;
using Liberty.SysException;
using Liberty.SysException.Exceptions;

namespace Liberty.Reservation.Application.Exceptions;

public class ReservationNotfoundException : AppNotfoundException
{
    private long ReservationId { get; set; }

    public override ErrorCode ErrorCode => ErrorCode.E2002;

    public override string Title => ErrorCode.GetEnumDescriptions()
        .Replace("{reservationId}", ReservationId.ToString());

    public override string ApproachMessage => "";

    public override bool IsNotFound => true;

    public ReservationNotfoundException()
    {
    }

    public ReservationNotfoundException(
        long reservationId
    )
    {
        ReservationId = reservationId;
    }
}
