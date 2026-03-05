namespace Liberty.Reservation.Manager.Application.Exceptions;

public class FacilityUnauthorizedException : AppAuthException
{
    public override ErrorCode ErrorCode => ErrorCode.E1011;
    public override string Title => "Unauthorized";
}
