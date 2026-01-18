using Liberty.ApplicationShared.Extensions;

namespace Liberty.Reservation.Manager.Application.Exceptions;

public class RangeDatePriceDataInvalidException(
    string fieldName
) : AppInvalidException
{
    public override ErrorCode ErrorCode => ErrorCode.E5001;
    public override string Title => string.Format(ErrorCode.GetEnumDescriptions(), fieldName);
}
