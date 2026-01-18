using Liberty.ApplicationShared.Extensions;
using Liberty.SysException;
using Liberty.SysException.Exceptions;

namespace Liberty.Reservation.Employee.Application.Exceptions;

public class MailTemplateSubjectFormatInvalidException : AppInvalidException
{
    public override ErrorCode ErrorCode => ErrorCode.E1003;
    public override string Title => ErrorCode.GetEnumDescriptions();
}
