using Microsoft.OpenApi;

namespace ValidationSample.OpenApi
{
    public interface IOpenApiPropertyModifier
    {   
        void ApplyModifier(OpenApiSchema parentSchema, OpenApiSchema propertySchema, string jsonPropertyName);
    }
}
