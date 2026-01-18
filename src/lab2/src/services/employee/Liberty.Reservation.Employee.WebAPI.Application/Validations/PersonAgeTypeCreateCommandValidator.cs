using Liberty.ApplicationShared.Extensions;
using Liberty.Reservation.Application.Utils;
using Liberty.Reservation.Employee.WebAPI.Application.UserCases.Commands.PersonAgeType;
using Liberty.SysException;

namespace Liberty.Reservation.Employee.WebAPI.Application.Validations;

public class PersonAgeTypeCreateCommandValidator : AbstractValidator<PersonAgeTypeCreateCommand>
{
    public PersonAgeTypeCreateCommandValidator()
    {
        RuleFor(x => x.Payload)
            .Cascade(CascadeMode.Stop)
            .SetValidator(new PersonAgeTypeCreateRequestValidator());
    }
}

public class PersonAgeTypeCreateRequestValidator : AbstractValidator<PersonAgeTypeCreateRequest>
{
    public PersonAgeTypeCreateRequestValidator()
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
            .NotNull()
            .WithErrorCode(ErrorCode.E0001);

        RuleFor(x => x.Meta.GroupName)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001);

        RuleFor(x => x.Meta.PersonAgeGroup)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001);

        RuleFor(x => x.Spas)
            .Cascade(CascadeMode.Stop)
            .Must(spas => spas is { Count: <= 4 })
            .WithErrorCode(ErrorCode.E1024)
            .WithMessage(ErrorCode.E1024.GetEnumDescriptions());

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
            .WithErrorCode(ErrorCode.E1026)
            .WithMessage(ErrorCode.E1026.GetEnumDescriptions());

        RuleFor(p => p.Spas)
            .Cascade(CascadeMode.Stop)
            .Must(
                spa =>
                    spa!.Select(x => new { x.PriceMax })
                        .Distinct()
                        .Count()
                    == spa!.Count
            )
            .WithErrorCode(ErrorCode.E1027)
            .WithMessage(ErrorCode.E1027.GetEnumDescriptions());
    }
}
