using Liberty.ApplicationShared.Extensions;
using Liberty.SysException;

namespace Liberty.GmoPaymentGateway.Exceptions;

public class PaymentGetUrlException(
    string orderId,
    string error
) : PaymentProviderException
{
    public override ErrorCode ErrorCode => ErrorCode.E3005;

    public override string Title => string.Format(
        ErrorCode.GetEnumDescriptions(),
        orderId,
        error
    );

    public override string ApproachMessage => "";
}
