using Liberty.ApplicationShared.Extensions;
using Liberty.SysException.Exceptions;

namespace Liberty.Reservation.Site.Application.Exceptions;

public class OnlinePaymentNotAllowedForGuestException : AppNotfoundException
{
    public override ErrorCode ErrorCode => ErrorCode.E2063;
    public override string Title => ErrorCode.GetEnumDescriptions();
}
