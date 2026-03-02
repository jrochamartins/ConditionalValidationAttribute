using System.ComponentModel.DataAnnotations;
using Z.Expressions;

namespace ValidationSample.Validations
{
    public class WithZExpresssionsRequiredIfAttribute(string condition) : RequiredAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var evaluateFunction = Eval.Compile<Func<object?, bool>>(condition, "Model");
            var evaluated = evaluateFunction(validationContext.ObjectInstance);

            if (evaluated)
                return base.IsValid(value, validationContext);
            return ValidationResult.Success;
        }
    }
}
