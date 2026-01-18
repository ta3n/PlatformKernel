using Liberty.ApplicationShared.Extensions;
using Liberty.SysException;

namespace Liberty.Reservation.Application.Exceptions;

public class ReservationNoPriceSettingException() : ReservationException("ご指定の条件でご予約できません")
{
    public override ErrorCode ErrorCode => ErrorCode.E2027;

    public override string ErrorCaption => ErrorCode.GetEnumDescriptions();

    public override string ApproachMessage => "";
}
