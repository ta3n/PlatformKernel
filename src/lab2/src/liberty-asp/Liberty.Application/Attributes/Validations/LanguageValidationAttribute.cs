using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberty.Application.Attributes.Validations
{
    public class LanguageValidationAttribute : ValidationAttribute
    {


        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string propertyName = validationContext.MemberName;


            if (value == null || string.IsNullOrEmpty(value.ToString())) return new ValidationResult("required.", new string[] { propertyName });

            var codes = new List<string> { "ja" };
            //
            string language = value.ToString();

            // パスワードのカスタムバリデーションロジックを実装
            if (!codes.Contains(language))
            {
                return new ValidationResult("不正なコードです", new string[] { propertyName });
            }

            // 他のカスタムバリデーションルールを追加

            // パスワードが要件を満たしていない場合は、エラーメッセージを返す

            return ValidationResult.Success;
        }
    }
}
