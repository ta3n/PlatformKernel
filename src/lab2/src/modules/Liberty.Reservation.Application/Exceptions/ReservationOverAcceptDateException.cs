using Liberty.ApplicationShared.Extensions;
using Liberty.SysException;

namespace Liberty.Reservation.Application.Exceptions;

public class ReservationOverAcceptDateException : ReservationServiceException
{
    public override ErrorCode ErrorCode => ErrorCode.E2021;
    public override string Title => ErrorCode.GetEnumDescriptions();
}
