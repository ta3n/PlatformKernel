using Liberty.ApplicationShared.Extensions;
using Liberty.SysException;
using Liberty.SysException.Exceptions;

namespace Liberty.Reservation.Application.Exceptions;

public class PlanRoomGroupSiteNotFoundException(
    long planId,
    long roomGroupId,
    long siteId
) : ModelException
{
    public override ErrorCode ErrorCode => ErrorCode.E2007;

    public override string Title => string.Format(
        ErrorCode.GetEnumDescriptions(),
        planId,
        roomGroupId,
        siteId
    );

    public override string ApproachMessage => "";
}
