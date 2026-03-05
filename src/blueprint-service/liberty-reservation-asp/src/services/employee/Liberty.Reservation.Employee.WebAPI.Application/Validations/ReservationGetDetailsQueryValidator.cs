using Liberty.Reservation.Application.Utils;
using Liberty.Reservation.Employee.WebAPI.Application.UserCases.Queries.BookingReservation;
using Liberty.SysException;

namespace Liberty.Reservation.Employee.WebAPI.Application.Validations;

public class ReservationGetDetailsQueryValidator : AbstractValidator<ReservationGetDetailsQuery>
{
    public ReservationGetDetailsQueryValidator()
    {
        RuleFor(x => x.Id)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .GreaterThan(0)
            .WithErrorCode(ErrorCode.E0001);
    }
}
