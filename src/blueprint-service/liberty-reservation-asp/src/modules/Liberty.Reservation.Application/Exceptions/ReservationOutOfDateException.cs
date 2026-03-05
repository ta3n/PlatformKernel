using Liberty.ApplicationShared.Extensions;
using Liberty.SysException;

namespace Liberty.Reservation.Application.Exceptions;

public class ReservationOutOfDateException : ReservationServiceException
{
    public override ErrorCode ErrorCode => ErrorCode.E2022;
    public override string Title => ErrorCode.GetEnumDescriptions();
}
