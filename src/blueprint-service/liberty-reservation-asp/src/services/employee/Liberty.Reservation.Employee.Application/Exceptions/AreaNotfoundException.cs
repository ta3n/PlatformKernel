using Liberty.ApplicationShared.Extensions;
using Liberty.SysException;
using Liberty.SysException.Exceptions;

namespace Liberty.Reservation.Employee.Application.Exceptions;

public class AreaNotfoundException : AppNotfoundException
{
    public override ErrorCode ErrorCode => ErrorCode.E1008;
    public override string Title => ErrorCode.GetEnumDescriptions();
}
