using Liberty.ApplicationShared.Extensions;

namespace Liberty.Reservation.Manager.Application.Exceptions;

public class OptionItemNotfoundException : AppNotfoundException
{
    public override ErrorCode ErrorCode => ErrorCode.E1038;
    public override string Title => ErrorCode.GetEnumDescriptions();
}
