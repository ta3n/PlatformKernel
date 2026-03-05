using Liberty.Reservation.Application.Utils;

namespace Liberty.Reservation.Manager.WebAPI.Application.Validations;

public class QuestionCreateRequestValidator : AbstractValidator<QuestionCreateRequest>
{
    public QuestionCreateRequestValidator()
    {
        RuleFor(x => x.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithErrorCode(ErrorCode.E0001)
            .MaximumLength(250)
            .WithErrorCode(ErrorCode.E0002);
    }
}
