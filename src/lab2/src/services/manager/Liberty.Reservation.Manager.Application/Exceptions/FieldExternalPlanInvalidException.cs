using Liberty.ApplicationShared.Extensions;

namespace Liberty.Reservation.Manager.Application.Exceptions;

public class FieldExternalPlanInvalidException(
    string fieldName,
    string fieldValue = "[Blank]"
) : AppInvalidException
{
    public override ErrorCode ErrorCode => ErrorCode.E5003;
    public override string Title => string.Format(ErrorCode.GetEnumDescriptions(), fieldName, fieldValue);
}
