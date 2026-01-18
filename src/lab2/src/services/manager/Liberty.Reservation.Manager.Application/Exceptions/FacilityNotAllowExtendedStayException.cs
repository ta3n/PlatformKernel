using Liberty.ApplicationShared.Extensions;

namespace Liberty.Reservation.Manager.Application.Exceptions;

public class FacilityNotAllowExtendedStayException : AppInvalidException
{
    public override ErrorCode ErrorCode => ErrorCode.E1070;
    public override string Title => ErrorCode.GetEnumDescriptions();
}
