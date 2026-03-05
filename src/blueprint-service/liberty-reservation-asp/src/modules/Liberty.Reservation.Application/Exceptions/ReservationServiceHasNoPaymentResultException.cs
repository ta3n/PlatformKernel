using Liberty.ApplicationShared.Extensions;
using Liberty.SysException;

namespace Liberty.Reservation.Application.Exceptions;

public class ReservationServiceHasNoPaymentResultException : ReservationServiceException
{
    public override ErrorCode ErrorCode => ErrorCode.E2010;
    public override string Title => ErrorCode.GetEnumDescriptions();
}
