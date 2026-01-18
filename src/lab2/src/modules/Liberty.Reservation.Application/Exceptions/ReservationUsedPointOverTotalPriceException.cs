using Liberty.ApplicationShared.Extensions;
using Liberty.SysException;

namespace Liberty.Reservation.Application.Exceptions;

public class ReservationUsedPointOverTotalPriceException(
    int usedPoint,
    int totalPrice
) : ReservationException
{
    public override ErrorCode ErrorCode => ErrorCode.E2009;

    public override string Title => string.Format(
        ErrorCode.GetEnumDescriptions(),
        usedPoint,
        totalPrice
    );

    public override string ApproachMessage => "";
}
