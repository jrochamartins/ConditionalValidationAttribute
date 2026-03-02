using System.ComponentModel.DataAnnotations;

namespace ValidationSample.Validations
{
    public class RequiredStringAttribute : RequiredAttribute
    {
        public RequiredStringAttribute(bool allowEmptyStrings = false) => AllowEmptyStrings = allowEmptyStrings;
    }
}
