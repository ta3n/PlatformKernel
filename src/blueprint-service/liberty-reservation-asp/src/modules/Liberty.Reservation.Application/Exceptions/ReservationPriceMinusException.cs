using Liberty.ApplicationShared.Extensions;
using Liberty.SysException;

namespace Liberty.Reservation.Application.Exceptions;

public class ReservationPriceMinusException : ReservationServiceException
{
    public override ErrorCode ErrorCode => ErrorCode.E2012;
    public override string Title => ErrorCode.GetEnumDescriptions();
}
