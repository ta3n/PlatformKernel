using Liberty.Reservation.Application.Utils;
using Liberty.Reservation.Site.File.WebAPI.Application.UserCases.Queries.Media;
using Liberty.SysException;

namespace Liberty.Reservation.Site.File.WebAPI.Application.Validations;

public class ImageGetQueryValidator : AbstractValidator<ImageGetQuery>
{
    public ImageGetQueryValidator()
    {
        RuleFor(x => x.Payload.Code)
            .NotNull()
            .NotEmpty()
            .WithErrorCode(ErrorCode.E0001);
    }
}
