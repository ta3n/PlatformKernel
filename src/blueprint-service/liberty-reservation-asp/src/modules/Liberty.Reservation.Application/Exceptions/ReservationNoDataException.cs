using Liberty.ApplicationShared.Extensions;
using Liberty.SysException;

namespace Liberty.Reservation.Application.Exceptions;

public class ReservationNoDataException : ReservationServiceException
{
    public override ErrorCode ErrorCode => ErrorCode.E2030;
    public override string Title => ErrorCode.GetEnumDescriptions();
}
