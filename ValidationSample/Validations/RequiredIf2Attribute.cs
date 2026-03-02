using DynamicExpresso;
using System.ComponentModel.DataAnnotations;

namespace ValidationSample.Validations
{
    public class RequiredIf2Attribute(string condition) : RequiredAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var interpreter = new Interpreter()
                .SetVariable("Model", validationContext.ObjectInstance)
                .SetVariable("Value", value);

            var evaluated = interpreter.Eval<bool>(condition);

            if (evaluated)
                return base.IsValid(value, validationContext);
            return ValidationResult.Success;
        }
    }
}
