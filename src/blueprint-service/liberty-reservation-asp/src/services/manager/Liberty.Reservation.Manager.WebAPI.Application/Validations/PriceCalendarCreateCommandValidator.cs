using Liberty.ApplicationShared.Extensions;
using Liberty.ApplicationShared.FluentValidationUtilities;
using Liberty.Reservation.Application.Utils;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.PriceCalendar;

namespace Liberty.Reservation.Manager.WebAPI.Application.Validations;

public class PriceCalendarCreateCommandValidator : AbstractValidator<PriceCalendarCreateCommand>
{
    public PriceCalendarCreateCommandValidator()
    {
        RuleForEach(x => x.Payload.Calendars)
            .Cascade(CascadeMode.Stop)
            .ChildRules(
                calendar =>
                {
                    calendar.RuleFor(x => x.PriceTypeId)
                        .Cascade(CascadeMode.Stop)
                        .NotNull()
                        .WithErrorCode(ErrorCode.E0001)
                        .GreaterThan(0)
                        .WithErrorCode(ErrorCode.E0003);

                    calendar.RuleFor(x => (long)x.DateCalendar)
                        .Cascade(CascadeMode.Stop)
                        .GreaterThan(0)
                        .WithErrorCode(ErrorCode.E0003)
                        .Must(ValidDate.BeValidDate)
                        .WithErrorCode(ErrorCode.E0008)
                        .WithMessage(ErrorCode.E0008.GetEnumDescriptions());
                }
            );
    }
}
