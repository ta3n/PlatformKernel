using Liberty.Reservation.Application.Utils;
using Liberty.SysException;

namespace Liberty.Reservation.Employee.WebAPI.Application.Validations;

public class CategoryCreateRequestValidator : AbstractValidator<CategoryCreateRequest>
{
    public CategoryCreateRequestValidator()
    {
        RuleFor(x => x.Name)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .NotEmpty()
            .WithErrorCode(ErrorCode.E0001)
            .MaximumLength(250)
            .WithErrorCode(ErrorCode.E0002);

        RuleFor(x => x.Description)
            .Cascade(CascadeMode.Stop)
            .MaximumLength(500)
            .WithErrorCode(ErrorCode.E0002);
    }
}
