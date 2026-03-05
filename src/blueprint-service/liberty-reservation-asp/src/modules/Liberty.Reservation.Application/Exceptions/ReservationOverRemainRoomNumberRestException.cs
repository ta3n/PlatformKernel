using Liberty.ApplicationShared.Extensions;
using Liberty.SysException;

namespace Liberty.Reservation.Application.Exceptions;

public class ReservationOverRemainRoomNumberRestException : ReservationServiceException
{
    public override ErrorCode ErrorCode => ErrorCode.E2014;
    public override string Title => ErrorCode.GetEnumDescriptions();
}
