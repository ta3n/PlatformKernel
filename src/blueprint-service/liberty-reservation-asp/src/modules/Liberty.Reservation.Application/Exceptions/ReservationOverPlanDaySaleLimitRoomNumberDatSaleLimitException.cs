using Liberty.ApplicationShared.Extensions;
using Liberty.SysException;

namespace Liberty.Reservation.Application.Exceptions;

public class ReservationOverPlanDaySaleLimitRoomNumberDataSaleLimitException : ReservationServiceException
{
    public override ErrorCode ErrorCode => ErrorCode.E2016;
    public override string Title => ErrorCode.GetEnumDescriptions();
}
