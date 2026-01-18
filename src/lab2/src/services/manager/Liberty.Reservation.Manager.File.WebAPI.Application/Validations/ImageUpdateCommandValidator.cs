using Liberty.Reservation.Application.Utils;
using Liberty.Reservation.Manager.File.WebAPI.Application.UserCases.Commands.Image;
using Liberty.SysException;

namespace Liberty.Reservation.Manager.File.WebAPI.Application.Validations;

public class ImageUpdateCommandValidator : AbstractValidator<ImageUpdateCommand>
{
    public ImageUpdateCommandValidator()
    {
        RuleFor(x => x.Payload.Id)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .NotEmpty()
            .WithErrorCode(ErrorCode.E0001)
            .GreaterThan(0)
            .WithErrorCode(ErrorCode.E0001);

        RuleFor(x => x.Payload.ImageCategoryIds)
            .Cascade(CascadeMode.Stop)
            .Must(x => x is null || x.TrueForAll(t => t > 0))
            .WithErrorCode(ErrorCode.E0010);

        RuleFor(x => x.Payload.MasterCategoryIds)
            .Cascade(CascadeMode.Stop)
            .Must(x => x is null || x.TrueForAll(t => t > 0))
            .WithErrorCode(ErrorCode.E0010);
    }
}
