using Liberty.ApplicationShared.Extensions;

namespace Liberty.Reservation.Manager.Application.Exceptions;

public class OrderNotfoundException : AppNotfoundException
{
    public override ErrorCode ErrorCode => ErrorCode.E2001;
    public override string Title => ErrorCode.GetEnumDescriptions();
}
