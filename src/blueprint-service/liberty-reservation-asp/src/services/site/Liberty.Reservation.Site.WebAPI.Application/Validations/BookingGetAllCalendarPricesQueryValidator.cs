using Liberty.ApplicationShared.Extensions;
using Liberty.Reservation.Application.Utils;
using Liberty.Reservation.Site.WebAPI.Application.UserCases.Queries.Booking;
using Liberty.SysException;

namespace Liberty.Reservation.Site.WebAPI.Application.Validations;

public class BookingGetAllCalendarPricesQueryValidator : AbstractValidator<BookingGetCalendarPricesQuery>
{
    public BookingGetAllCalendarPricesQueryValidator()
    {
        RuleFor(x => x.PlanId)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .GreaterThan(0)
            .WithErrorCode(ErrorCode.E0001);

        RuleFor(x => x.RoomGroupId)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .GreaterThan(0)
            .WithErrorCode(ErrorCode.E0001);

        RuleFor(x => x.Payload.GuestsPerRoom)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithErrorCode(ErrorCode.E0001);

        RuleFor(x => x.Payload.RestNumber)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .GreaterThan(0)
            .WithErrorCode(ErrorCode.E0001);

        RuleFor(x => x.Payload.RoomNumber)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .GreaterThan(0)
            .WithErrorCode(ErrorCode.E0001);

        RuleFor(x => x.Payload.CheckInDate)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .GreaterThan(0)
            .WithErrorCode(ErrorCode.E0001)
            .Must(ValidDate.BeValidDate)
            .WithErrorCode(ErrorCode.E0008)
            .WithMessage(ErrorCode.E0008.GetEnumDescriptions());

        RuleFor(x => x.Payload.CheckOutDate)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .GreaterThan(0)
            .WithErrorCode(ErrorCode.E0001)
            .Must(ValidDate.BeValidDate)
            .WithErrorCode(ErrorCode.E0008)
            .WithMessage(ErrorCode.E0008.GetEnumDescriptions())
            .GreaterThan(x => x.Payload.CheckInDate)
            .WithErrorCode(ErrorCode.E2042);

        RuleFor(x => x.Payload.MinPrice)
            .Cascade(CascadeMode.Stop)
            .GreaterThanOrEqualTo(0)
            .WithErrorCode(ErrorCode.E0011);

        RuleFor(x => x.Payload.MaxPrice)
            .Cascade(CascadeMode.Stop)
            .GreaterThanOrEqualTo(0)
            .WithErrorCode(ErrorCode.E0011)
            .GreaterThanOrEqualTo(x => x.Payload.MinPrice)
            .WithErrorCode(ErrorCode.E2043)
            .When(x => x.Payload.MinPrice is not null);

        RuleForEach(x => x.Payload.GuestsPerRoom)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithErrorCode(ErrorCode.E0001)
            .SetValidator(new PersonOfBookingSearchRequestValidator());
    }
}
