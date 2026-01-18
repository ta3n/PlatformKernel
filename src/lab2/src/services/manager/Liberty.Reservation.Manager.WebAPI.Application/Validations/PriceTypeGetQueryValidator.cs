using Liberty.Reservation.Application.Utils;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.PriceType;

namespace Liberty.Reservation.Manager.WebAPI.Application.Validations;

public class PriceTypeGetQueryValidator : AbstractValidator<PriceTypeGetQuery>
{
    public PriceTypeGetQueryValidator()
    {
        RuleFor(x => x.Id)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .GreaterThan(0)
            .WithErrorCode(ErrorCode.E0001);
    }
}
