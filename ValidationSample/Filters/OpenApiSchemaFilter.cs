using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using System.Text.Json;
using ValidationSample.Models;

namespace ValidationSample.Filters
{
    public class OpenApiSchemaFilter : IOpenApiSchemaTransformer
    {
        public Task TransformAsync(OpenApiSchema schema, OpenApiSchemaTransformerContext context, CancellationToken cancellationToken)
        {
            var typeProperties = context.JsonTypeInfo?.Type.GetProperties();

            if (schema.Properties is null ||
                typeProperties is null)
                return Task.CompletedTask;

            foreach (var property in typeProperties)
            {
                var jsonPropName = JsonNamingPolicy.CamelCase.ConvertName(property.Name);

                if (schema.Properties.TryGetValue(jsonPropName, out var genericSchema) &&
                     genericSchema is OpenApiSchema)
                {
                    if (property.GetCustomAttributes(typeof(OpenApiRequiredAttribute), false).Length > 0)
                    {
                        schema.Required ??= new HashSet<string>();
                        schema.Required.Add(jsonPropName);
                    }
                }
            }

            return Task.CompletedTask;
        }
    }
}