using Liberty.ApplicationShared.Extensions;

namespace Liberty.Reservation.Manager.Application.Exceptions;

public class AppDateNotfoundException : AppNotfoundException
{
    public override ErrorCode ErrorCode => ErrorCode.E1005;
    public override string Title => ErrorCode.GetEnumDescriptions();
}
