using Liberty.ApplicationShared.Extensions;
using Liberty.SysException;

namespace Liberty.Reservation.Application.Exceptions;

public class ReservationUsedPointRemainException(
    int usedPoint,
    int remainPoint
) : ReservationException
{
    public override ErrorCode ErrorCode => ErrorCode.E2008;

    public override string Title => string.Format(
        ErrorCode.GetEnumDescriptions(),
        usedPoint,
        remainPoint
    );

    public override string ApproachMessage => "";
}
