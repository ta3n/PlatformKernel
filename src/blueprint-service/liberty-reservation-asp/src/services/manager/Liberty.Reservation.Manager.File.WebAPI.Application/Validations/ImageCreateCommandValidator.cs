using Liberty.ApplicationShared.Extensions;
using Liberty.Reservation.Application.Utils;
using Liberty.Reservation.Manager.File.WebAPI.Application.UserCases.Commands.Image;
using Liberty.SysException;

namespace Liberty.Reservation.Manager.File.WebAPI.Application.Validations;

public class ImageCreateCommandValidator : AbstractValidator<ImageCreateCommand>
{
    public ImageCreateCommandValidator()
    {
        RuleFor(x => x.Payload.ImageCategoryIds)
            .Cascade(CascadeMode.Stop)
            .Must(x => x is null || x.TrueForAll(t => t > 0))
            .WithErrorCode(ErrorCode.E0010)
            .WithMessage(ErrorCode.E0010.GetEnumDescriptions());
    }
}
