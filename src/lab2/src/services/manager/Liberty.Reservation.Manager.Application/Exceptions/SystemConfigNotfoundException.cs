using Liberty.ApplicationShared.Extensions;

namespace Liberty.Reservation.Manager.Application.Exceptions;

public class SystemConfigNotfoundException : AppNotfoundException
{
    public override ErrorCode ErrorCode => ErrorCode.E1016;
    public override string Title => ErrorCode.GetEnumDescriptions();
}
