using Liberty.ApplicationShared.Extensions;

namespace Liberty.Reservation.Manager.Application.Exceptions;

public class FacilityNotfoundException : AppNotfoundException
{
    public override ErrorCode ErrorCode => ErrorCode.E1011;
    public override string Title => ErrorCode.GetEnumDescriptions();
}
