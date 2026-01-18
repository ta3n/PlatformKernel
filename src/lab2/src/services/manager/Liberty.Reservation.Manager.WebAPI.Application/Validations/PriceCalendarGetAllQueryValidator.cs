using Liberty.ApplicationShared.Extensions;
using Liberty.ApplicationShared.FluentValidationUtilities;
using Liberty.Reservation.Application.Utils;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.PriceCalendar;

namespace Liberty.Reservation.Manager.WebAPI.Application.Validations;

public class PriceCalendarGetAllQueryValidator : AbstractValidator<PriceCalendarGetAllQuery>
{
    public PriceCalendarGetAllQueryValidator()
    {
        RuleFor(x => x.StartDate)
            .Cascade(CascadeMode.Stop)
            .GreaterThan(0)
            .WithErrorCode(ErrorCode.E0003)
            .Must(ValidDate.BeValidDate)
            .WithErrorCode(ErrorCode.E0008)
            .WithMessage(ErrorCode.E0008.GetEnumDescriptions());

        RuleFor(x => x.EndDate)
            .Cascade(CascadeMode.Stop)
            .GreaterThanOrEqualTo(x => x.StartDate)
            .WithErrorCode(ErrorCode.E0009)
            .Must(ValidDate.BeValidDate)
            .WithErrorCode(ErrorCode.E0008)
            .WithMessage(ErrorCode.E0008.GetEnumDescriptions());
    }
}
