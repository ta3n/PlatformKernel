using Liberty.ApplicationShared.Extensions;
using Liberty.SysException.Exceptions;

namespace Liberty.Reservation.Site.Application.Exceptions;

public class SiteHasChangesException : AppInvalidException
{
    public override ErrorCode ErrorCode => ErrorCode.E2047;
    public override string Title => ErrorCode.GetEnumDescriptions();
}
