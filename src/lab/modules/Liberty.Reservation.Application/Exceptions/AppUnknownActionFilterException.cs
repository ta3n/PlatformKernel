using Liberty.Reservation.Application.Constants;

namespace Liberty.Reservation.Application.Exceptions;

public class AppUnknownActionFilterException(
    Exception exception
) : AppUnknownException
{
    public override ErrorCode ErrorCode => ErrorCode.E0000x;

    public override string Title => "Unknown Error";

    public Exception Exception { get; } = exception;
}
