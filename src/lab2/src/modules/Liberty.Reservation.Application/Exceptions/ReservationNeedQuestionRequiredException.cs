using Liberty.ApplicationShared.Extensions;
using Liberty.SysException;
using Liberty.SysException.Exceptions;

namespace Liberty.Reservation.Application.Exceptions;

public class ReservationNeedQuestionRequiredException(
    string questionName
) : AppInvalidException
{
    public override ErrorCode ErrorCode => ErrorCode.E2071;
    public override string Title => string.Format(ErrorCode.GetEnumDescriptions(), questionName);
}
