using Liberty.ApplicationShared.Extensions;
using Liberty.ApplicationShared.FluentValidationUtilities;
using Liberty.Reservation.Application.Utils;

namespace Liberty.Reservation.Manager.WebAPI.Application.Validations;

public class PlanUpdatePublishAcceptCommandValidator : AbstractValidator<PlanUpdatePublishAcceptCommand>
{
    public PlanUpdatePublishAcceptCommandValidator()
    {
        RuleFor(x => x.Id)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .GreaterThan(0)
            .WithErrorCode(ErrorCode.E0001);

        RuleFor(x => x.Payload.Sites)
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
            .WithName(nameof(PlanUpdatePublishAcceptRequest.DisplayDateEnd))
            .WithErrorCode(ErrorCode.E1018)
            .WithMessage(ErrorCode.E1018.GetEnumDescriptions());

        RuleFor(x => x.Payload)
            .Cascade(CascadeMode.Stop)
            .Must(BeValidUseAcceptDateRange)
            .WithName(nameof(PlanUpdatePublishAcceptRequest.AcceptDateEnd))
            .WithErrorCode(ErrorCode.E1019)
            .WithMessage(ErrorCode.E1019.GetEnumDescriptions());

        RuleFor(x => x.Payload)
            .Cascade(CascadeMode.Stop)
            .Must(BeValidUseBookingReception)
            .WithName(nameof(PlanUpdatePublishAcceptRequest.BookingReceptionEnd))
            .WithErrorCode(ErrorCode.E1058)
            .WithMessage(ErrorCode.E1058.GetEnumDescriptions());

        RuleFor(x => x.Payload.ReceptionDayLimit)
            .Cascade(CascadeMode.Stop)
            .GreaterThanOrEqualTo(0)
            .WithErrorCode(ErrorCode.E1059)
            .WithMessage(ErrorCode.E1059.GetEnumDescriptions())
            .LessThanOrEqualTo(60)
            .WithErrorCode(ErrorCode.E1059)
            .WithMessage(ErrorCode.E1059.GetEnumDescriptions())
            .When(x => x.Payload.ReceptionDayLimit is not null);
    }

    private static bool BeValidUseDisplayDateRange(
        PlanUpdatePublishAcceptRequest request
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
        PlanUpdatePublishAcceptRequest request
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

    private static bool BeValidUseBookingReception(
        PlanUpdatePublishAcceptRequest request
    )
    {
        return request.UseBookingReception is true
            && request.BookingReceptionStart is not null
            && request.BookingReceptionEnd is not null
            && request.BookingReceptionStart <= request.BookingReceptionEnd;
    }
}
