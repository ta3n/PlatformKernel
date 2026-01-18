using Liberty.ApplicationShared.Extensions;

namespace Liberty.Reservation.Manager.Application.Exceptions;

public class PlanUseDayInvalidNightLimitException : AppInvalidException
{
    public override ErrorCode ErrorCode => ErrorCode.E0016;
    public override string Title => ErrorCode.GetEnumDescriptions();
}
