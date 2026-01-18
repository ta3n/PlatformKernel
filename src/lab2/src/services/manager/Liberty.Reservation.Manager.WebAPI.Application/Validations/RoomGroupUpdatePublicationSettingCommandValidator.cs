using Liberty.ApplicationShared.Extensions;
using Liberty.ApplicationShared.FluentValidationUtilities;
using Liberty.Reservation.Application.Utils;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.RoomGroup;

namespace Liberty.Reservation.Manager.WebAPI.Application.Validations;

public class RoomGroupUpdatePublicationSettingCommandValidator
    : AbstractValidator<RoomGroupUpdatePublicationSettingCommand>
{
    public RoomGroupUpdatePublicationSettingCommandValidator()
    {
        RuleFor(x => x.Payload.Id)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .GreaterThan(0)
            .WithErrorCode(ErrorCode.E0001);

        RuleFor(x => x.Payload.SiteIds)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001);

        RuleFor(x => x.Payload.ReceptionLimit!)
            .Cascade(CascadeMode.Stop)
            .Must(ValidDate.BeAValidTimeSpan)
            .WithErrorCode(ErrorCode.E0012)
            .WithMessage(ErrorCode.E0012.GetEnumDescriptions());

        RuleFor(x => x.Payload.DisplayDateEnd)
            .Cascade(CascadeMode.Stop)
            .GreaterThanOrEqualTo(x => x.Payload.DisplayDateStart)
            .WithErrorCode(ErrorCode.E0009)
            .When(x => x.Payload.UseDisplayDate ?? false);

        RuleFor(x => x.Payload.AcceptDateEnd)
            .Cascade(CascadeMode.Stop)
            .GreaterThanOrEqualTo(x => x.Payload.AcceptDateStart)
            .WithErrorCode(ErrorCode.E0009)
            .When(x => x.Payload.UseAcceptDate ?? false);

        RuleFor(x => x.Payload)
            .Cascade(CascadeMode.Stop)
            .Must(BeValidUseDisplayDateRange)
            .WithName(nameof(RoomGroupUpdatePublicationSettingRequest.DisplayDateEnd))
            .WithErrorCode(ErrorCode.E1018)
            .WithMessage(ErrorCode.E1018.GetEnumDescriptions());

        RuleFor(x => x.Payload)
            .Cascade(CascadeMode.Stop)
            .Must(BeValidUseAcceptDateRange)
            .WithName(nameof(RoomGroupUpdatePublicationSettingRequest.AcceptDateEnd))
            .WithErrorCode(ErrorCode.E1019)
            .WithMessage(ErrorCode.E1019.GetEnumDescriptions());
    }

    private static bool BeValidUseDisplayDateRange(
        RoomGroupUpdatePublicationSettingRequest request
    )
    {
        if (request.UseDisplayDate is false)
        {
            return true;
        }

        return request.DisplayDateStart is not null
            && request.DisplayDateEnd is not null
            && request.DisplayDateStart <= request.DisplayDateEnd;
    }

    private static bool BeValidUseAcceptDateRange(
        RoomGroupUpdatePublicationSettingRequest request
    )
    {
        if (request.UseAcceptDate is false)
        {
            return true;
        }

        return request.AcceptDateStart is not null
            && request.AcceptDateEnd is not null
            && request.AcceptDateStart <= request.AcceptDateEnd;
    }
}
