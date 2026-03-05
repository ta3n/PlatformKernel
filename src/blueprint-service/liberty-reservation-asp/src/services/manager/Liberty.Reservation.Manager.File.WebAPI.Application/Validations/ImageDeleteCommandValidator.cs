using Liberty.Reservation.Application.Utils;
using Liberty.Reservation.Manager.File.WebAPI.Application.UserCases.Commands.Image;
using Liberty.SysException;

namespace Liberty.Reservation.Manager.File.WebAPI.Application.Validations;

public class ImageDeleteCommandValidator : AbstractValidator<ImageDeleteCommand>
{
    public ImageDeleteCommandValidator()
    {
        RuleFor(x => x.Payload.Id)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .NotEmpty()
            .WithErrorCode(ErrorCode.E0001)
            .GreaterThan(0)
            .WithErrorCode(ErrorCode.E0001);
    }
}
