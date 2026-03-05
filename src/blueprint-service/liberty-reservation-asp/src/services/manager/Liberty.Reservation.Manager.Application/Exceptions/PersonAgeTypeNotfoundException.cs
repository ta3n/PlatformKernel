using Liberty.ApplicationShared.Extensions;

namespace Liberty.Reservation.Manager.Application.Exceptions;

public class PersonAgeTypeNotfoundException : AppNotfoundException
{
    public override ErrorCode ErrorCode => ErrorCode.E1039;
    public override string Title => ErrorCode.GetEnumDescriptions();
}
