using System.ComponentModel.DataAnnotations;

namespace Liberty.ApplicationShared.Attributes.Validations;

public class MaxWordValidationAttribute(
    int maxWords
) : ValidationAttribute("{0} has to many words.")
{
    protected override ValidationResult? IsValid(
        object? value,
        ValidationContext validationContext
    )
    {
        if (value is null)
        {
            return ValidationResult.Success;
        }

        var textValue = value.ToString() ?? string.Empty;
        if (textValue.Length <= maxWords)
        {
            return ValidationResult.Success;
        }

        var errorMessage = FormatErrorMessage(validationContext.DisplayName);
        return new ValidationResult(errorMessage);
    }
}
