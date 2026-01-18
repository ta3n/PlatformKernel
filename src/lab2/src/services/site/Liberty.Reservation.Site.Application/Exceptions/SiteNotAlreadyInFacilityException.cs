using Liberty.ApplicationShared.Extensions;
using Liberty.SysException.Exceptions;

namespace Liberty.Reservation.Site.Application.Exceptions;

public class SiteNotAlreadyInFacilityException : AppNotfoundException
{
    public override ErrorCode ErrorCode => ErrorCode.E1048;
    public override string Title => ErrorCode.GetEnumDescriptions();
}
