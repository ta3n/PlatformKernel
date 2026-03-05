using Liberty.ApplicationShared.Extensions;
using Liberty.SysException.Exceptions;

namespace Liberty.Reservation.Site.Application.Exceptions;

public class RoomGroupNotAlreadyInPlanException : AppNotfoundException
{
    public override ErrorCode ErrorCode => ErrorCode.E1047;
    public override string Title => ErrorCode.GetEnumDescriptions();
}
