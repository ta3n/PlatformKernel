using Liberty.ApplicationShared.Extensions;
using Liberty.SysException;

namespace Liberty.Reservation.Application.Exceptions;

public class ReservationOverPlanGroupNumberDaySaleLimitException : ReservationServiceException
{
    public override ErrorCode ErrorCode => ErrorCode.E2015;
    public override string Title => ErrorCode.GetEnumDescriptions();
}
