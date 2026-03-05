using Liberty.SysException.Exceptions;
using Liberty.SysException;
using Liberty.ApplicationShared.Extensions;

namespace Liberty.Reservation.Application.Exceptions;

public class KakusanException : AppInvalidException
{
    public override ErrorCode ErrorCode => ErrorCode.E4001;
    public override string Title => ErrorCode.GetEnumDescriptions();
}
