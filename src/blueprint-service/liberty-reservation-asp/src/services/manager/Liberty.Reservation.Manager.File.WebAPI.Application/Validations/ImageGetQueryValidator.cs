using Liberty.Reservation.Application.Utils;
using Liberty.Reservation.Manager.File.WebAPI.Application.UserCases.Queries.Image;
using Liberty.SysException;

namespace Liberty.Reservation.Manager.File.WebAPI.Application.Validations;

public class ImageGetQueryValidator : AbstractValidator<ImageGetQuery>
{
    public ImageGetQueryValidator()
    {
        RuleFor(x => x.Code)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .NotEmpty()
            .WithErrorCode(ErrorCode.E0001);
    }
}
