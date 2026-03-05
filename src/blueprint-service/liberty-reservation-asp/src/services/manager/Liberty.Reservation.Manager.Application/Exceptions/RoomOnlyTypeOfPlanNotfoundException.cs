using Liberty.ApplicationShared.Extensions;

namespace Liberty.Reservation.Manager.Application.Exceptions;

public class RoomOnlyTypeOfPlanNotfoundException : AppNotfoundException
{
    public override ErrorCode ErrorCode => ErrorCode.E1045;
    public override string Title => ErrorCode.GetEnumDescriptions();
}
