using Liberty.ApplicationShared.Extensions;

namespace Liberty.Reservation.Manager.Application.Exceptions;

public class QuestionNotfoundException : AppNotfoundException
{
    public override ErrorCode ErrorCode => ErrorCode.E1042;
    public override string Title => ErrorCode.GetEnumDescriptions();
}
