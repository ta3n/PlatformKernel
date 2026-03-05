using Liberty.ApplicationShared.Extensions;
using Liberty.SysException;
using Liberty.SysException.Exceptions;

namespace Liberty.Reservation.Application.Exceptions;

public class CancelCancellationFeeException : AppInvalidException
{
    public override ErrorCode ErrorCode => ErrorCode.E2068;
    public override string Title => ErrorCode.GetEnumDescriptions();
}
