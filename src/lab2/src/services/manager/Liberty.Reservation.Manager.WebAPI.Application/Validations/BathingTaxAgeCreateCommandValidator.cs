using Liberty.Reservation.Application.Utils;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.BathingTaxAge;

namespace Liberty.Reservation.Manager.WebAPI.Application.Validations;

public class BathingTaxAgeCreateCommandValidator : AbstractValidator<BathingTaxAgeCreateCommand>
{
    public BathingTaxAgeCreateCommandValidator()
    {
        RuleFor(x => x.Payload)
            .Cascade(CascadeMode.Stop)
            .SetValidator(new BathingTaxAgeDetailsCreateRequestValidator());
    }
}

public class BathingTaxAgeDetailsCreateRequestValidator : AbstractValidator<BathingTaxAgeCreateRequest>
{
    public BathingTaxAgeDetailsCreateRequestValidator()
    {
        RuleFor(x => x.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithErrorCode(ErrorCode.E0001)
            .MaximumLength(255)
            .WithErrorCode(ErrorCode.E0002);

        RuleFor(x => x.AgeMin)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .GreaterThanOrEqualTo(0)
            .LessThanOrEqualTo(18);

        RuleFor(x => x.AgeMax)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .GreaterThanOrEqualTo(x => x.AgeMin)
            .LessThanOrEqualTo(999);

        RuleFor(x => x.Meta)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithErrorCode(ErrorCode.E0001);

        RuleFor(x => x.Spas)
            .Cascade(CascadeMode.Stop)
            .Must(spas => spas is { Count: <= 4 })
            .WithMessage("The object of Spas must be less than or equal to 4.");

        RuleForEach(p => p.Spas)
            .Cascade(CascadeMode.Stop)
            .ChildRules(
                child =>
                {
                    child.RuleFor(x => x.PriceMin).GreaterThanOrEqualTo(0);
                    child.RuleFor(x => x.PriceMax).GreaterThan(0);
                    child.RuleFor(x => x.PriceMax).GreaterThanOrEqualTo(x => x.PriceMin);
                }
            );

        RuleFor(p => p.Spas)
            .Cascade(CascadeMode.Stop)
            .Must(
                spa =>
                    spa!.Select(x => new { x.PriceMin })
                        .Distinct()
                        .Count()
                    == spa!.Count
            )
            .WithMessage(
                obj => $"Duplicate priceMin in Spas {obj.Name}"
            );

        RuleFor(p => p.Spas)
            .Cascade(CascadeMode.Stop)
            .Must(
                spa =>
                    spa!.Select(x => new { x.PriceMax })
                        .Distinct()
                        .Count()
                    == spa!.Count
            )
            .WithMessage(
                obj => $"Duplicate priceMax in Spas {obj.Name}"
            );
    }
}
