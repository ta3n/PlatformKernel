using Liberty.ApplicationShared.Extensions;
using Liberty.SysException;

namespace Liberty.GmoPaymentGateway.Exceptions;

public class OnlinePaymentChangeOrderException(
    string? orderId
) : PaymentProviderException
{
    public override ErrorCode ErrorCode => ErrorCode.E3006;

    public override string Title => ErrorCode.GetEnumDescriptions();

    public override string ApproachMessage => "";

    public override string ErrorCaption => orderId ?? string.Empty;
}
