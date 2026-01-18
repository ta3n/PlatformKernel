using Liberty.ApplicationShared.Extensions;

namespace Liberty.Reservation.Manager.Application.Exceptions;

public class AllergenNotfoundException : AppNotfoundException
{
    public override ErrorCode ErrorCode => ErrorCode.E1029;
    public override string Title => ErrorCode.GetEnumDescriptions();
}
