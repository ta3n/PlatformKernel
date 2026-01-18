using Liberty.ApplicationShared.Extensions;

namespace Liberty.Reservation.Manager.Application.Exceptions;

public class CancellationPolicyHasPlanUsingException : AppInvalidException
{
    public override ErrorCode ErrorCode => ErrorCode.E1064;
    public override string Title => ErrorCode.GetEnumDescriptions();
}
