using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Exceptions;

namespace Liberty.Reservation.Employee.Application.Exceptions;

public class EmployeeMetaNotfoundException : AppNotfoundException
{
    public override ErrorCode ErrorCode => ErrorCode.E0000x;
    public override string Title => "Employee meta not found";
}
