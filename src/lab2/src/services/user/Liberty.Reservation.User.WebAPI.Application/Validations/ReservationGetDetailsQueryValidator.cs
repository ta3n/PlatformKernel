using FluentValidation;
using Liberty.Reservation.Application.Utils;
using Liberty.Reservation.User.WebAPI.Application.UserCases.Queries.BookingReservation;
using Liberty.SysException;

namespace Liberty.Reservation.User.WebAPI.Application.Validations;

public class ReservationGetDetailsQueryValidator : AbstractValidator<ReservationGetDetailsQuery>
{
    public ReservationGetDetailsQueryValidator()
    {
        RuleFor(x => x.Id)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .NotEmpty()
            .WithErrorCode(ErrorCode.E0001)
            .GreaterThan(0)
            .WithErrorCode(ErrorCode.E0001);
    }
}
