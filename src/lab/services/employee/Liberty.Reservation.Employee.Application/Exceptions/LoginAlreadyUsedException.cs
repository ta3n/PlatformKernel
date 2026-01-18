using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Exceptions;

namespace Liberty.Reservation.Employee.Application.Exceptions;

public class LoginAlreadyUsedException : AppInvalidException
{
    public override ErrorCode ErrorCode => ErrorCode.E0000x;

    public override string Title => "Login name is already in use!";
}
