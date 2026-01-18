using Liberty.ApplicationShared.Extensions;
using Liberty.SysException.Exceptions;

namespace Liberty.Reservation.Site.Application.Exceptions;

public class BookingInvalidRoomNumberException : AppInvalidException
{
    public override ErrorCode ErrorCode => ErrorCode.E2038;
    public override string Title => ErrorCode.GetEnumDescriptions();
}
