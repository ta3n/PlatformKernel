using Liberty.ApplicationShared.Extensions;
using Liberty.SysException;
using Liberty.SysException.Exceptions;

namespace Liberty.Reservation.User.Application.Exceptions;

public class FacilityNotAllowExtendedStayException : AppInvalidException
{
    public override ErrorCode ErrorCode => ErrorCode.E1070;
    public override string Title => ErrorCode.GetEnumDescriptions();
}
