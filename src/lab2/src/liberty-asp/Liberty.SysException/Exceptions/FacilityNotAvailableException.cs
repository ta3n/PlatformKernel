using Liberty.ApplicationShared.Extensions;

namespace Liberty.SysException.Exceptions;

public class FacilityNotAvailableException : AppNotfoundException
{
    public override ErrorCode ErrorCode => ErrorCode.E1001;
    public override string Title => ErrorCode.GetEnumDescriptions();
}
