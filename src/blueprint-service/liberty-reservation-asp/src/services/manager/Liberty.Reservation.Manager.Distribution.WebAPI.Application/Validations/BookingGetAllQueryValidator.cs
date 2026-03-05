using FluentValidation;
using Liberty.ApplicationShared.Extensions;
using Liberty.ApplicationShared.FluentValidationUtilities;
using Liberty.Reservation.Application.Utils;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.UserCases.Queries.Rss;
using Liberty.SysException;

namespace Liberty.Reservation.Manager.Distribution.WebAPI.Application.Validations;

public class BookingGetAllQueryValidator : AbstractValidator<BookingGetAllQuery>
{
    public BookingGetAllQueryValidator()
    {
        RuleFor(x => x.Request.FacilityIds)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .Must(facilityIds => facilityIds!.Length > 0)
            .WithMessage("FacilityIds must contain at least one item.");

        RuleFor(x => x.Request.RestNumber)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0011)
            .GreaterThan(0)
            .WithErrorCode(ErrorCode.E0011);

        RuleFor(x => x.Request.RoomNumber)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0011)
            .GreaterThan(0)
            .WithErrorCode(ErrorCode.E0011);

        RuleFor(x => x.Request.CheckInDate)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0011)
            .GreaterThan(0)
            .WithErrorCode(ErrorCode.E0011)
            .Must(ValidDate.BeValidDate)
            .WithErrorCode(ErrorCode.E0008)
            .WithMessage(ErrorCode.E0008.GetEnumDescriptions());

        RuleFor(x => x.Request.Person)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0011)
            .GreaterThan(0)
            .WithErrorCode(ErrorCode.E0011);
    }
}
