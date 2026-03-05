using FluentValidation;
using Liberty.ApplicationShared.Extensions;
using Liberty.ApplicationShared.FluentValidationUtilities;
using Liberty.Reservation.Application.Utils;
using Liberty.Reservation.User.WebAPI.Application.UserCases.Commands.BookingReservation;
using Liberty.SysException;

namespace Liberty.Reservation.User.WebAPI.Application.Validations;

public class ChangePersonsBookingCommandValidator : AbstractValidator<ChangePersonsBookingCommand>
{
    public ChangePersonsBookingCommandValidator()
    {
        RuleFor(x => x.Payload.GuestsPerRoom)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .NotEmpty()
            .WithErrorCode(ErrorCode.E0001);

        RuleFor(x => x.Payload.RestNumber)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .NotEmpty()
            .WithErrorCode(ErrorCode.E0001)
            .GreaterThan(0)
            .WithErrorCode(ErrorCode.E0001);

        RuleFor(x => x.Payload.RoomNumber)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .NotEmpty()
            .WithErrorCode(ErrorCode.E0001)
            .GreaterThan(0)
            .WithErrorCode(ErrorCode.E0001);

        RuleFor(x => x.Payload.CheckInDate)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .NotEmpty()
            .WithErrorCode(ErrorCode.E0001)
            .GreaterThan(0)
            .WithErrorCode(ErrorCode.E0001)
            .Must(ValidDate.BeValidDate)
            .WithErrorCode(ErrorCode.E0008)
            .WithMessage(ErrorCode.E0008.GetEnumDescriptions());
    }
}
