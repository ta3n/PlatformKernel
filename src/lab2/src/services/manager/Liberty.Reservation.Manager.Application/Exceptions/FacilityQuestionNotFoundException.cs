using Liberty.ApplicationShared.Extensions;

namespace Liberty.Reservation.Manager.Application.Exceptions;

public class FacilityQuestionNotFoundException : AppNotfoundException
{
    public override ErrorCode ErrorCode => ErrorCode.E1035;
    public override string Title => ErrorCode.GetEnumDescriptions();
}
