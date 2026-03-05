using Liberty.ApplicationShared.Extensions;
using Liberty.SysException.Exceptions;

namespace Liberty.Reservation.Site.Application.Exceptions;

public class LastUpdateStringInvalidException : AppInvalidException
{
    public override ErrorCode ErrorCode => ErrorCode.E2058;
    public override string Title => ErrorCode.GetEnumDescriptions();
}
