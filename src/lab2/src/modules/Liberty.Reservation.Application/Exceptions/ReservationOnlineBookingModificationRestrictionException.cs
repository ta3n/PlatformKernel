using Liberty.ApplicationShared.Extensions;
using Liberty.SysException;
using Liberty.SysException.Exceptions;

namespace Liberty.Reservation.Application.Exceptions;

public class ReservationOnlineBookingModificationRestrictionException : AppInvalidException
{
    public override ErrorCode ErrorCode => ErrorCode.E1063;
    public override string Title => ErrorCode.GetEnumDescriptions();
}
