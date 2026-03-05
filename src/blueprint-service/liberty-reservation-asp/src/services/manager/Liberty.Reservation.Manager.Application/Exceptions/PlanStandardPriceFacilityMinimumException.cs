using Liberty.ApplicationShared.Extensions;

namespace Liberty.Reservation.Manager.Application.Exceptions;

public class PlanStandardPriceFacilityMinimumException : AppException
{
    public override ErrorCode ErrorCode => ErrorCode.E1057;
    public override string Title => ErrorCode.GetEnumDescriptions();
}
