using Liberty.ApplicationShared.Extensions;
using Liberty.SysException;
using Liberty.SysException.Exceptions;

namespace Liberty.Reservation.Application.Exceptions;

public class ReservationNoAllowNoShowException : AppInvalidException
{
    public override ErrorCode ErrorCode => ErrorCode.E2064;
    public override string Title => ErrorCode.GetEnumDescriptions();
}
