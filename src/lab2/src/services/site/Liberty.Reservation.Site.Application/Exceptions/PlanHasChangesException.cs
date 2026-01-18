using Liberty.ApplicationShared.Extensions;
using Liberty.SysException.Exceptions;

namespace Liberty.Reservation.Site.Application.Exceptions;

public class PlanHasChangesException : AppInvalidException
{
    public override ErrorCode ErrorCode => ErrorCode.E2046;
    public override string Title => ErrorCode.GetEnumDescriptions();
}
