using Liberty.ApplicationShared.Extensions;
using Liberty.SysException;
using Liberty.SysException.Exceptions;

namespace Liberty.Reservation.User.Application.Exceptions;

public class FacilityNotfoundException : AppNotfoundException
{
    public override ErrorCode ErrorCode => ErrorCode.E1011;
    public override string Title => ErrorCode.GetEnumDescriptions();
}
