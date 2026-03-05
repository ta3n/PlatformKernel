using Liberty.ApplicationShared.Extensions;
using Liberty.SysException.Exceptions;

namespace Liberty.Reservation.Site.Application.Exceptions;

public class QuestionHasChangesException : AppNotfoundException
{
    public override ErrorCode ErrorCode => ErrorCode.E2056;
    public override string Title => ErrorCode.GetEnumDescriptions();
}
