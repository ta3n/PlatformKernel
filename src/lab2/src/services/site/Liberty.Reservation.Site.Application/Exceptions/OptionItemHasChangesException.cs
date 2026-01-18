using Liberty.ApplicationShared.Extensions;
using Liberty.SysException.Exceptions;

namespace Liberty.Reservation.Site.Application.Exceptions;

public class OptionItemHasChangesException : AppInvalidException
{
    public override ErrorCode ErrorCode => ErrorCode.E2049;
    public override string Title => ErrorCode.GetEnumDescriptions();
}
