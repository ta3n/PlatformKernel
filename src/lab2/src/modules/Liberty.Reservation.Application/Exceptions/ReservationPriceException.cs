using Liberty.ApplicationShared.Extensions;
using Liberty.SysException;
using Liberty.SysException.Exceptions;

namespace Liberty.Reservation.Application.Exceptions;

public class ReservationPriceException(
    string message
) : AppInvalidException
{
    public override ErrorCode ErrorCode => ErrorCode.E2032;

    public override string Title => string.Format(ErrorCode.GetEnumDescriptions(), message);

    public override string ApproachMessage => "";
}
