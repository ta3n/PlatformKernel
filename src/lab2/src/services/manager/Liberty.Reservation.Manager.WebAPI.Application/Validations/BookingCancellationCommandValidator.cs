using Liberty.Reservation.Application.Utils;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.BookingReservation;

namespace Liberty.Reservation.Manager.WebAPI.Application.Validations;

public class BookingCancellationCommandValidator : AbstractValidator<BookingCancellationCommand>
{
    public BookingCancellationCommandValidator()
    {
        RuleFor(x => x.Payload.Id)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .GreaterThan(0)
            .WithErrorCode(ErrorCode.E0001);
    }
}
