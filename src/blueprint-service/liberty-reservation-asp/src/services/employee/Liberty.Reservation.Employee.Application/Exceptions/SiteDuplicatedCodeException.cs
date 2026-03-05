using Liberty.ApplicationShared.Extensions;
using Liberty.SysException;
using Liberty.SysException.Exceptions;

namespace Liberty.Reservation.Employee.Application.Exceptions;

public class SiteDuplicatedCodeException(
    string code
) : AppInvalidException
{
    public override ErrorCode ErrorCode => ErrorCode.E1004;

    public override string Title => string.Format(
        ErrorCode.GetEnumDescriptions(),
        code
    );
}
