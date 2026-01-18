using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberty.Application.Attributes.Validations
{
    public class MaxWordValidationAttribute : ValidationAttribute
    {
        private readonly int _maxWords;
        public MaxWordValidationAttribute(int maxWords)
            : base("{0} has to many words.")
        {
            _maxWords = maxWords;
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null) return ValidationResult.Success;
            var textValue = value.ToString();
            if (textValue.Length <= _maxWords) return ValidationResult.Success;
            var errorMessage = FormatErrorMessage((validationContext.DisplayName));
            return new ValidationResult(errorMessage);
        }
    }
}
