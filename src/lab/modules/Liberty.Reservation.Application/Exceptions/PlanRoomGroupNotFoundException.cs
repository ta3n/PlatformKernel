using Liberty.Reservation.Application.Constants;

namespace Liberty.Reservation.Application.Exceptions;

public class PlanRoomGroupNotFoundException : ModelException
{
    public override ErrorCode ErrorCode => ErrorCode.X00028;

    public override string Title => "エラー";

    public override string ApproachMessage => "";

    public PlanRoomGroupNotFoundException() : base($"指定のプランもしくは部屋タイプは利用できません")
    {
    }

    public PlanRoomGroupNotFoundException(
        long facilityId,
        long planId,
        long roomGroupId
    ) : base($"指定のプランもしくは部屋タイプは利用できません")
    {
    }
}
