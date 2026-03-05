using Liberty.ApplicationShared.Extensions;

namespace Liberty.SysException.Exceptions;

public class GuestReservationCodeIncorrectException : AppNotfoundException
{
    public override ErrorCode ErrorCode => ErrorCode.E2044;
    public override string Title => ErrorCode.GetEnumDescriptions();
}
