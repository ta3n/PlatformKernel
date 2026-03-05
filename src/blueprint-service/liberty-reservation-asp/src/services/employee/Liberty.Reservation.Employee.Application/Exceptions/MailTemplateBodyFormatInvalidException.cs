using Liberty.ApplicationShared.Extensions;
using Liberty.SysException;
using Liberty.SysException.Exceptions;

namespace Liberty.Reservation.Employee.Application.Exceptions;

public class MailTemplateBodyFormatInvalidException : AppInvalidException
{
    public override ErrorCode ErrorCode => ErrorCode.E1002;
    public override string Title => ErrorCode.GetEnumDescriptions();
}
