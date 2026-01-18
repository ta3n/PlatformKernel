using Liberty.ApplicationShared.Extensions;
using Liberty.SysException;
using Liberty.SysException.Exceptions;

namespace Liberty.Reservation.Application.Exceptions;

public class ReservationCancellationDataNotFoundException : AppNotfoundException
{
    public override ErrorCode ErrorCode => ErrorCode.E1032;
    public override string Title => ErrorCode.GetEnumDescriptions();
}
