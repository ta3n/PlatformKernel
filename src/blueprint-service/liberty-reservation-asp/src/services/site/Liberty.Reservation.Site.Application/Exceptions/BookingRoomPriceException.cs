using Liberty.ApplicationShared.Extensions;
using Liberty.SysException.Exceptions;

namespace Liberty.Reservation.Site.Application.Exceptions;

public class BookingRoomPriceException(
    long roomId,
    long dateStay
) : AppNotfoundException
{
    public override ErrorCode ErrorCode => ErrorCode.E2040;

    public override string Title => string.Format(
        ErrorCode.GetEnumDescriptions(),
        roomId,
        dateStay
    );
}
