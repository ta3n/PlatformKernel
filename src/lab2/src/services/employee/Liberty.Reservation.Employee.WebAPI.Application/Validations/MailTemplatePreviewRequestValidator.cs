using Liberty.Reservation.Application.Utils;
using Liberty.SysException;

namespace Liberty.Reservation.Employee.WebAPI.Application.Validations;

public class MailTemplatePreviewRequestValidator : AbstractValidator<MailTemplatePreviewRequest>
{
    public MailTemplatePreviewRequestValidator()
    {
        RuleFor(x => x.Format)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .MaximumLength(5000)
            .WithErrorCode(ErrorCode.E0002);

        RuleFor(x => x.IoType)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .NotEmpty()
            .WithErrorCode(ErrorCode.E0001)
            .MaximumLength(20)
            .WithErrorCode(ErrorCode.E0002);
    }
}
