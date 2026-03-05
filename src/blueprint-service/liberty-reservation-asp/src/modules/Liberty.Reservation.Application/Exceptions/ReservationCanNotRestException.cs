using Liberty.ApplicationShared.Extensions;
using Liberty.SysException;

namespace Liberty.Reservation.Application.Exceptions;

public class ReservationCanNotRestException(
    long appDateId
) : ReservationException("予約変更の締切を過ぎているため、この予約は変更できません。")
{
    public override ErrorCode ErrorCode => ErrorCode.E2034;

    public override string ErrorCaption => string.Format(
        ErrorCode.GetEnumDescriptions(),
        appDateId
    );

    public override string ApproachMessage => "";
}
