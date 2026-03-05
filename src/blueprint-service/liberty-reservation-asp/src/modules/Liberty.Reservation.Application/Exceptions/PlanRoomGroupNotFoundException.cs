using Liberty.ApplicationShared.Extensions;
using Liberty.SysException;
using Liberty.SysException.Exceptions;

namespace Liberty.Reservation.Application.Exceptions;

public class PlanRoomGroupNotFoundException(
    long facilityId,
    long planId,
    long roomGroupId
) : ModelException
{
    public override ErrorCode ErrorCode => ErrorCode.E2006;

    public override string Title => string.Format(
        ErrorCode.GetEnumDescriptions(),
        facilityId,
        planId,
        roomGroupId
    );

    public override string ApproachMessage => "";
}
