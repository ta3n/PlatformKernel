using Liberty.ApplicationShared.Extensions;
using Liberty.Reservation.Application.Utils;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.Facility;

namespace Liberty.Reservation.Manager.WebAPI.Application.Validations;

public class FacilityUpdateClassificationCommandValidator : AbstractValidator<FacilityUpdateClassificationCommand>
{
    public FacilityUpdateClassificationCommandValidator()
    {
        RuleFor(x => x.Payload.Allergens)
            .Cascade(CascadeMode.Stop)
            .Must(x => x is null || x.TrueForAll(t => t > 0))
            .WithErrorCode(ErrorCode.E0010)
            .WithMessage(ErrorCode.E0010.GetEnumDescriptions());

        RuleFor(x => x.Payload.Features)
            .Cascade(CascadeMode.Stop)
            .Must(x => x is null || x.TrueForAll(t => t > 0))
            .WithErrorCode(ErrorCode.E0010)
            .WithMessage(ErrorCode.E0010.GetEnumDescriptions());

        RuleFor(x => x.Payload.Equipments)
            .Cascade(CascadeMode.Stop)
            .Must(x => x is null || x.TrueForAll(t => t > 0))
            .WithErrorCode(ErrorCode.E0010)
            .WithMessage(ErrorCode.E0010.GetEnumDescriptions());

        RuleFor(x => x.Payload.Services)
            .Cascade(CascadeMode.Stop)
            .Must(x => x is null || x.TrueForAll(t => t > 0))
            .WithErrorCode(ErrorCode.E0010)
            .WithMessage(ErrorCode.E0010.GetEnumDescriptions());

        RuleFor(x => x.Payload.Baths)
            .Cascade(CascadeMode.Stop)
            .Must(x => x is null || x.TrueForAll(t => t > 0))
            .WithErrorCode(ErrorCode.E0010)
            .WithMessage(ErrorCode.E0010.GetEnumDescriptions());

        RuleFor(x => x.Payload.Sceneries)
            .Cascade(CascadeMode.Stop)
            .Must(x => x is null || x.TrueForAll(t => t > 0))
            .WithErrorCode(ErrorCode.E0010)
            .WithMessage(ErrorCode.E0010.GetEnumDescriptions());

        RuleFor(x => x.Payload.Amenities)
            .Cascade(CascadeMode.Stop)
            .Must(x => x is null || x.TrueForAll(t => t > 0))
            .WithErrorCode(ErrorCode.E0010)
            .WithMessage(ErrorCode.E0010.GetEnumDescriptions());

        RuleFor(x => x.Payload.Meals)
            .Cascade(CascadeMode.Stop)
            .Must(x => x is null || x.TrueForAll(t => t > 0))
            .WithErrorCode(ErrorCode.E0010)
            .WithMessage(ErrorCode.E0010.GetEnumDescriptions());
    }
}
