using Liberty.ApplicationShared.Extensions;
using Liberty.SysException.Exceptions;

namespace Liberty.Reservation.Site.Application.Exceptions;

public class FacilityHasChangesException : AppNotfoundException
{
    public override ErrorCode ErrorCode => ErrorCode.E2045;
    public override string Title => ErrorCode.GetEnumDescriptions();
}
