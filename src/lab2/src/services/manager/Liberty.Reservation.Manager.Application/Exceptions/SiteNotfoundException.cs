using Liberty.ApplicationShared.Extensions;

namespace Liberty.Reservation.Manager.Application.Exceptions;

public class SiteNotfoundException : AppNotfoundException
{
    public override ErrorCode ErrorCode => ErrorCode.E1015;
    public override string Title => ErrorCode.GetEnumDescriptions();
}
