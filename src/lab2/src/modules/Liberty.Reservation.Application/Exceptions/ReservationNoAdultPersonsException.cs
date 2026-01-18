using Liberty.ApplicationShared.Extensions;
using Liberty.SysException;

namespace Liberty.Reservation.Application.Exceptions;

public class ReservationNoAdultPersonsException() : ReservationException("ご指定の条件でご予約できません")
{
    public override ErrorCode ErrorCode => ErrorCode.E2031;
    public override string Title => ErrorCode.GetEnumDescriptions();

    public override string ErrorCaption => "予約エラー";

    public override string ApproachMessage => "";
}
