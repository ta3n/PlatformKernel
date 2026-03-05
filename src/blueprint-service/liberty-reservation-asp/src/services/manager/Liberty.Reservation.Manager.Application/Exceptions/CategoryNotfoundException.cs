using Liberty.ApplicationShared.Extensions;

namespace Liberty.Reservation.Manager.Application.Exceptions;

public class CategoryNotfoundException : AppNotfoundException
{
    public override ErrorCode ErrorCode => ErrorCode.E1009;
    public override string Title => ErrorCode.GetEnumDescriptions();
}
