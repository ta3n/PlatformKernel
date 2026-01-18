using Liberty.ApplicationShared.Extensions;
using Liberty.SysException;
using Liberty.SysException.Exceptions;

namespace Liberty.Reservation.Employee.Application.Exceptions;

public class AppDateDataNotfoundException : AppNotfoundException
{
    public override ErrorCode ErrorCode => ErrorCode.E1006;
    public override string Title => ErrorCode.GetEnumDescriptions();
}
