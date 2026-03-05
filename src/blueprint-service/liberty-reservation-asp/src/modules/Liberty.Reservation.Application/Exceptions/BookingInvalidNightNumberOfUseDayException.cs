using Liberty.ApplicationShared.Extensions;
using Liberty.SysException;
using Liberty.SysException.Exceptions;

namespace Liberty.Reservation.Application.Exceptions;

public class BookingInvalidNightNumberOfUseDayException : AppInvalidException
{
    public override ErrorCode ErrorCode => ErrorCode.E0016;
    public override string Title => ErrorCode.GetEnumDescriptions();
}
