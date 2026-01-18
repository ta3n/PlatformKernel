using Liberty.ApplicationShared.Extensions;
using Liberty.SysException;

namespace Liberty.Reservation.Application.Exceptions;

public class ReservationNoRemainRoomNumberRestException : ReservationServiceException
{
    public override ErrorCode ErrorCode => ErrorCode.E2026;
    public override string Title => ErrorCode.GetEnumDescriptions();
}
