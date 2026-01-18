using Liberty.ApplicationShared.Extensions;

namespace Liberty.Reservation.Manager.Application.Exceptions;

public class PersonAgeTypeSpaTaxDataNotFoundException : AppNotfoundException
{
    public override ErrorCode ErrorCode => ErrorCode.E1040;

    public override string Title => ErrorCode.GetEnumDescriptions();
}
