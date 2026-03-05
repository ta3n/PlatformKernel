using FluentValidation;
using Liberty.Reservation.Application.Utils;
using Liberty.Reservation.User.File.WebAPI.Application.UserCases.Queries.Media;
using Liberty.SysException;

namespace Liberty.Reservation.User.File.WebAPI.Application.Validations;

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
