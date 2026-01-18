using Liberty.ApplicationShared.Extensions;

namespace Liberty.Reservation.Manager.Application.Exceptions;

public class PriceCalendarRangeLockedException : AppInvalidException
{
    public override ErrorCode ErrorCode => ErrorCode.E2077;
    public override string Title => ErrorCode.GetEnumDescriptions();
}
