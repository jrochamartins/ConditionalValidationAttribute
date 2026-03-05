using Microsoft.OpenApi;

namespace ValidationSample.Filters
{
    [AttributeUsage(AttributeTargets.Property)]
    public class OpenApiMinLengthAttribute : Attribute, IOpenApiPropertyModifier
    {
        private readonly int _minLength;

        public OpenApiMinLengthAttribute(int minLength)
        {
            _minLength = minLength;
        }

        public void ApplyModifier(OpenApiSchema parentSchema, OpenApiSchema propertySchema, string jsonPropertyName)
        {
            propertySchema.MinLength = _minLength;
        }
    }
}
