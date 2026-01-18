using Liberty.ApplicationShared.Extensions;
using Liberty.SysException;

namespace Liberty.GmoPaymentGateway.Exceptions;

public class PaymentSearchTradeException(
    string orderId
) : PaymentProviderException
{
    public override ErrorCode ErrorCode => ErrorCode.E3001;

    public override string Title => string.Format(
        ErrorCode.GetEnumDescriptions(),
        orderId
    );

    public override string ApproachMessage => "";
}
