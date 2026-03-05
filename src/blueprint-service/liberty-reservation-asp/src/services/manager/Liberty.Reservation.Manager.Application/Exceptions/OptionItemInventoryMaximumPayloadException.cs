using Liberty.ApplicationShared.Extensions;

namespace Liberty.Reservation.Manager.Application.Exceptions;

public class OptionItemInventoryMaximumPayloadException : AppNotfoundException
{
    public override ErrorCode ErrorCode => ErrorCode.E1050;
    public override string Title => ErrorCode.GetEnumDescriptions();
}
