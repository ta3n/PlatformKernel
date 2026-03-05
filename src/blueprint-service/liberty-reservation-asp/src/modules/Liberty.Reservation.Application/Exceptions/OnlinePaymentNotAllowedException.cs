using Liberty.ApplicationShared.Extensions;
using Liberty.Reservation.Application.Constants;
using Liberty.SysException;
using Liberty.SysException.Exceptions;

namespace Liberty.Reservation.Application.Exceptions;

public class OnlinePaymentNotAllowedException : AppInvalidException
{
    public override ErrorCode ErrorCode => ErrorCode.E2069;

    public override string Title => string.Format(
        ErrorCode.GetEnumDescriptions(),
        DefaultValues.OnlinePaymentDayLimit
    );
}
