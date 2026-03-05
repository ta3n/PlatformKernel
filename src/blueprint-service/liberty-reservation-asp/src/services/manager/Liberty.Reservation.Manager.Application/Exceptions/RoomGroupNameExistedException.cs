using Liberty.ApplicationShared.Extensions;

namespace Liberty.Reservation.Manager.Application.Exceptions;

public class RoomGroupNameExistedException : AppNotfoundException
{
    public override ErrorCode ErrorCode => ErrorCode.E1049;
    public override string Title => ErrorCode.GetEnumDescriptions();
}
