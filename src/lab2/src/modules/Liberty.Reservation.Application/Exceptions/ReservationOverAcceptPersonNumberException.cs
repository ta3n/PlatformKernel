using Liberty.ApplicationShared.Extensions;
using Liberty.SysException;

namespace Liberty.Reservation.Application.Exceptions;

public class ReservationOverAcceptPersonNumberException : ReservationServiceException
{
    public override ErrorCode ErrorCode => ErrorCode.E2020;
    public override string Title => ErrorCode.GetEnumDescriptions();
}
