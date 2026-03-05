using Liberty.ApplicationShared.Extensions;
using Liberty.SysException;

namespace Liberty.Reservation.Application.Exceptions;

public class ReservationNoMatchPersonsException() : ReservationInvalidException("予約情報の人数が一致しません")
{
    public override ErrorCode ErrorCode => ErrorCode.E2028;

    public override string Title => ErrorCode.GetEnumDescriptions();

    public override string ApproachMessage => "予約情報の人数が一致しません";
}
