using Microsoft.OpenApi;

namespace ValidationSample.Filters
{
    public interface IOpenApiPropertyModifier
    {   
        void ApplyModifier(OpenApiSchema parentSchema, OpenApiSchema propertySchema, string jsonPropertyName);
    }
}
