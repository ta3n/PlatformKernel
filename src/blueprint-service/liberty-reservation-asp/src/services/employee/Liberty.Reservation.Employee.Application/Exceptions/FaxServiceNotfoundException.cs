using Liberty.ApplicationShared.Extensions;
using Liberty.SysException;
using Liberty.SysException.Exceptions;

namespace Liberty.Reservation.Employee.Application.Exceptions;

public class FaxServiceNotfoundException : AppNotfoundException
{
    public override ErrorCode ErrorCode => ErrorCode.E1012;
    public override string Title => ErrorCode.GetEnumDescriptions();
}
