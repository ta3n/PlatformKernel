using Liberty.ApplicationShared.Extensions;
using Liberty.SysException.Exceptions;

namespace Liberty.Reservation.Site.Application.Exceptions;

public class PlanPaymentOnSiteNotAvailableException : AppInvalidException
{
    public override ErrorCode ErrorCode => ErrorCode.E2065;
    public override string Title => ErrorCode.GetEnumDescriptions();
}
