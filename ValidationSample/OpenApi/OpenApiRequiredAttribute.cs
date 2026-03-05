using Microsoft.OpenApi;

namespace ValidationSample.OpenApi
{
    [AttributeUsage(AttributeTargets.Property)]
    public class OpenApiRequiredAttribute : Attribute, IOpenApiPropertyModifier
    {
        public void ApplyModifier(OpenApiSchema parentSchema, OpenApiSchema propertySchema, string jsonPropertyName)
        {
            parentSchema.Required ??= new HashSet<string>();
            parentSchema.Required.Add(jsonPropertyName);

            if (propertySchema.Type.HasValue)
                propertySchema.Type &= ~JsonSchemaType.Null;
        }
    }
}
