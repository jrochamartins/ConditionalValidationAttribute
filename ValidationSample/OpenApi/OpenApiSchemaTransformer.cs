using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using System.Text.Json;

namespace ValidationSample.OpenApi
{
    public class OpenApiSchemaTransformer : IOpenApiSchemaTransformer
    {
        public Task TransformAsync(OpenApiSchema schema, OpenApiSchemaTransformerContext context, CancellationToken cancellationToken)
        {
            var typeProperties = context.JsonTypeInfo?.Type.GetProperties();

            if (schema.Properties is null || typeProperties is null)
                return Task.CompletedTask;

            foreach (var property in typeProperties)
            {
                var modifiers = property.GetCustomAttributes(typeof(IOpenApiPropertyModifier), false);
                               
                if (modifiers.Length == 0)
                    continue;
                                
                var jsonPropName = JsonNamingPolicy.CamelCase.ConvertName(property.Name);

                if (!schema.Properties.TryGetValue(jsonPropName, out var propertySchemaInterface) ||
                    propertySchemaInterface is not OpenApiSchema propertySchema)
                    continue;
                                
                foreach (IOpenApiPropertyModifier modifier in modifiers.Cast<IOpenApiPropertyModifier>())
                    modifier.ApplyModifier(schema, propertySchema, jsonPropName);
            }

            return Task.CompletedTask;
        }
    }
}