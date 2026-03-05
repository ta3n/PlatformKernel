using Liberty.Reservation.Application.Utils;

namespace Liberty.Reservation.Manager.WebAPI.Application.Validations;

public class PlanUpdateBasicSettingCommandValidator : AbstractValidator<PlanUpdateBasicSettingCommand>
{
    public PlanUpdateBasicSettingCommandValidator()
    {
        RuleFor(x => x.Payload.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithErrorCode(ErrorCode.E0001)
            .MaximumLength(200)
            .WithErrorCode(ErrorCode.E0002);

        RuleFor(x => x.Payload.NameForImport)
            .Cascade(CascadeMode.Stop)
            .MaximumLength(50)
            .WithErrorCode(ErrorCode.E0002);

        RuleFor(x => x.Payload.Summary)
            .Cascade(CascadeMode.Stop)
            .MaximumLength(10000)
            .WithErrorCode(ErrorCode.E0002);

        RuleFor(x => x.Payload.Description)
            .Cascade(CascadeMode.Stop)
            .MaximumLength(10000)
            .WithErrorCode(ErrorCode.E0002);

        RuleForEach(p => p.Payload.Files)
            .Cascade(CascadeMode.Stop)
            .ChildRules(
                child =>
                {
                    child.RuleFor(x => x.Id)
                        .Cascade(CascadeMode.Stop)
                        .NotNull()
                        .WithErrorCode(ErrorCode.E0001)
                        .GreaterThan(0)
                        .WithErrorCode(ErrorCode.E0003);

                    child.RuleFor(x => x.Index)
                        .Cascade(CascadeMode.Stop)
                        .NotNull()
                        .WithErrorCode(ErrorCode.E0001)
                        .GreaterThanOrEqualTo(0)
                        .WithErrorCode(ErrorCode.E0011);
                }
            );
    }
}
