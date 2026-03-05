using Liberty.Reservation.Application.Utils;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.CancellationPolicy;

namespace Liberty.Reservation.Manager.WebAPI.Application.Validations;

public class CancellationPolicyUpdateCommandValidator : AbstractValidator<CancellationPolicyUpdateCommand>
{
    public CancellationPolicyUpdateCommandValidator()
    {
        RuleFor(x => x.Payload.Id)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .GreaterThan(0)
            .WithErrorCode(ErrorCode.E0001);

        RuleFor(x => x.Payload.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithErrorCode(ErrorCode.E0001)
            .MaximumLength(250)
            .WithErrorCode(ErrorCode.E0002);

        RuleFor(x => x.Payload.Description)
            .Cascade(CascadeMode.Stop)
            .MaximumLength(500)
            .WithErrorCode(ErrorCode.E0002);

        RuleForEach(x => x.Payload.Data)
            .Cascade(CascadeMode.Stop)
            .SetValidator(new DataOfCancellationUpdateRequestValidator());
    }
}

public class DataOfCancellationUpdateRequestValidator : AbstractValidator<DataOfCancellationUpdateRequest>
{
    public DataOfCancellationUpdateRequestValidator()
    {
        RuleFor(x => x.Id)
            .Cascade(CascadeMode.Stop)
            .GreaterThan(0)
            .WithErrorCode(ErrorCode.E0001)
            .When(x => x.Id is not null);

        RuleFor(x => x.DayStart)
            .Cascade(CascadeMode.Stop)
            .GreaterThanOrEqualTo(0)
            .WithErrorCode(ErrorCode.E0001);

        RuleFor(x => x.DayEnd)
            .Cascade(CascadeMode.Stop)
            .GreaterThanOrEqualTo(x => x.DayStart)
            .WithErrorCode(ErrorCode.E0015);

        RuleFor(x => x.Rate)
            .Cascade(CascadeMode.Stop)
            .GreaterThanOrEqualTo(0)
            .WithErrorCode(ErrorCode.E0011)
            .LessThanOrEqualTo(100)
            .WithErrorCode(ErrorCode.E0004);
    }
}
