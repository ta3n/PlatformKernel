using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Exceptions;

namespace Liberty.Reservation.Employee.Application.Exceptions;

public class EmailAlreadyUsedException : AppInvalidException
{
    public override ErrorCode ErrorCode => ErrorCode.E0000x;
    public override string Title => "Email is already in use!";
}
