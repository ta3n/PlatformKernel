using Liberty.ApplicationShared.Extensions;

namespace Liberty.Reservation.Manager.Application.Exceptions;

public class ReservationChangeNotAllowedException : AppInvalidException
{
    public override ErrorCode ErrorCode => ErrorCode.E1060;
    public override string Title => ErrorCode.GetEnumDescriptions();
}
