using Liberty.ApplicationShared.Extensions;
using Liberty.SysException.Exceptions;

namespace Liberty.Reservation.Site.Application.Exceptions;

public class FileHasChangesException : AppNotfoundException
{
    public override ErrorCode ErrorCode => ErrorCode.E2057;
    public override string Title => ErrorCode.GetEnumDescriptions();
}
