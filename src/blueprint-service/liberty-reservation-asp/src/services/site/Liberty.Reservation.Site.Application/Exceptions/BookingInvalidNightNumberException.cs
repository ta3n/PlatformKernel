using Liberty.ApplicationShared.Extensions;
using Liberty.SysException.Exceptions;

namespace Liberty.Reservation.Site.Application.Exceptions;

public class BookingInvalidNightNumberException : AppNotfoundException
{
    public override ErrorCode ErrorCode => ErrorCode.E2037;
    public override string Title => ErrorCode.GetEnumDescriptions();
}
