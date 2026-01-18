using Liberty.ApplicationShared.Extensions;

namespace Liberty.Reservation.Manager.Application.Exceptions;

public class QuestionHasNoContentException : AppInvalidException
{
    public override ErrorCode ErrorCode => ErrorCode.E1061;
    public override string Title => ErrorCode.GetEnumDescriptions();
}
