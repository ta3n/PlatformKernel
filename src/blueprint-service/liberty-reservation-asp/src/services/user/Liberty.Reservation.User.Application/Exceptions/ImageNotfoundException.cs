using Liberty.ApplicationShared.Extensions;
using Liberty.SysException;
using Liberty.SysException.Exceptions;

namespace Liberty.Reservation.User.Application.Exceptions;

public class ImageNotfoundException : AppNotfoundException
{
    public override ErrorCode ErrorCode => ErrorCode.E1034;
    public override string Title => ErrorCode.GetEnumDescriptions();
}
