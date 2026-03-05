using Liberty.ApplicationShared.Extensions;
using Liberty.SysException.Exceptions;

namespace Liberty.Reservation.Site.Application.Exceptions;

public class BookingNotfoundException : AppNotfoundException
{
    public override ErrorCode ErrorCode => ErrorCode.E2002;
    public override string Title => ErrorCode.GetEnumDescriptions();
}
