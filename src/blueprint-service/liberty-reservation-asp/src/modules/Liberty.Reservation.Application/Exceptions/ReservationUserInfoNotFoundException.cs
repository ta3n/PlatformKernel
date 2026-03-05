using Liberty.ApplicationShared.Extensions;
using Liberty.SysException;

namespace Liberty.Reservation.Application.Exceptions;

public class ReservationUserInfoNotFoundException(
    long reservationId,
    long appDateId,
    long roomGroupIndex
) : ReservationException
{
    public override ErrorCode ErrorCode => ErrorCode.E2005;

    public override string Title => string.Format(
        ErrorCode.GetEnumDescriptions(),
        reservationId,
        appDateId,
        roomGroupIndex
    );

    public override string ApproachMessage => "";

    public override bool IsNotFound => true;
}
