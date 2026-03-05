using Liberty.ApplicationShared.Extensions;

namespace Liberty.Reservation.Manager.Application.Exceptions;

public class FacilityRoomGroupDuplicateGroupNameException(
    long facilityId,
    string groupName
) : AppException
{
    public override ErrorCode ErrorCode => ErrorCode.E1036;

    public override string Title => string.Format(
        ErrorCode.GetEnumDescriptions(),
        groupName,
        facilityId
    );
}
