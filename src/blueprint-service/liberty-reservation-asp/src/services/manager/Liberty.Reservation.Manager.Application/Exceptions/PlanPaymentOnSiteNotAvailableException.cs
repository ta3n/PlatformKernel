using Liberty.ApplicationShared.Extensions;

namespace Liberty.Reservation.Manager.Application.Exceptions;

public class PlanPaymentOnSiteNotAvailableException : AppInvalidException
{
    public override ErrorCode ErrorCode => ErrorCode.E1063;
    public override string Title => ErrorCode.GetEnumDescriptions();
}
