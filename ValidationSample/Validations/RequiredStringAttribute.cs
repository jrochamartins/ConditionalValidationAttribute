using System.ComponentModel.DataAnnotations;

namespace ValidationSample.Validations
{
    public class RequiredStringAttribute : RequiredAttribute
    {
        public RequiredStringAttribute() => AllowEmptyStrings = false;
        public RequiredStringAttribute(bool allowEmptyStrings) => AllowEmptyStrings = allowEmptyStrings;
    }
}
