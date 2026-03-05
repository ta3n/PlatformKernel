using FluentValidation;
using FluentValidation.Results;
using Liberty.ApplicationShared.Extensions;
using Liberty.SysException;
using Microsoft.OpenApi.Extensions;

namespace Liberty.Reservation.Manager.Distribution.WebAPI.Application.Web.Extensions;

public static class ValidationExtension
{
    private const string DisplayValue = "[Blank]";

    public static IRuleBuilder<T, string?> WithBlankAwareMessage<T>(
        this IRuleBuilder<T, string?> ruleBuilder,
        string propertyName,
        ErrorCode errorCode = ErrorCode.E5003
    )
    {
        return ruleBuilder.Custom(
            (
                value,
                context
            ) =>
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    return;
                }

                var message = string.Format(errorCode.GetEnumDescriptions(), propertyName, DisplayValue);

                context.AddFailure(new ValidationFailure(propertyName, message) { ErrorCode = errorCode.GetDisplayName() });
            }
        );
    }

    public static IRuleBuilder<T, long?> WithBlankAwareMessage<T>(
        this IRuleBuilder<T, long?> ruleBuilder,
        string propertyName,
        ErrorCode errorCode = ErrorCode.E5003
    )
    {
        return ruleBuilder.Custom(
            (
                value,
                context
            ) =>
            {
                if (value is not null)
                {
                    return;
                }

                var message = string.Format(errorCode.GetEnumDescriptions(), propertyName, DisplayValue);

                context.AddFailure(new ValidationFailure(propertyName, message) { ErrorCode = errorCode.GetDisplayName() });
            }
        );
    }

    public static IRuleBuilder<T, int?> WithBlankAwareMessage<T>(
        this IRuleBuilder<T, int?> ruleBuilder,
        string propertyName,
        ErrorCode errorCode = ErrorCode.E5003
    )
    {
        return ruleBuilder.Custom(
            (
                value,
                context
            ) =>
            {
                if (value is not null)
                {
                    return;
                }

                var message = string.Format(errorCode.GetEnumDescriptions(), propertyName, DisplayValue);

                context.AddFailure(new ValidationFailure(propertyName, message) { ErrorCode = errorCode.GetDisplayName() });
            }
        );
    }
}
