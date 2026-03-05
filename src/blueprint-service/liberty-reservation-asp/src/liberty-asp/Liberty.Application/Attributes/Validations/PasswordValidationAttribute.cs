using System.ComponentModel.DataAnnotations;

namespace Liberty.Application.Attributes.Validations
{
    public class PasswordValidationAttribute : ValidationAttribute
    {


        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string propertyName = validationContext.MemberName;


            if (value==null || string.IsNullOrEmpty(value.ToString())) return new ValidationResult("required.", new string[] { propertyName });


            //
            string newPassword = value.ToString();

            // FIXMEパスワードのカスタムバリデーションロジックを実装
            if (newPassword.Length < 6)
            {
                return new ValidationResult("パスワードは少なくとも6文字必要です。", new string[] { propertyName });
            }

            // 他のカスタムバリデーションルールを追加

            // パスワードが要件を満たしていない場合は、エラーメッセージを返す

            return ValidationResult.Success;
        }
    }
}
