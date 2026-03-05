using Liberty.ApplicationShared.Extensions;
using Liberty.SysException.Exceptions;

namespace Liberty.Reservation.Site.Application.Exceptions;

public class RoomGroupNotfoundException : AppNotfoundException
{
    public override ErrorCode ErrorCode => ErrorCode.E1044;
    public override string Title => ErrorCode.GetEnumDescriptions();
}
