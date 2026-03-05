using Liberty.ApplicationShared.Extensions;
using Liberty.SysException;

namespace Liberty.Reservation.Application.Exceptions;

public class ReservationHasNoAdultPersonAgeTypesException() : ReservationInvalidException("予約情報に「大人」区分がありません")
{
    public override ErrorCode ErrorCode => ErrorCode.E2033;

    public override string Title => ErrorCode.GetEnumDescriptions();

    public override string ApproachMessage => "予約情報に「大人」区分がありません";
}
