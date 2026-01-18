using Liberty.Reservation.Application.Constants;

namespace Liberty.Reservation.Application.Exceptions;

public class AppUnCodedErrorException : AppUnknownException
{
    public override ErrorCode ErrorCode => ErrorCode.E0000x;

    public override string Title => "UnCoded Error";
}
