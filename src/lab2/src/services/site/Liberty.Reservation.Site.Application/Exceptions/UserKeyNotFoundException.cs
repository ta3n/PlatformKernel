using Liberty.ApplicationShared.Extensions;
using Liberty.SysException.Exceptions;

namespace Liberty.Reservation.Site.Application.Exceptions;

public class UserKeyNotFoundException : AppNotfoundException
{
    public override ErrorCode ErrorCode => ErrorCode.E2052;
    public override string Title => ErrorCode.GetEnumDescriptions();
}
