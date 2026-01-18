using System.ComponentModel.DataAnnotations;

namespace Liberty.ApplicationShared.Attributes.Validations;

public class PasswordValidationAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(
        object? value,
        ValidationContext validationContext
    )
    {
        var propertyName = validationContext.MemberName ?? string.Empty;

        if (value == null || string.IsNullOrEmpty(value.ToString()))
        {
            return new ValidationResult(
                "required.",
                new[]
                {
                    propertyName
                }
            );
        }

        //
        var newPassword = value.ToString() ?? string.Empty;

        // FIXMEパスワードのカスタムバリデーションロジックを実装
        if (newPassword.Length < 6)
        {
            return new ValidationResult(
                "パスワードは少なくとも6文字必要です。",
                new[]
                {
                    propertyName
                }
            );
        }

        // 他のカスタムバリデーションルールを追加

        // パスワードが要件を満たしていない場合は、エラーメッセージを返す

        return ValidationResult.Success;
    }
}
