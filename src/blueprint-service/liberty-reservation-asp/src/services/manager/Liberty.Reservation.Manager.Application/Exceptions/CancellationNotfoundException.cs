using Liberty.ApplicationShared.Extensions;

namespace Liberty.Reservation.Manager.Application.Exceptions;

public class CancellationNotfoundException : AppNotfoundException
{
    public override ErrorCode ErrorCode => ErrorCode.E1033;
    public override string Title => ErrorCode.GetEnumDescriptions();
}
