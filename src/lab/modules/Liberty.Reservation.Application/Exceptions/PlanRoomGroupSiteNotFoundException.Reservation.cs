using Liberty.Reservation.Application.Constants;

namespace Liberty.Reservation.Application.Exceptions;

public class PlanRoomGroupSiteNotFoundException : ModelException
{
    public override ErrorCode ErrorCode => ErrorCode.X00029;

    public override string Title => "エラー";

    public override string ApproachMessage => "";

    public PlanRoomGroupSiteNotFoundException() : base($"指定のプラン、部屋タイプ、もしくは掲載先は利用できません")
    {
    }

    public PlanRoomGroupSiteNotFoundException(
        long planId,
        long roomGroupId,
        long siteId
    ) : base($"指定のプラン、部屋タイプ、もしくは掲載先は利用できません")
    {
    }
}
