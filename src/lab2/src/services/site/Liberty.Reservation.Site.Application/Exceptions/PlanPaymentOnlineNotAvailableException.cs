using Liberty.ApplicationShared.Extensions;
using Liberty.SysException.Exceptions;

namespace Liberty.Reservation.Site.Application.Exceptions;

public class PlanPaymentOnlineNotAvailableException : AppInvalidException
{
    public override ErrorCode ErrorCode => ErrorCode.E2036;
    public override string Title => ErrorCode.GetEnumDescriptions();
}
