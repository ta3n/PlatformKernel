using Liberty.ApplicationShared.Extensions;

namespace Liberty.Reservation.Manager.Application.Exceptions;

public class AreaNotfoundException : AppNotfoundException
{
    public override ErrorCode ErrorCode => ErrorCode.E1008;
    public override string Title => ErrorCode.GetEnumDescriptions();
}
