using Liberty.ApplicationShared.Extensions;
using Liberty.SysException;
using Liberty.SysException.Exceptions;

namespace Liberty.Reservation.Application.Exceptions;

public class PermissionNotfoundException : AppNotfoundException
{
    public override ErrorCode ErrorCode => ErrorCode.E0100;
    public override string Title => ErrorCode.GetEnumDescriptions();
}
