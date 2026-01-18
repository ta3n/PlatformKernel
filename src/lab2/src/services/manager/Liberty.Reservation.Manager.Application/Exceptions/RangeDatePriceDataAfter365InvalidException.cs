using Liberty.ApplicationShared.Extensions;

namespace Liberty.Reservation.Manager.Application.Exceptions;

public class RangeDatePriceDataAfter365InvalidException(
    string fieldName
) : AppInvalidException
{
    public override ErrorCode ErrorCode => ErrorCode.E5004;
    public override string Title => string.Format(ErrorCode.GetEnumDescriptions(), fieldName);
}
