using Liberty.ApplicationShared.Extensions;
using Liberty.SysException;

namespace Liberty.GmoPaymentGateway.Exceptions;

public class PaymentTransactionException(
    string accessId
) : PaymentProviderException
{
    public override ErrorCode ErrorCode => ErrorCode.E3003;

    public override string Title => string.Format(
        ErrorCode.GetEnumDescriptions(),
        accessId
    );

    public override string ApproachMessage => "";
}
