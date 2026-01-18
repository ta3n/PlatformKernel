using Liberty.ApplicationShared.Extensions;
using Liberty.SysException;

namespace Liberty.Reservation.Application.Exceptions;

public class ReservationNoPersonAgeTypeException() : ReservationException("対応していない人数内訳の利用区分があります")
{
    public override ErrorCode ErrorCode => ErrorCode.E2029;

    public override string Title => ErrorCode.GetEnumDescriptions();

    public override string ErrorCaption => ErrorCode.GetEnumDescriptions();

    public override string ApproachMessage => "";
}
