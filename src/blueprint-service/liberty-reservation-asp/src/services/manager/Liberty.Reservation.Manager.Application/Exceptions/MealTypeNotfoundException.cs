using Liberty.ApplicationShared.Extensions;

namespace Liberty.Reservation.Manager.Application.Exceptions;

public class MealTypeNotfoundException : AppNotfoundException
{
    public override ErrorCode ErrorCode => ErrorCode.E1037;
    public override string Title => ErrorCode.GetEnumDescriptions();
}
