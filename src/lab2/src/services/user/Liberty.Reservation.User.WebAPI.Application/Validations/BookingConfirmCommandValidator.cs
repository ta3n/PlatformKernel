using FluentValidation;
using Liberty.Reservation.Application.Utils;
using Liberty.Reservation.User.WebAPI.Application.UserCases.Commands.BookingReservation;
using Liberty.SysException;

namespace Liberty.Reservation.User.WebAPI.Application.Validations;

public class BookingConfirmCommandValidator : AbstractValidator<BookingConfirmCommand>
{
    public BookingConfirmCommandValidator()
    {
        RuleFor(x => x.Payload.Id)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .NotEmpty()
            .WithErrorCode(ErrorCode.E0001)
            .GreaterThan(0)
            .WithErrorCode(ErrorCode.E0001);
    }
}
