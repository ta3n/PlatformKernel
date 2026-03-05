using Liberty.Reservation.Application.Utils;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.BookingReservation;

namespace Liberty.Reservation.Manager.WebAPI.Application.Validations;

public class BookingNoShowCommandValidator : AbstractValidator<BookingNoShowCommand>
{
    public BookingNoShowCommandValidator()
    {
        RuleFor(x => x.Payload.Id)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .GreaterThan(0)
            .WithErrorCode(ErrorCode.E0001);

        RuleFor(x => x.Payload.Reason)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithErrorCode(ErrorCode.E0001)
            .MaximumLength(500)
            .WithErrorCode(ErrorCode.E0002);
    }
}
