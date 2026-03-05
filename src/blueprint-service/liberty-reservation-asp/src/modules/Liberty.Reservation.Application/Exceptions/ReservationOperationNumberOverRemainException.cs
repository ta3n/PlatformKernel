using Liberty.ApplicationShared.Extensions;
using Liberty.SysException;

namespace Liberty.Reservation.Application.Exceptions;

/// <summary>在庫数を超えていた場合の例外</summary>
public class ReservationOperationNumberOverRemainException : ReservationException
{
    public override ErrorCode ErrorCode => ErrorCode.E2026;

    public override string Title => ErrorCode.GetEnumDescriptions();
}
