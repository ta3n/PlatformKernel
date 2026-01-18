using Liberty.ApplicationShared.Extensions;
using Liberty.SysException;
using Liberty.SysException.Exceptions;

namespace Liberty.Reservation.User.Application.Exceptions;

public class SystemConfigNotfoundException : AppNotfoundException
{
    public override ErrorCode ErrorCode => ErrorCode.E1016;
    public override string Title => ErrorCode.GetEnumDescriptions();
}
