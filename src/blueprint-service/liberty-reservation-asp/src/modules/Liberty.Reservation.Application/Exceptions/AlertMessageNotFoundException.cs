using Liberty.ApplicationShared.Extensions;
using Liberty.SysException;
using Liberty.SysException.Exceptions;

namespace Liberty.Reservation.Application.Exceptions;

public class AlertMessageNotFoundException : AppNotfoundException
{
    public override ErrorCode ErrorCode => ErrorCode.E1067;
    public override string Title => ErrorCode.GetEnumDescriptions();
}
