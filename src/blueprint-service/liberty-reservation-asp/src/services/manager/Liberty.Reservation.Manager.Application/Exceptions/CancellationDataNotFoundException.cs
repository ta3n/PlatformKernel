using Liberty.ApplicationShared.Extensions;

namespace Liberty.Reservation.Manager.Application.Exceptions;

public class CancellationDataNotFoundException : AppNotfoundException
{
    public override ErrorCode ErrorCode => ErrorCode.E1032;
    public override string Title => ErrorCode.GetEnumDescriptions();
}
