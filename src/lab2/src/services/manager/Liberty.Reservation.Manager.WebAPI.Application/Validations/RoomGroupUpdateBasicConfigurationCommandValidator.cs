using System.Globalization;
using Liberty.ApplicationShared.Extensions;
using Liberty.Reservation.Application.Utils;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.RoomGroup;

namespace Liberty.Reservation.Manager.WebAPI.Application.Validations;

public class RoomGroupUpdateBasicConfigurationCommandValidator
    : AbstractValidator<RoomGroupUpdateBasicConfigurationCommand>
{
    public RoomGroupUpdateBasicConfigurationCommandValidator()
    {
        RuleFor(x => x.Payload.Id)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .GreaterThan(0)
            .WithErrorCode(ErrorCode.E0001);

        RuleFor(x => x.Payload.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithErrorCode(ErrorCode.E0001)
            .MaximumLength(255)
            .WithErrorCode(ErrorCode.E0002);

        RuleFor(x => x.Payload.BaseNumber)
            .Cascade(CascadeMode.Stop)
            .GreaterThanOrEqualTo(0)
            .WithErrorCode(ErrorCode.E0011);

        RuleFor(x => x.Payload.CapacityMin)
            .Cascade(CascadeMode.Stop)
            .GreaterThanOrEqualTo(0)
            .WithErrorCode(ErrorCode.E0011);

        RuleFor(x => x.Payload.CapacityMax)
            .Cascade(CascadeMode.Stop)
            .GreaterThanOrEqualTo(x => x.Payload.CapacityMin)
            .WithErrorCode(ErrorCode.E1021);

        RuleFor(x => x.Payload.Size)
            .Cascade(CascadeMode.Stop)
            .GreaterThanOrEqualTo(0)
            .WithErrorCode(ErrorCode.E0011)
            .Must(
                x => x == null || HasAtMostOneDecimalPlace(x.Value)
            )
            .WithErrorCode(ErrorCode.E1073)
            .WithMessage(ErrorCode.E1073.GetEnumDescriptions());

        RuleFor(x => x.Payload.BedTypes)
            .Cascade(CascadeMode.Stop)
            .Must(x => x.Any())
            .WithErrorCode(ErrorCode.E0001);

        RuleForEach(x => x.Payload.BedTypes)
            .Cascade(CascadeMode.Stop)
            .SetValidator(new BedTypeOfRoomGroupUpdateBasicConfigurationRequestValidator());
    }

    /// <summary>
    /// Determines whether a given decimal value has at most one decimal place.
    /// </summary>
    /// <param name="value">
    /// The decimal value to check, represented as a <see cref="decimal"/>.
    /// </param>
    /// <returns>
    /// <c>true</c> if the value has at most one decimal place; otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// This method converts the decimal value to a string using the invariant culture
    /// and checks the number of digits after the decimal point.
    /// </remarks>
    /// <example>
    /// <code>
    /// bool result = HasAtMostOneDecimalPlace(12.3m); // Returns true
    /// bool result = HasAtMostOneDecimalPlace(12.34m); // Returns false
    /// </code>
    /// </example>
    private static bool HasAtMostOneDecimalPlace(
        decimal value
    )
    {
        var parts = value.ToString("0.########", CultureInfo.InvariantCulture).Split('.');
        return parts.Length == 1 || parts[1].Length <= 1;
    }
}

public class BedTypeOfRoomGroupUpdateBasicConfigurationRequestValidator
    : AbstractValidator<BedTypeOfRoomGroupUpdateBasicConfigurationRequest>
{
    public BedTypeOfRoomGroupUpdateBasicConfigurationRequestValidator()
    {
        RuleFor(x => x.Id)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .GreaterThan(0)
            .WithErrorCode(ErrorCode.E0001);

        RuleFor(x => x.Number)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .GreaterThanOrEqualTo(0)
            .WithErrorCode(ErrorCode.E0011);
    }
}
