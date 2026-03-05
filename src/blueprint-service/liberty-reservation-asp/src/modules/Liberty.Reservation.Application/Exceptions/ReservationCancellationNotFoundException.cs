using Liberty.ApplicationShared.Extensions;
using Liberty.SysException;

namespace Liberty.Reservation.Application.Exceptions;

public class ReservationCancellationNotFoundException() : ReservationException("予約情報のキャンセル規定が見つかりません")
{
    public override ErrorCode ErrorCode => ErrorCode.E2035;

    public override string Title => ErrorCode.GetEnumDescriptions();

    public override string ApproachMessage => "";

    public override bool IsNotFound => true;
}
