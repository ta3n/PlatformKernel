using System.ComponentModel.DataAnnotations;

namespace Liberty.Application.Attributes.Validations
{
    public class UsernameValidationAttribute : ValidationAttribute
    {


        protected override  ValidationResult IsValid(object value, ValidationContext validationContext)
        {
           if (value != null)
            {

                string propertyName = validationContext.MemberName;
                string newPassword = value.ToString();

                if (newPassword.Length <= 3)
                {
                    return new ValidationResult("ユーザ名は少なくとも3文字必要です。", new string[]{ propertyName });
                }

                // FIXME 他のカスタムバリデーションルールを追加

                // パスワードが要件を満たしていない場合は、エラーメッセージを返す
            }

            return ValidationResult.Success;
        }
    }
}
