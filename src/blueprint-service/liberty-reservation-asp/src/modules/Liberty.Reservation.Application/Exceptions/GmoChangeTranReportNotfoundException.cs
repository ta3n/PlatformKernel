using Liberty.ApplicationShared.Extensions;
using Liberty.SysException;
using Liberty.SysException.Exceptions;

namespace Liberty.Reservation.Application.Exceptions;

public class GmoChangeTranReportNotfoundException : AppNotfoundException
{
    public override ErrorCode ErrorCode => ErrorCode.E2002;
    public override string Title => ErrorCode.GetEnumDescriptions();
}
