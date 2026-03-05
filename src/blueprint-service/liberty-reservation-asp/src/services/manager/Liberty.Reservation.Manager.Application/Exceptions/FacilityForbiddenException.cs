namespace Liberty.Reservation.Manager.Application.Exceptions;

public class FacilityForbiddenException : AppForbiddenException
{
    public override ErrorCode ErrorCode => ErrorCode.E1011;
    public override string Title => "この操作を実行する権限がありません。必要な権限を確認してください。";
}
