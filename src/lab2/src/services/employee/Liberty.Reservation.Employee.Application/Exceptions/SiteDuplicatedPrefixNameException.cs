using Liberty.ApplicationShared.Extensions;
using Liberty.SysException;
using Liberty.SysException.Exceptions;

namespace Liberty.Reservation.Employee.Application.Exceptions;

public class SiteDuplicatedPrefixNameException(
    string prefixName
) : AppInvalidException
{
    public override ErrorCode ErrorCode => ErrorCode.E1074;

    public override string Title => string.Format(
        ErrorCode.GetEnumDescriptions(),
        prefixName
    );
}
