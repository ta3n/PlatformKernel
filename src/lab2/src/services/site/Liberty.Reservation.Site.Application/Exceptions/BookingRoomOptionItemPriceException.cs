using Liberty.ApplicationShared.Extensions;
using Liberty.SysException.Exceptions;

namespace Liberty.Reservation.Site.Application.Exceptions;

public class BookingRoomOptionItemPriceException(
    long roomId,
    long dateStay,
    long optionItemId
) : AppInvalidException
{
    public override ErrorCode ErrorCode => ErrorCode.E2039;

    public override string Title => string.Format(
        ErrorCode.GetEnumDescriptions(),
        optionItemId,
        roomId,
        dateStay
    );
}
