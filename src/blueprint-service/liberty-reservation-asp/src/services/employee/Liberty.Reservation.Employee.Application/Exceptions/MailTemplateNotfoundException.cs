using Liberty.ApplicationShared.Extensions;
using Liberty.SysException;
using Liberty.SysException.Exceptions;

namespace Liberty.Reservation.Employee.Application.Exceptions;

public class MailTemplateNotfoundException(
    string mailTypeIo
) : AppNotfoundException
{
    public override ErrorCode ErrorCode => ErrorCode.E1013;

    public override string Title => string.Format(
        ErrorCode.GetEnumDescriptions(),
        mailTypeIo
    );
}
