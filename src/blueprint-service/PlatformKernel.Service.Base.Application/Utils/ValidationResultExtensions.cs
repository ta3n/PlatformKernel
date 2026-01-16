using FluentValidation;
using FluentValidation.Results;
using PlatformKernel.SysException;

namespace PlatformKernel.Service.Base.Application.Utils;

/// <summary>
/// Provides extension methods for working with validation results and errors.
/// </summary>
public static class ValidationResultExtensions
{
    /// <summary>
    /// Retrieves the error code from the specified validation result by evaluating its errors.
    /// </summary>
    /// <param name="validationResult">
    /// The <see cref="ValidationResult"/> instance containing the error details.
    /// </param>
    /// <returns>
    /// The <see cref="ErrorCode"/> extracted from the validation result errors.
    /// </returns>
    public static ErrorCode GetErrorCode(
        this ValidationResult validationResult
    )
    {
        return validationResult.Errors.GetErrorCode();
    }

    /// <summary>
    /// Retrieves the error code from the list of validation failures.
    /// If no error code is found or is invalid, a default error code is returned.
    /// </summary>
    /// <param name="errors">
    /// A list of <see cref="ValidationFailure"/> objects representing validation errors.
    /// </param>
    /// <returns>
    /// The error code derived from the first validation failure in the list or a default error code if not found.
    /// </returns>
    public static ErrorCode GetErrorCode(
        this List<ValidationFailure> errors
    )
    {
        var code = errors.FirstOrDefault()?.ErrorCode;
        if (string.IsNullOrEmpty(code))
        {
            return ErrorCode.E0100;
        }

        var isErrorCodeDef = Enum.TryParse<ErrorCode>(code, out var errorCode);
        return isErrorCodeDef ? errorCode : ErrorCode.E0100;
    }

    /// <summary>
    /// Retrieves the error message from the given <see cref="ValidationResult"/> instance.
    /// </summary>
    /// <param name="validationResult">The validation result containing error details.</param>
    /// <returns>A string representing the error message.</returns>
    public static string GetErrorMessage(
        this ValidationResult validationResult
    )
    {
        return validationResult.Errors.GetErrorMessage();
    }

    /// <summary>
    /// Retrieves the error message from a list of validation failures.
    /// </summary>
    /// <param name="errors">The list of validation failures to extract the error message from.</param>
    /// <returns>A string containing the error message from the first validation failure, or an empty string if no errors are present.</returns>
    public static string GetErrorMessage(
        this List<ValidationFailure> errors
    )
    {
        return errors.FirstOrDefault()?.ErrorMessage ?? string.Empty;
    }

    /// Retrieves the field name associated with the first validation error in the ValidationResult.
    /// <param name="validationResult">The ValidationResult containing validation errors.</param>
    /// <returns>The name of the field that caused the validation error, or null if no errors are present.</returns>
    public static string GetErrorField(
        this ValidationResult validationResult
    )
    {
        return validationResult.Errors.GetErrorField();
    }

    /// Retrieves the property name of the first validation error from a list of validation failures,
    /// while removing a specified prefix if the property name matches any of the ignored fields.
    /// <param name="errors">The list of validation failures containing error details.</param>
    /// <returns>The property name of the first validation error, with any ignored field prefix removed, or an empty string if no errors are present.</returns>
    public static string GetErrorField(
        this List<ValidationFailure> errors
    )
    {
        var ignoredFields = new[] { "Payload" };

        var errorField = errors.FirstOrDefault()?.PropertyName;
        if (errorField is null)
        {
            return string.Empty;
        }

        var ignoredField = Array.Find(
            ignoredFields,
            field => errorField.Contains(field)
        );
        return ignoredField != null
            ? errorField.Replace($"{ignoredField}.", string.Empty)
            : errorField;
    }

    /// <summary>
    /// Adds a custom error code to the given rule in the FluentValidation pipeline.
    /// </summary>
    /// <typeparam name="T">The type of the object being validated.</typeparam>
    /// <typeparam name="TProperty">The type of the property being validated.</typeparam>
    /// <param name="rule">The rule builder options to which the error code is applied.</param>
    /// <param name="errorCode">The custom error code to associate with the rule.</param>
    /// <returns>The modified rule builder options with the specified error code applied.</returns>
    public static IRuleBuilderOptions<T, TProperty> WithErrorCode<T, TProperty>(
        this IRuleBuilderOptions<T, TProperty> rule,
        ErrorCode errorCode
    )
    {
        rule.WithErrorCode(errorCode.ToString());

        return rule;
    }
}
