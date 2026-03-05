using Liberty.ApplicationShared.Extensions;

namespace Liberty.Reservation.Manager.Application.Exceptions;

public class AppDateTypeNotfoundException : AppNotfoundException
{
    public override ErrorCode ErrorCode => ErrorCode.E1007;
    public override string Title => ErrorCode.GetEnumDescriptions();
}
