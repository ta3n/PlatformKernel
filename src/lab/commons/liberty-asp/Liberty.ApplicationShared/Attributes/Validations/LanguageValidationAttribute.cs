using System.ComponentModel.DataAnnotations;

namespace Liberty.ApplicationShared.Attributes.Validations;

public class LanguageValidationAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(
        object? value,
        ValidationContext validationContext
    )
    {
        var propertyName = validationContext.MemberName ?? string.Empty;

        if (value is null || string.IsNullOrEmpty(value.ToString()))
        {
            return new ValidationResult(
                "required.",
                new[]
                {
                    propertyName
                }
            );
        }

        var codes = new List<string> { "ja" };
        //
        var language = value.ToString() ?? string.Empty;

        // パスワードのカスタムバリデーションロジックを実装
        if (!codes.Contains(language))
        {
            return new ValidationResult(
                "不正なコードです",
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
