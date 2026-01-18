using Liberty.ApplicationShared.Extensions;

namespace Liberty.Reservation.Manager.Application.Exceptions;

public class PlanDailyPriceBelowFacilityMinimumException : AppInvalidException
{
    public override ErrorCode ErrorCode => ErrorCode.E1056;
    public override string Title => ErrorCode.GetEnumDescriptions();
}
