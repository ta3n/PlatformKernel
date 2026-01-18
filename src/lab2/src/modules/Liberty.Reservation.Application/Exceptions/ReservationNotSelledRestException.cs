using Liberty.ApplicationShared.Extensions;
using Liberty.SysException;

namespace Liberty.Reservation.Application.Exceptions;

public class ReservationNotSelledRestException : ReservationServiceException
{
    public override ErrorCode ErrorCode => ErrorCode.E2025;

    public override string Title => ErrorCode.GetEnumDescriptions();
}
