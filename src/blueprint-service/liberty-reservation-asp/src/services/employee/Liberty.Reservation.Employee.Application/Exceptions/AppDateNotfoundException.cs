using Liberty.ApplicationShared.Extensions;
using Liberty.SysException;
using Liberty.SysException.Exceptions;

namespace Liberty.Reservation.Employee.Application.Exceptions;

public class AppDateNotfoundException : AppNotfoundException
{
    public override ErrorCode ErrorCode => ErrorCode.E1005;
    public override string Title => ErrorCode.GetEnumDescriptions();
}
