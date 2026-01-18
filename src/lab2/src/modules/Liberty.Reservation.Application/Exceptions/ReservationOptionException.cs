using Liberty.ApplicationShared.Extensions;
using Liberty.SysException;
using Liberty.SysException.Exceptions;

namespace Liberty.Reservation.Application.Exceptions;

public class ReservationOptionException : AppInvalidException
{
    public override ErrorCode ErrorCode => ErrorCode.E2074;

    public override string Title => ErrorCode.GetEnumDescriptions();
}
