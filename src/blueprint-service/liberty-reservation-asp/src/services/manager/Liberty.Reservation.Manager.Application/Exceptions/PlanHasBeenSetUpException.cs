using Liberty.ApplicationShared.Extensions;

namespace Liberty.Reservation.Manager.Application.Exceptions;

public class PlanHasBeenSetUpException : AppInvalidException
{
    public override ErrorCode ErrorCode => ErrorCode.E2062;
    public override string Title => ErrorCode.GetEnumDescriptions();
}
