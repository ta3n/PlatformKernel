using Liberty.ApplicationShared.Extensions;
using Liberty.SysException;
using Liberty.SysException.Exceptions;

namespace Liberty.GmoPaymentGateway.Exceptions;

public class OnlinePaymentUpdateChangeAmountException(
    string? orderId
) : AppException
{
    public override ErrorCode ErrorCode => ErrorCode.E3008;
    public override string Title => ErrorCode.GetEnumDescriptions();

    public override string ErrorCaption => orderId ?? string.Empty;
}
