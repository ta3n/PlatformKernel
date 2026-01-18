using Liberty.ApplicationShared.Extensions;
using Liberty.SysException;
using Liberty.SysException.Exceptions;

namespace Liberty.Reservation.Application.Exceptions;

public class ReservationNightException : AppInvalidException
{
    public override ErrorCode ErrorCode => ErrorCode.E2076;
    public override string Title => ErrorCode.GetEnumDescriptions();
}
