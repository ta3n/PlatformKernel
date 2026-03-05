using Liberty.ApplicationShared.Extensions;

namespace Liberty.Reservation.Manager.Application.Exceptions;

public class RoomGroupInventoryInvalidException(
    long roomGroupId,
    int max
) : AppInvalidException
{
    public override ErrorCode ErrorCode => ErrorCode.E1043;

    public override string Title => ErrorCode.GetEnumDescriptions()
        .Replace("{roomGroupId}", roomGroupId.ToString())
        .Replace("{maxQuantity}", max.ToString());
}
