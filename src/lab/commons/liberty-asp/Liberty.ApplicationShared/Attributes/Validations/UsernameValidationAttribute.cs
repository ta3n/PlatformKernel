using System.ComponentModel.DataAnnotations;

namespace Liberty.ApplicationShared.Attributes.Validations;

public class UsernameValidationAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(
        object? value,
        ValidationContext validationContext
    )
    {
        if (value is not null)
        {
            var propertyName = validationContext.MemberName ?? string.Empty;
            var newPassword = value.ToString();

            if (newPassword is { Length: <= 3 })
            {
                return new ValidationResult(
                    "ユーザ名は少なくとも3文字必要です。",
                    new[]
                    {
                        propertyName
                    }
                );
            }

            // FIXME 他のカスタムバリデーションルールを追加

            // パスワードが要件を満たしていない場合は、エラーメッセージを返す
        }

        return ValidationResult.Success;
    }
}
