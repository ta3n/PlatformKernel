using Liberty.ApplicationShared.Extensions;
using Liberty.SysException;

namespace Liberty.Reservation.Application.Exceptions;

public class ReservationOverNumberOfStayLimitException(
    int numberOfNights
) : ReservationServiceException
{
    public override ErrorCode ErrorCode => ErrorCode.E2018;

    public override string Title => ErrorCode.GetEnumDescriptions()
        .Replace("{numberOfNights}", numberOfNights.ToString());
}
