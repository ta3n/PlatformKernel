using Liberty.Reservation.Application.Utils;

namespace Liberty.Reservation.Manager.WebAPI.Application.Validations;

public class CategoryCreateRequestValidator : AbstractValidator<CategoryCreateRequest>
{
    public CategoryCreateRequestValidator()
    {
        RuleFor(x => x.Name)
            .Cascade(CascadeMode.Stop)
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
