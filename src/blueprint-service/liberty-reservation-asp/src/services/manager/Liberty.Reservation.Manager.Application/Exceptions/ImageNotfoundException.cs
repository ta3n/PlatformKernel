using Liberty.ApplicationShared.Extensions;

namespace Liberty.Reservation.Manager.Application.Exceptions;

public class ImageNotfoundException : AppNotfoundException
{
    public override ErrorCode ErrorCode => ErrorCode.E1034;
    public override string Title => ErrorCode.GetEnumDescriptions();
}
