using Liberty.ApplicationShared.Extensions;
using Liberty.SysException.Exceptions;

namespace Liberty.Reservation.Site.Application.Exceptions;

public class OptionItemUpdatedTimeInvalidException : AppInvalidException
{
    public override ErrorCode ErrorCode => ErrorCode.E2059;
    public override string Title => ErrorCode.GetEnumDescriptions();
}
