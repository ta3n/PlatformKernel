using Liberty.ApplicationShared.Extensions;
using Liberty.Reservation.Application.Utils;
using Liberty.Reservation.Site.WebAPI.Application.UserCases.Commands.Booking;
using Liberty.SysException;

namespace Liberty.Reservation.Site.WebAPI.Application.Validations;

public class BookingCheckNightNumberCommandValidator : AbstractValidator<BookingCheckNightNumberCommand>
{
    public BookingCheckNightNumberCommandValidator()
    {
        RuleFor(x => x.PlanId)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .GreaterThan(0)
            .WithErrorCode(ErrorCode.E0001);

        RuleFor(x => x.RoomGroupId)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .GreaterThan(0)
            .WithErrorCode(ErrorCode.E0001);

        RuleFor(x => x.Payload.RestNumber)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .GreaterThan(0)
            .WithErrorCode(ErrorCode.E0001);

        RuleFor(x => x.Payload.CheckInDate)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .GreaterThan(0)
            .WithErrorCode(ErrorCode.E0001)
            .Must(ValidDate.BeValidDate)
            .WithErrorCode(ErrorCode.E0008)
            .WithMessage(ErrorCode.E0008.GetEnumDescriptions());
    }
}
