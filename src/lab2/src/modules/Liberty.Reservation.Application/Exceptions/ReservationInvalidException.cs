using Liberty.ApplicationShared.Extensions;
using Liberty.SysException;

namespace Liberty.Reservation.Application.Exceptions;

public class ReservationInvalidException(
    string message
) : ReservationException
{
    public override ErrorCode ErrorCode => ErrorCode.E2032;

    public override string Title => string.Format(ErrorCode.GetEnumDescriptions(), message);

    public override string ApproachMessage => "";
}
