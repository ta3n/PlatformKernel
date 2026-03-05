using Liberty.ApplicationShared.Extensions;
using Liberty.SysException;

namespace Liberty.Reservation.Application.Exceptions;

public class ReservationOnLinePaymentExpiredException(
    long reservationId
) : ReservationException
{
    public override ErrorCode ErrorCode => ErrorCode.E2024;

    public override string Title => string.Format(
        ErrorCode.GetEnumDescriptions(),
        reservationId
    );

    public override string ApproachMessage => "";
}
