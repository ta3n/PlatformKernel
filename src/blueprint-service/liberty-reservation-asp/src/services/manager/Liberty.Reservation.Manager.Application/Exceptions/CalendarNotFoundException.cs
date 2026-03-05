using Liberty.ApplicationShared.Extensions;

namespace Liberty.Reservation.Manager.Application.Exceptions;

public class CalendarNotFoundException : AppNotfoundException
{
    public override ErrorCode ErrorCode => ErrorCode.E1031;
    public override string Title => ErrorCode.GetEnumDescriptions();
}
