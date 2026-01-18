using Liberty.ApplicationShared.Extensions;
using Liberty.SysException.Exceptions;

namespace Liberty.Reservation.Site.Application.Exceptions;

public class PersonAgeTypeHasChangesException : AppInvalidException
{
    public override ErrorCode ErrorCode => ErrorCode.E2051;
    public override string Title => ErrorCode.GetEnumDescriptions();
}
