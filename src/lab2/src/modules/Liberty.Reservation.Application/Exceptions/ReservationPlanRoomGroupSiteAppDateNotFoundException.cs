using Liberty.ApplicationShared.Extensions;
using Liberty.SysException;

namespace Liberty.Reservation.Application.Exceptions;

public class ReservationPlanRoomGroupSiteAppDateNotFoundException() : ReservationException("販売情報が見つかりません")
{
    public override ErrorCode ErrorCode => ErrorCode.E2013;

    public override string ErrorCaption => ErrorCode.GetEnumDescriptions();

    public override string ApproachMessage => "";
}
