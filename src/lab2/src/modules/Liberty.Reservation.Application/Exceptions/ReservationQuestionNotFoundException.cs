using Liberty.ApplicationShared.Extensions;
using Liberty.SysException;

namespace Liberty.Reservation.Application.Exceptions;

public class ReservationQuestionNotFoundException(
    long reservationId,
    long questionId
) : ReservationException
{
    public override ErrorCode ErrorCode => ErrorCode.E2011;

    public override string Title => string.Format(
        ErrorCode.GetEnumDescriptions(),
        reservationId,
        questionId
    );

    public override string ApproachMessage => "";

    public override bool IsNotFound => true;
}
