using Liberty.ApplicationShared.Extensions;

namespace Liberty.Reservation.Manager.Application.Exceptions;

public class RoomGroupNotfoundException : AppNotfoundException
{
    public override ErrorCode ErrorCode => ErrorCode.E1044;
    public override string Title => ErrorCode.GetEnumDescriptions();
}
