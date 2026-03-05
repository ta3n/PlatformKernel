using Liberty.ApplicationShared.Extensions;
using Liberty.SysException.Exceptions;

namespace Liberty.Reservation.Site.Application.Exceptions;

public class SystemConfigNotfoundException : AppNotfoundException
{
    public override ErrorCode ErrorCode => ErrorCode.E1016;
    public override string Title => ErrorCode.GetEnumDescriptions();
}
