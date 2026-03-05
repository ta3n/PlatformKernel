using Liberty.ApplicationShared.Extensions;
using Liberty.SysException.Exceptions;

namespace Liberty.Reservation.Site.Application.Exceptions;

public class CancellationHasChangesException : AppNotfoundException
{
    public override ErrorCode ErrorCode => ErrorCode.E2050;
    public override string Title => ErrorCode.GetEnumDescriptions();
}
