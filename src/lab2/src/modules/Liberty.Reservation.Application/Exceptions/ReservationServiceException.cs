using Liberty.ApplicationShared.Extensions;
using Liberty.SysException;
using Liberty.SysException.Exceptions;

namespace Liberty.Reservation.Application.Exceptions;

public abstract class ReservationServiceException : AppInvalidException
{
    public override ErrorCode ErrorCode => ErrorCode.E2004;

    public override string ErrorCaption => ErrorCode.GetEnumDescriptions();

    public override string ApproachMessage => "";

    /// <summary>いつの予約日のエラーか判別するため</summary>
    public long AppDateId { get; set; }

    /// <summary>何連泊目のエラーか判別するため</summary>
    public int RestIndex { get; set; }
}
