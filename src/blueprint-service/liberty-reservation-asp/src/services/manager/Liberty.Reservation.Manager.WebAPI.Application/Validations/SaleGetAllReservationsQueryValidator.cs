using Liberty.ApplicationShared.Extensions;
using Liberty.ApplicationShared.FluentValidationUtilities;
using Liberty.Reservation.Application.Utils;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.Sale;

namespace Liberty.Reservation.Manager.WebAPI.Application.Validations;

public class SaleGetAllReservationsQueryValidator : AbstractValidator<SaleGetAllReservationsQuery>
{
    public SaleGetAllReservationsQueryValidator()
    {
        RuleFor(x => x.StartAppDateId)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .GreaterThan(0)
            .WithErrorCode(ErrorCode.E0001);

        RuleFor(x => x.StartAppDateId)
            .Cascade(CascadeMode.Stop)
            .Must(ValidDate.BeValidDate)
            .WithErrorCode(ErrorCode.E0008)
            .WithMessage(ErrorCode.E0008.GetEnumDescriptions());

        RuleFor(x => x.EndAppDateId)
            .Cascade(CascadeMode.Stop)
            .Must(ValidDate.BeValidDate)
            .WithErrorCode(ErrorCode.E0008)
            .WithMessage(ErrorCode.E0008.GetEnumDescriptions());

        RuleFor(x => x.EndAppDateId)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .GreaterThanOrEqualTo(x => x.StartAppDateId)
            .WithErrorCode(ErrorCode.E0009);
    }
}
