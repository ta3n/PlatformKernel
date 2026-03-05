using Liberty.ApplicationShared.Extensions;
using Liberty.SysException.Exceptions;

namespace Liberty.Reservation.Site.Application.Exceptions;

public class RoomGroupHasChangesException : AppNotfoundException
{
    public override ErrorCode ErrorCode => ErrorCode.E2048;
    public override string Title => ErrorCode.GetEnumDescriptions();
}
