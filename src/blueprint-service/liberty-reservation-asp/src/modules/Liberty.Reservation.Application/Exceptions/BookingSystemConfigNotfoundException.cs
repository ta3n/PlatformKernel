using Liberty.ApplicationShared.Extensions;
using Liberty.SysException;
using Liberty.SysException.Exceptions;

namespace Liberty.Reservation.Application.Exceptions;

public class BookingSystemConfigNotfoundException : AppNotfoundException
{
    public override ErrorCode ErrorCode => ErrorCode.E1016;
    public override string Title => ErrorCode.GetEnumDescriptions();
}
