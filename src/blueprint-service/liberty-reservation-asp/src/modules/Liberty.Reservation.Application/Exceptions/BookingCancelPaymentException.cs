using Liberty.ApplicationShared.Extensions;
using Liberty.SysException;
using Liberty.SysException.Exceptions;

namespace Liberty.Reservation.Application.Exceptions;

public class BookingCancelPaymentException : AppInvalidException
{
    public override ErrorCode ErrorCode => ErrorCode.E2066;
    public override string Title => ErrorCode.GetEnumDescriptions();
}
