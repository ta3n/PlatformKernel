using Liberty.Reservation.Application.Constants;

namespace Liberty.Reservation.Application.Exceptions;

public class RequestInvalidException(
    ErrorCode errorCode,
    string title
) : AppInvalidException
{
    public override ErrorCode ErrorCode => errorCode;
    public override string Title => title;
}
