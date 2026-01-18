using Liberty.ApplicationShared.Extensions;
using Liberty.SysException;

namespace Liberty.Reservation.Application.Exceptions;

public class ReservationOverDisplayDateException : ReservationServiceException
{
    public override ErrorCode ErrorCode => ErrorCode.E2017;
    public override string Title => ErrorCode.GetEnumDescriptions();
}
