using Liberty.ApplicationShared.Extensions;
using Liberty.SysException;
using Liberty.SysException.Exceptions;

namespace Liberty.Reservation.Employee.Application.Exceptions;

public class FaxNumberIncorrectException(
    string message
) : AppInvalidException
{
    public override ErrorCode ErrorCode => ErrorCode.E1051;
    public override string Title => ErrorCode.GetEnumDescriptions();
    public override string Message => message;
}
