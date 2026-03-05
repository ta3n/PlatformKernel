using Liberty.ApplicationShared.Extensions;
using Liberty.SysException;

namespace Liberty.Reservation.Application.Exceptions;

public class ReservationOverCapacityNumberRestException : ReservationServiceException
{
    public override ErrorCode ErrorCode => ErrorCode.E2019;
    public override string Title => ErrorCode.GetEnumDescriptions();
}
