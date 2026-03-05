using Liberty.ApplicationShared.Extensions;

namespace Liberty.Reservation.Manager.Application.Exceptions;

public class PriceBelowFacilityMinimumException : AppNotfoundException
{
    public override ErrorCode ErrorCode => ErrorCode.E1055;
    public override string Title => ErrorCode.GetEnumDescriptions();
}
